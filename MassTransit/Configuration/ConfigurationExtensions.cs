using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using GreenPipes;
using ESS.FW.ServiceBus.MassTransit.Observers;
using MassTransit;
using MassTransit.Internals.Extensions;
using MassTransit.RabbitMqTransport;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.Extensions.Logging;
using ESS.FW.Common.Components;
using ESS.FW.Common.Autofac;
using ESS.FW.Common.Attributes;

namespace ESS.FW.ServiceBus.MassTransit.Configuration
{
    /// <summary>
    ///     configuration class Autofac extensions.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        ///     Use MassTransit for message bus.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="assembles"></param>
        /// <param name="endpointName"></param>
        /// <param name="retry"></param>
        /// <param name="isTransaction">Encapsulate the pipe behavior in a transaction</param>
        /// <param name="prefetchCount">Specify the maximum number of concurrent messages that are consumed</param>
        /// <returns></returns>
        public static Common.Configurations.Configuration UseMassTransit(
            this Common.Configurations.Configuration configuration,BusConfig config, Assembly[] assembles = null
            )
        {
            var objectContainer = ObjectContainer.Current as AutofacObjectContainer;
            string assemblyName = Assembly.GetCallingAssembly().GetName().Name;
            string endpointName = string.Empty;
            if (string.IsNullOrEmpty(config.EndpointName))
            {
                endpointName = assemblyName;
            }
            string endPoint = endpointName;
            if (objectContainer != null)
            {
                var container = objectContainer.Container;
                var builder = new ContainerBuilder();

                //consumer 不能加component，不然会注册2次

                if (assembles != null)
                    foreach (var ass in assembles)
                    {
                        try
                        {
                            builder.RegisterConsumers(ass);
                        }
                        catch (ReflectionTypeLoadException loadException)
                        {
                            //目前写死判断，少dll报错，多dll跳过
                            var message = string.Join(",",
                                loadException.LoaderExceptions.Select(c => c.Message).Distinct());
                            if (message.Contains("系统找不到指定的文件"))
                            {
                                throw new ApplicationException(message);
                            }
                        }
                    }

                var clusterHosts = config.Ip.Split(',');
                builder.Register(context =>
                {
                    var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        config.Host = string.Format("rabbitmq://{0}/{1}",
                            clusterHosts[0],
                            string.IsNullOrEmpty(config.VirtualHost)
                                ? ""
                                : config.VirtualHost + "/");
                        var host = cfg.Host(new Uri(config.Host), h =>
                        {
                            h.Username(config.UserName);
                            h.Password(config.Password);
                            if (clusterHosts.Length > 0)
                            {
                                h.UseCluster(c =>
                                {
                                    foreach (var clusterHost in clusterHosts)
                                    {
                                        c.Node(clusterHost);
                                    }
                                });
                            }
                        });
                        //以机器名为消息总线队列名的方式暂保留
                        var endpointNames = new ArrayList { endpointName };


                        if (config.Retry > 0)
                        {
                            cfg.UseRetry(retryConfig => retryConfig.Incremental(config.Retry, new TimeSpan(0, 0, 5), new TimeSpan(0, 0, 5)));
                        }
                        //绑定消息总线
                        foreach (var a in endpointNames)
                        {
                            cfg.ReceiveEndpoint(host, a.ToString(), ec =>
                            {

                                if (config.IsTransaction)
                                {
                                    ec.UseTransaction(x => { x.Timeout = TimeSpan.FromSeconds(300); });
                                }

                                ec.PrefetchCount = config.PrefetchCount;
                                ec.UseConcurrencyLimit(config.PrefetchCount);

                                ec.LoadFrom(context, MatchingScopeLifetimeTags.RequestLifetimeScopeTag.ToString());
                            });
                        }
                        ConfigCustomMessage(host, cfg, context);
                        cfg.ConfigureJsonSerializer(c => new JsonSerializerSettings()
                        {
                            Converters = new List<JsonConverter>
                            {
                                new IsoDateTimeConverter(),
                                new StringEnumConverter {CamelCaseText = true}
                            },
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            TypeNameHandling = TypeNameHandling.Auto
                            //NullValueHandling = NullValueHandling.Ignore
                        });
                    });

                    return busControl;
                })
                    .SingleInstance()
                    .As<IBusControl>()
                    .As<IBus>();
                builder.Update(container);

                var bc = container.Resolve<IBusControl>();
                configuration.SetDefault<Common.ServiceBus.IBus, MassTransitBus>(new MassTransitBus(bc));
                var receiveObserver = new ReceiveObserver(container.Resolve<ILoggerFactory>());
                var receiveHandle = bc.ConnectReceiveObserver(receiveObserver);

                var publishObserver = new PublishObserver(container.Resolve<ILoggerFactory>());
                var publishHandle = bc.ConnectPublishObserver(publishObserver);

                var sendObserver = new SendObserver(container.Resolve<ILoggerFactory>());
                var shendHandle = bc.ConnectSendObserver(sendObserver);
                //var result = bc.GetProbeResult();
                //Console.WriteLine(result.ToJsonString());

                ObjectContainer.Current.RegisterGeneric(typeof(Common.ServiceBus.IRequestClient<,>),
                    typeof(Impl.RequestClient<,>),
                    LifeStyle.Transient);

                bc.Start();
            }


            return configuration;
        }

        /// <summary>
        /// 处理自定义消息，为配置了MessageAttribute的消息
        /// </summary>
        /// <param name="host"></param>
        /// <param name="cfg"></param>
        /// <param name="context"></param>
        private static void ConfigCustomMessage(IRabbitMqHost host, IRabbitMqBusFactoryConfigurator cfg, IComponentContext context)
        {
            IList<Type> concreteTypes = FindTypes(context, r => true, typeof(IConsumer));
            foreach (var concreteType in concreteTypes)
            {
                var attrs = concreteType.GetAttribute<MessageAttribute>();
                foreach (var attr in attrs)
                {
                    var qn = attr.QueueName;
                    if (!attr.IsFullUrl)
                    {
                        qn = qn + "-" + Environment.MachineName;
                    }
                    cfg.ReceiveEndpoint(host, qn, ec =>
                    {
                        ec.PrefetchCount = attr.Prefetch;
                        ec.UseConcurrencyLimit(attr.Prefetch);
                        ec.LoadFrom(context, MatchingScopeLifetimeTags.RequestLifetimeScopeTag.ToString());

                    });
                }
                #region TODO routekey
                //var messages = concreteType.GetInterfaces().Where(c => c.IsGenericType).Select(c=>c.GenericTypeArguments[0]);
                //if(messages != null && messages.Count()>0)
                //{
                //    foreach(var msg in messages)
                //    {
                //        var attrs = msg.GetAttribute<MessageAttribute>();
                //        foreach(var attr in attrs)
                //        {
                //            cfg.ReceiveEndpoint(host, attr.QueueName, ec =>
                //            {
                //                ec.LoadFrom(context, MatchingScopeLifetimeTags.RequestLifetimeScopeTag.ToString());
                //            });
                //        }
                //    }
                //}
                #endregion
            }
        }


        private static IList<Type> FindTypes(IComponentContext scope, Func<Type, bool> filter, Type interfaceType)
        {
            return scope.ComponentRegistry.Registrations
                .SelectMany(r => r.Services.OfType<IServiceWithType>(), (r, s) => new { r, s })
                .Where(rs => rs.s.ServiceType.HasInterface(interfaceType))
                .Select(rs => rs.s.ServiceType)
                .Where(filter)
                .ToList();
        }
    }
}