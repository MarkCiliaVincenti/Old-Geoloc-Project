using System.Configuration;
using System.IO;
using Autofac;
using MassTransit;
using Serilog;
using SmartFleet.Core.Infrastructure.MassTransit;
using SmartFleet.Core.ReverseGeoCoding;
using SmartFleet.Data;
using TeltonikaListner;

namespace SmartFleet.TcpWorker
{
    public static class DependencyRegistrar
    {
        static IContainer Container { get; set; }
        static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ReverseGeoCodingService>();
            var bus = RabbitMqConfig.ConfigureSenderBus();
            builder.RegisterInstance(bus).As<IBusControl>();
            builder.RegisterInstance(InitLog()).As<ILogger>();
            builder.Register(c => new RedisConnectionManager(ConfigurationManager.AppSettings["RedisUrl"],  ConfigurationManager.AppSettings["redisPass"])).As<IRedisConnectionManager>();
            builder.RegisterType<RedisCache>().As<IRedisCache>();
            builder.RegisterType<TeltonikaTcpServer>().SingleInstance();
            return builder.Build();
        }

        public static void ResolveDependencies()
        {
            Container = BuildContainer();
            Container.Resolve<ReverseGeoCodingService>();
            Container.Resolve<IBusControl>();
            Container.Resolve<IRedisCache>();
            Container.Resolve<ILogger>();
           
            var path = Directory.GetCurrentDirectory(); 
            MicroServicesLoader.Loader(path);

        }

        public static TeltonikaTcpServer StartListener()
        {
            var listener = Container.Resolve<TeltonikaTcpServer>();
            return listener;
        }
        private static ILogger InitLog()
        {
            var log = new LoggerConfiguration()
                // .WriteTo.Console()
                .WriteTo.File("tcp-worker-role.txt")
                .CreateLogger();
            return log;
        }
    }
}
