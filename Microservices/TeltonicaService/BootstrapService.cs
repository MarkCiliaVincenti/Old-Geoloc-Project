using System;
using Autofac;
using MassTransit;
using SmartFleet.Core;
using TeltonicaService.Infrastructure;

namespace TeltonicaService
{
    public class BootstrapService :IMicroService
    {
        

        public void StartService()
        {
            ContainerBuilder builder = new ContainerBuilder();
            var dependencyRegistrar = new DependencyRegistrar();
            dependencyRegistrar.Register(builder);

            try
            {
                DependencyRegistrar.ResolveServiceBus().Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //  throw;
            }
        }
    }
}
