using System;
using System.Reflection;
using ESS.FW.Common.Autofac;
using ESS.FW.Common.Components;
using ESS.FW.Common.ServiceBus;
using ESS.FW.ServiceBus.MassTransit.Configuration;
using Microsoft.Extensions.Logging;

namespace MassTransit.Tests.Consumer
{
    internal class Program
    {
        private static IBus _bus;

        private static void Main(string[] args)
        {
            //var factory = new ConnectionFactory() { HostName = "10.3.5.95", UserName = "jzt", Password = "jzt" };
            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //    //channel.QueueDeclare(queue: "hello",
            //    //                     durable: false,
            //    //                     exclusive: false,
            //    //                     autoDelete: false,
            //    //                     arguments: null);

            //    var consumer = new EventingBasicConsumer(channel);
            //    consumer.Received += (model, ea) =>
            //    {
            //        var body = ea.Body;
            //        var message = Encoding.UTF8.GetString(body);
            //        //if(message.IndexOf("0-")>-1) throw new Exception(message);
            //        //Console.WriteLine(" [x] Received {0}", message);
            //        Console.WriteLine(" [x] Received {0}", message.Substring(message.IndexOf("SystemVersion"),50));
            //    };
            //    channel.BasicConsume(queue: "ESS.FW.ServiceBus.MassTransit.Tests.Consumer-DESKTOP-MR61830",
            //                         noAck: true,
            //                         consumer: consumer);

            //    Console.WriteLine(" Press [enter] to exit.");
            //    Console.ReadLine();
            //}
            Setup();


            Console.ReadKey();
        }

        public static void Setup()
        {
            var assambly = Assembly.GetAssembly(typeof (Program));
            var config = ESS.FW.Common.Configurations.Configuration.Create()
                .UseAutofac()
                .RegisterCommonComponents()
                //.UseEntLibLog()
                ;

            //config.SetDefault<IConsumeObserver, ConsumeObserver>(LifeStyle.Transient);

            //config.SetDefault<IIdGenerator, IdGeneratorRepository>(LifeStyle.Transient);

            //config.UseEfRepository(typeof(JztDbContext));
            
            config.SetDefault<ILoggerFactory, LoggerFactory>();
            var busConfig = new BusConfig()
            {
                Ip = "10.3.5.95",
                UserName="admin",
                Password="admin"
            };
            config.UseMassTransit(busConfig,new[] {assambly});

            using (var scope = ObjectContainer.BeginLifetimeScope())
            {
                _bus = scope.Resolve<IBus>();
            }
        }
    }
}