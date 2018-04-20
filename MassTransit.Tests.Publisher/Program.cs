using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ESS.FW.Common.Autofac;
using ESS.FW.Common.Components;
using ESS.FW.ServiceBus.MassTransit.Configuration;
using ESS.FW.ServiceBus.MassTransit.Impl;
using ESS.Shared.Events.Common.Test;
using Microsoft.Extensions.Logging;
using IBus = ESS.FW.Common.ServiceBus.IBus;

namespace MassTransit.Tests.Publisher
{
    internal class Program
    {
        private static ESS.FW.Common.ServiceBus.IBus _bus;

        private static void Main(string[] args)
        {

            Setup();

            while (true)
            {
                Console.WriteLine("Enter message (or quit to exit)");
                Console.Write("> ");
                var value = Console.ReadLine();

                if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                var message = new HpImplementation { SystemVersion = value + "-" + DateTime.Now };
                if ("send".Equals(value, StringComparison.OrdinalIgnoreCase))
                    _bus.Send("MassTransit.Tests.Consumer", message);
                else if ("rr" == value)
                    using (var scope = ObjectContainer.BeginLifetimeScope())
                    {
                        var client = scope.Resolve<ESS.FW.Common.ServiceBus.IRequestClient<Request, RequestResult>>();
                        Task.Run(() =>
                        {
                            var result =
                                client.Request("MassTransit.Tests.Consumer",
                                    new Request { Message = "request" }, new CancellationToken()).Result;
                            Console.WriteLine(result.Message);
                        });
                    }
                //else if ("trans" == value)
                //    _bus.Publish(new TransactionEvent { Message = "transaction test" });
                else if ("order" == value)
                    for (var i = 0; i < 100; i++)
                        _bus.Send("MassTransit.Tests.Consumer",
                            new HpImplementation { SystemVersion = i + "-" + DateTime.Now }, true);
                //else if ("orderp" == value)
                //    for (var i = 0; i < 100; i++)
                //        _bus.OrderPublish(
                //            new HpImplementation { SystemVersion = i + "-" + DateTime.Now });
                else if ("attr" == value)
                {
                    var msg = new AttributeEvent();
                    msg.Message = DateTime.Now.ToString();
                    _bus.Send("AttributeEvent", msg, true);
                }
                else
                    _bus.Send("MassTransit.Tests.Consumer", message, true);
            }

            Console.ReadKey();
        }

        public static void Setup()
        {
            var busConfig = new BusConfig()
            {
                Ip = "10.3.5.95",
                UserName = "admin",
                Password = "admin"
            };
            var assambly = Assembly.GetAssembly(typeof(Program));
            var config = ESS.FW.Common.Configurations.Configuration.Create()
                .UseAutofac()
                .RegisterCommonComponents();

            config.SetDefault<ILoggerFactory, LoggerFactory>();

            config.UseMassTransit(busConfig, new[] { assambly });
            
            using (var scope = ObjectContainer.BeginLifetimeScope())
            {
                _bus = scope.Resolve<ESS.FW.Common.ServiceBus.IBus>();
            }
        }
    }
}