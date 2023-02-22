using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartFleet.Service.Tracking;

namespace Domains.TestIntegration.Queries
{ 
    [TestClass]
    public class GetPositionsOfVehicleTest : TestBase
    {
        [TestMethod]
        public async Task Query_ShouldNotBeNull()
        {
            var servicePosition = new PositionService(null, Factory.Object);
            var query = await servicePosition.GetVehiclePositionsByPeriodAsync(
                Guid.Parse("7027121d-2214-41af-9264-4babd18c9880"), new DateTime(2019, 5, 7), new DateTime(2019, 6, 7));
            Assert.AreEqual(query.Count, 1833);
        }
    }
}
