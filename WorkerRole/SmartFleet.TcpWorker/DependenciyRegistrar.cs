using Autofac;
using MassTransit;
using SmartFleet.Core.Infrastructure.MassTransit;
using SmartFleet.Core.ReverseGeoCoding;
using TeltonikaListner;

namespace SmartFleet.TcpWorker
{
    public static class DependenciyRegistrar
    {
        static IContainer Container { get; set; }
        static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ReverseGeoCodingService>();
            var bus = MassTransitConfig.ConfigureSenderBus();
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
