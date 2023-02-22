using Autofac;
using EdgeService.Handler;
using MassTransit;
using SmartFleet.Core;
using SmartFleet.Core.Data;
using SmartFleet.Core.Infrastructure.MassTransit;
using SmartFleet.Data;
using SmartFleet.Data.Dbcontextccope.Implementations;

namespace EdgeService
{
    class Program
    {
        private static IContainer Container { get; set; }

        static void Main(string[] args)
        {
            
            var builder = new ContainerBuilder();
            builder.RegisterType<DbContextScopeFactory>().As<IDbContextScopeFactory>();
            builder.RegisterType<AmbientDbContextLocator>().As<IAmbientDbContextLocator>();
            builder.RegisterGeneric(typeof(EfScopeRepository<>)).As(typeof(IScopeRepository<>)).InstancePerLifetimeScope();
            var bus = RabbitMqConfig.InitReceiverBus<TeltonikaedgeHandler>("teltonika");
            builder.RegisterInstance(bus).As<IBusControl>();
            Container = builder.Build();
         
            Container.Resolve<IAmbientDbContextLocator>();
            bus.StartAsync();

        }

        public static IScopeRepository<T> ScopeRepository<T>() where T: BaseEntity
        {
            return Container.Resolve<IScopeRepository<T>>();

        }

        public static IDbContextScopeFactory ResolveDbContextScopeFactory()
        {
            return Container.Resolve<IDbContextScopeFactory>();
        }


    }
}
