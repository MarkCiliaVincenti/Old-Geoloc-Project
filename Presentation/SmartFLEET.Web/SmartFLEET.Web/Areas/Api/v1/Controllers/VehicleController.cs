using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using SmartFleet.Core.Domain.Vehicles;
using SmartFleet.Data;
using SmartFLEET.Web.Areas.Api.Controllers;

namespace SmartFLEET.Web.Areas.Api.v1.Controllers
{
    [RoutePrefix("api/fleet/v1/Vehicle")]
    public class VehicleController : BaseApiController
    {
        SmartFleetObjectContext _objectContext = new SmartFleetObjectContext();
        public VehicleController()
        {
            
        }
        /// <summary>
        /// Get collection of vehicles.
        /// </summary>
        [HttpGet]
        [Route("vehicles")]
        [Description("get list of vehicles")]
        [Authorize]
        
        public async Task<IEnumerable<Vehicle>> GetAllVehicles() => await _objectContext.Vehicles.ToListAsync();
        /// <summary>
        /// Get a vehicle by VIN.
        /// </summary>
        [HttpGet]
        [Route("getVehicle")]
        [Description("get a vhicle by VIN")]
        [Authorize]
        public async Task<Vehicle> GetVehicle(string vin)
        {
            return await _objectContext.Vehicles.FirstOrDefaultAsync(x => x.Vin == vin);
        }
        public VehicleController(SmartFleetObjectContext objectContext, IMapper mapper) : base(objectContext, mapper)
        {
        }
    }
}
