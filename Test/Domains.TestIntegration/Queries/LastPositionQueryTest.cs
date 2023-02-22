using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartFleet.Core.Data;
using SmartFleet.Core.Data.Enums;
using SmartFleet.Data.Dbcontextccope.Implementations;
using SmartFleet.MobileUnit.Domain.Movements.Handlers;
using SmartFleet.MobileUnit.Domain.Movements.Queries;

namespace Domains.TestIntegration.Queries
{
    [TestClass]
    public class LastPositionQueryTest
    {
        [TestMethod]
        public async Task Query_ShouldNotBeNull()
        {
            var factory = new Mock<IDbContextScopeFactory>();
            IDbContextScope dbContextScope = new DbContextScope();
            factory.Setup(x => x.Create(DbContextScopeOption.JoinExisting)).Returns(dbContextScope);
            var mapper = new Mock<IMapper>();
            var handler = new MovementsQueriesHandler(factory.Object, mapper.Object, null);
            var command = new GetLastPositionByMobileUnitIdQuery { MobileUnitId = Guid.Parse("286d624c-08c3-46fb-9674-024c1511269d") };
            var actualResult = await handler.Handle(command, CancellationToken.None);
            Assert.IsNotNull(actualResult);
            Assert.IsNotNull(actualResult.Latitude);
            Assert.IsNotNull(actualResult.Longitude);
        }
    }
}
