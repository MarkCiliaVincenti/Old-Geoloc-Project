using Autofac;
using MassTransit;
using SmartFleet.Core.Infrastructure.MassTransit;
using SmartFleet.Core.ReverseGeoCoding;
using IContainer = Autofac.IContainer;

namespace TeltonikaListner
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
            builder.RegisterType<TeltonikaServer>();
            return builder.Build();
        }

        public static TeltonikaServer ResolveTeltonicaListner()
        {
            Container = BuildContainer();
            Container.Resolve<ReverseGeoCodingService>();
            Container.Resolve<IBusControl>();
            var listner = Container.Resolve<TeltonikaServer>();
            return listner;
        }

    }
}
