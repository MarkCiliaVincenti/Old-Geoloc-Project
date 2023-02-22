using System;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using MassTransit;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SmartFleet.Core.Data;
using SmartFleet.Core.Infrastructure.MassTransit;
using SmartFleet.Core.ReverseGeoCoding;
using SmartFleet.Data;
using SmartFleet.Data.Dbcontextccope.Implementations;
using SmartFleet.Service.Authentication;
using SmartFleet.Service.Common;
using SmartFleet.Service.Customers;
using SmartFleet.Service.Tracking;
using SmartFleet.Service.Vehicles;
using SmartFLEET.Web.Automapper;
using SmartFLEET.Web.Hubs;
using System.Web;
using MediatR;
using Microsoft.ApplicationInsights.Extensibility;
using SmartFleet.Customer.Domain;
using SmartFleet.Customer.Domain.Common.DomainMapping;
using SmartFleet.MobileUnit.Domain;
using SmartFleet.MobileUnit.Domain.Common;
using SmartFleet.Web.Framework.DataTables;

namespace SmartFLEET.Web
{
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // Skip authenticating all ext.axd embedded resources (.js, .css, images)
            if (HttpContext.Current.Request.FilePath.EndsWith("/ext.axd"))
            {
                HttpContext.Current.SkipAuthorization = true;
            }
        }


        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var culture = Request["culture"] ?? Request.Cookies["culture"]?.Name;
            if (culture == null) culture = "en";
            var ci = CultureInfo.GetCultureInfo(culture);

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            var cookie = new HttpCookie("culture", ci.Name);
            Response.Cookies.Add(cookie);
        }
        protected void Application_Start()
        {
            
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = false;
           // HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();

            // seeds init data
            SeedInitData.SeedInitialData();

            #region register different services and classes using autofac
#if DEBUG
            TelemetryConfiguration.Active.DisableTelemetry = true;
#endif
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterAssemblyTypes(typeof(MvcApplication).Assembly)
                .AsImplementedInterfaces();

            builder.RegisterModule(new AutofacWebTypesModule());
           //builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<SmartFleetObjectContext>().As<SmartFleetObjectContext>();

            builder.RegisterType<UserStore<IdentityUser>>().As<IUserStore<IdentityUser>>();
            builder.RegisterType<RoleStore<IdentityRole>>().As<IRoleStore<IdentityRole, string>>();
             builder.RegisterType<UserManager<IdentityUser>>();
           
            builder.RegisterType<AuthenticationService>().As<IAuthenticationService>();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<DbContextScopeFactory>().As<IDbContextScopeFactory>();
            builder.RegisterType<ReverseGeoCodingService>().As<ReverseGeoCodingService>();
            builder.RegisterType<VehicleService>().As<IVehicleService>();
            builder.RegisterType<CustomerService>().As<ICustomerService>();

            builder.RegisterType<PositionService>().As<IPositionService>();
            builder.RegisterType<CustomerService>().As<ICustomerService>();
            builder.RegisterType<PdfService>().As<IPdfService>();
             #endregion

            #region automapper

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new SmartFleetAdminMappings());
                cfg.AddProfile(new CustomerDomainMapping());
                cfg.AddProfile(new MobileUnitDomainMapping());
            });
            var mapper = mapperConfiguration.CreateMapper();
            builder.RegisterInstance(mapper).As<IMapper>();
            SignalRHubManager.Mapper = mapper;
            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
            RegisterDomains(builder);

            var queryBuilder = new DataTablesLinqQueryBulider();
            builder.RegisterInstance(queryBuilder).As<DataTablesLinqQueryBulider>();
            #endregion
            #region add masstransit consumer
            builder.Register(context => RabbitMqConfig.InitReceiverBus<SignalRHandler>("Smartfleet.Web.endpoint"))
                .SingleInstance()
                .As<IBusControl>()
                .As<IBus>();
            #endregion

            #region register redis cache

            builder.Register(c => new RedisConnectionManager(ConfigurationManager.AppSettings["RedisUrl"], ConfigurationManager.AppSettings["redisPass"])).As<IRedisConnectionManager>();
            builder.RegisterType<RedisCache>().As<IRedisCache>();

            #endregion
            var container = builder.Build();
            container.Resolve<IBusControl>().StartAsync();
           
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
           
        }

        private static void RegisterDomains(ContainerBuilder builder)
        {
            var customerDomain = new CustomerDomainDependencyRegistrar();
            var mobileUnitDependencyRegistrar = new MobileUnitDependencyRegistrar();
            customerDomain.Register(builder);
            mobileUnitDependencyRegistrar.Register(builder);
        }
    }
}
