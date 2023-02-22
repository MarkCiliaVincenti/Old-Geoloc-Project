using System;
using Autofac;
using MassTransit;
using SmartFleet.Core;
using SmartFleet.Core.Data;
using SmartFleet.Core.Infrastructure.MassTransit;
using SmartFleet.Core.Infrastructure.Registration;
using SmartFleet.Core.ReverseGeoCoding;
using SmartFleet.Data;
using SmartFleet.Data.Dbcontextccope.Implementations;
using TeltonicaService.Handlers;

namespace TeltonicaService.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private static IContainer Container { get; set; }

        public void Register(ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            // registration of dependencies
            builder.RegisterType<DbContextScopeFactory>().As<IDbContextScopeFactory>();
            builder.RegisterType<AmbientDbContextLocator>().As<IAmbientDbContextLocator>();
            builder.RegisterType<ReverseGeoCodingService>().SingleInstance();

            builder.RegisterGeneric(typeof(EfScopeRepository<>)).As(typeof(IScopeRepository<>))
                .InstancePerLifetimeScope();
            builder.Register(context => RabbitMqConfig.InitReceiverBus<TeltonikaHandler>("TeltonikaHandler.endpoint"))
                .SingleInstance()
                .As<IBusControl>()
                .As<IBus>();
            Container = builder.Build();

            Container.Resolve<IAmbientDbContextLocator>();
        }

        public static IScopeRepository<T> ScopeRepository<T>() where T : BaseEntity
        {
            return Container.Resolve<IScopeRepository<T>>();

        }
        public static IDbContextScopeFactory ResolveDbContextScopeFactory()
        {
            return Container.Resolve<IDbContextScopeFactory>();
        }

        public static IBusControl ResolveServiceBus()
        {
            return Container.Resolve<IBusControl>();
        }
        public static ReverseGeoCodingService ResolveGeoCodeService()
        {
            return Container.Resolve<ReverseGeoCodingService>();
        }
    }
}
