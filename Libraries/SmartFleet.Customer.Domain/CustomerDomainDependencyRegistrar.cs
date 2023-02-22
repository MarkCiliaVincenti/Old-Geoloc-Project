using System.Reflection;
using Autofac;
using SmartFleet.Core.Infrastructure.Registration;

namespace SmartFleet.Customer.Domain
{
    public class CustomerDomainDependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder)
        {
            
            var allHandlers = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(allHandlers)
                .Where(t => t.Name.EndsWith("Handler"))
                .AsImplementedInterfaces();

           
        }
    }
}
