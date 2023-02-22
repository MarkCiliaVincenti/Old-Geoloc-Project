using System;
using System.Linq;
using Autofac;
using SmartFleet.Core.Data;

namespace SmartFleet.Core.Infrastructure.Registration
{
    public class Resolver
    {
        public static IRepository<T> RepositoryResolve<T>(IContainer container) where T :BaseEntity
        {
            return container.Resolve<IRepository<T>>();
        }
        
    }
}
