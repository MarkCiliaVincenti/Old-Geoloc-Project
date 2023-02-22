using Autofac;

namespace SmartFleet.Core.Infrastructure.Registration
{
    public interface IDependencyRegistrar
    {
        void Register(ContainerBuilder builder);
    }
}