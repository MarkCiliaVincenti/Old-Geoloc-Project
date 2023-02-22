using AutoMapper;
using Moq;
using SmartFleet.Core.Data;
using SmartFleet.Core.Data.Enums;
using SmartFleet.Customer.Domain.Common.DomainMapping;
using SmartFleet.Data.Dbcontextccope.Implementations;
using SmartFleet.MobileUnit.Domain.Common;

namespace Domains.TestIntegration
{
    public class TestBase
    {
        protected static IMapper Mapper;
        protected static Mock<IDbContextScopeFactory> Factory;
        public TestBase()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                // cfg.AddProfile(new SmartFleetAdminMappings());
                cfg.AddProfile(new CustomerDomainMapping());
                cfg.AddProfile(new MobileUnitDomainMapping());
            });
            Factory = new Mock<IDbContextScopeFactory>();
            Mapper = mapperConfiguration.CreateMapper();
            IDbContextScope dbContextScope = new DbContextScope();
            Factory.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(dbContextScope);
            //HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();
        }
    }
}
