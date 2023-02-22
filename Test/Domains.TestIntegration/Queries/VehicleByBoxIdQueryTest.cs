using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartFleet.Core.Data;
using SmartFleet.Core.Data.Enums;
using SmartFleet.Customer.Domain.Queries.Vehicles;
using SmartFleet.Data.Dbcontextccope.Implementations;

namespace Domains.TestIntegration.Queries
{
    [TestClass]
    public class VehicleByBoxIdQueryTest
    {
        [TestMethod]
        public  async Task Query_ShouldNotBeNull()
        {
            var factory = new Mock<IDbContextScopeFactory>();
            IDbContextScope dbContextScope = new DbContextScope();
            factory.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(dbContextScope);
            var  mapper = new Mock<IMapper>();
            var handler = new VehiclesQueriesHandler(factory.Object, mapper.Object , null);
            var command = new GetVehicleByMobileUnitImeiQuery { Imei = ("286d624c-08c3-46fb-9674-024c1511269d") };
            var actualResult = await handler.Handle(command, CancellationToken.None);
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(actualResult.Id,Guid.Parse("922a7faa-0dfe-49e6-9f78-124ed6e5953a"));
        }
    }
}
