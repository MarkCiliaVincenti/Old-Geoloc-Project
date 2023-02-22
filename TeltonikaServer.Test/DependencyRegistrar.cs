using SmartFleet.Core.Infrastructure.MassTransit;
using Autofac;
using MassTransit;
using SmartFleet.Core.ReverseGeoCoding;

namespace TeltonikaServer.Test
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
            builder.RegisterType<TeltonikaTcpServer>();
            return builder.Build();
        }

        public static TeltonikaTcpServer ResolveDependencies()
        {
            Container = BuildContainer();
            Container.Resolve<ReverseGeoCodingService>();
            Container.Resolve<IBusControl>();
            var listner = Container.Resolve<TeltonikaTcpServer>();
            return listner;
        }

    }
}
