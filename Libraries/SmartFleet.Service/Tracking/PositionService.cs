using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SmartFleet.Core.Data;
using SmartFleet.Core.Domain.Movement;
using SmartFleet.Core.Helpers;
using SmartFleet.Core.ReverseGeoCoding;
using SmartFleet.Data;
using SmartFleet.Service.Models;

namespace SmartFleet.Service.Tracking
{
    public class PositionService : IPositionService
    {
        private  SmartFleetObjectContext _objectContext;
        private readonly ReverseGeoCodingService _geoCodingService;
        private readonly IDbContextScopeFactory _dbContextScopeFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geoCodingService"></param>
        /// <param name="dbContextScopeFactory"></param>
        public PositionService(  ReverseGeoCodingService geoCodingService, IDbContextScopeFactory dbContextScopeFactory)
        {
            _geoCodingService = geoCodingService;
            _dbContextScopeFactory = dbContextScopeFactory;
        }

        public async Task<List<PositionViewModel>> GetLastVehiclePositionAsync(string userName)
        {
            using (var contextFScope = _dbContextScopeFactory.Create())
            {
                _objectContext = contextFScope.DbContexts.Get<SmartFleetObjectContext>();
                var positions = new List<PositionViewModel>();
                
                var query = await (from customer in _objectContext.Customers
                    join account in _objectContext.UserAccounts on customer.Id equals account.CustomerId
                    where account.UserName == userName
                    join vcl in _objectContext.Vehicles on customer.Id equals vcl.CustomerId into vehicleJoin
                    from vehicle in vehicleJoin
                    join box in _objectContext.Boxes on vehicle.Id equals box.VehicleId into boxesJoin
                    from box in boxesJoin
                    join position in _objectContext.Positions on box.Id equals position.Box_Id  
                    group position by position.Box_Id into g
                    select new {boxId = g.Key, Position = g.OrderByDescending(x=>x.Timestamp).Select(x=> new PositionViewModel {Address = x.Address, Latitude = x.Lat, Longitude = x.Long}).FirstOrDefault() } ).ToArrayAsync().ConfigureAwait(false);
                foreach (var item in query)
                {
                   
                    var vehicle = await _objectContext.Boxes.Include(x=>x.Vehicle).Where(x => x.Id == item.boxId).Select(x => new  {x.Vehicle.VehicleType, x.Vehicle.VehicleName, x.Vehicle.CustomerId, x.VehicleId}).FirstOrDefaultAsync().ConfigureAwait(false);
                    var pos = new PositionViewModel(vehicle?.VehicleName, vehicle?.CustomerId.ToString(), vehicle.VehicleType, vehicle?.VehicleId.ToString());
                    pos.Address = item.Position.Address;
                    pos.Latitude = item.Position.Latitude;
                    pos.Longitude = item.Position.Longitude;
                    positions.Add(pos);
                }
                return positions;
            }
        }

        public async Task<List<Position>> GetVehiclePositionsByPeriodAsync(Guid vehicleId, DateTime startPeriod,DateTime endPeriod , string timeZoneInfo  = null)
        {
            using (var contextFScope = _dbContextScopeFactory.Create())
            {
                _objectContext = contextFScope.DbContexts.Get<SmartFleetObjectContext>();
                
                var box = await _objectContext
                    .Boxes
                    .Select(x=>new {  x.Id, x.VehicleId })
                    .FirstOrDefaultAsync(v => v.VehicleId == vehicleId)
                    .ConfigureAwait(false);
                
                if (box == null) return new List<Position>();
                
                if (box.Id == Guid.Empty) return new List<Position>();
                
                var positions =await 
                    _objectContext
                        .Positions
                        .Where(p => p.Box_Id == box.Id && p.Timestamp >= startPeriod && p.Timestamp <= endPeriod)
                        .ToListAsync()
                        .ConfigureAwait(false);
               if(!string.IsNullOrEmpty(timeZoneInfo))
                   foreach (var position in positions)
                       position.Timestamp = position.Timestamp.ConvertToCurrentTimeZone(timeZoneInfo);

                return positions;
            }
        }
    }
}
