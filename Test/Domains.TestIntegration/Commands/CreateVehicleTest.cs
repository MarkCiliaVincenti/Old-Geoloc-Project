using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartFleet.Core.Domain.Vehicles;
using SmartFleet.Customer.Domain.Commands.Vehicles;

namespace Domains.TestIntegration.Commands
{
    [TestClass]
    public class CreateVehicleTest : TestBase
    {
        [TestMethod]
        public async Task CreateVehicleCommand_ShouldBeSaved()
        {
            
           //var handler = new VehiclesCommandsHandler(Factory.Object, Mapper);
           // var command = new CreateVehicleCommand
           // {
           //     VehicleName = "truck test",
           //     VehicleStatus = VehicleStatus.Pending,
           //     VehicleType = (short) VehicleType.Truck,
           //     CustomerId = Guid.Parse("4d34e52a-5709-4c73-bf0a-e6069192a5b8"),
           //     BoxId = Guid.Parse("d100c331-fc30-44e9-85fa-fd42c39ce415"),
           //     ModelId = Guid.Parse("33650973-a5f0-4504-ace1-ffbbad6855a7"),
           //     Vin = "XLRTE47MS0E958242",
           //     LicensePlate = "674-CDD"

           // };
           // var actualResult = await handler.Handle(command, CancellationToken.None);
           // Assert.IsNull(actualResult);

        }
    }
}
