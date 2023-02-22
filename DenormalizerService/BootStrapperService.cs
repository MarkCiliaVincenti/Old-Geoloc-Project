using Autofac;
using DenormalizerService.Infrastructure;
using SmartFleet.Core;

namespace DenormalizerService
{
    public class BootStrapperService :IMicroService
    {
      
        public void StartService()
        {
            ContainerBuilder builder = new ContainerBuilder();
            var dependencyRegistrar = new DependencyRegistrar();
            dependencyRegistrar.Register(builder);
            DependencyRegistrar.ResolveServiceBus()
                .StartAsync()
                .GetAwaiter()
                .GetResult();
            
        }
    }
}