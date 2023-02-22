using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SmartFleet.Core.Data;
using SmartFleet.Core.Domain.Gpsdevices;
using SmartFleet.Core.Domain.Vehicles;
using SmartFleet.Data;

namespace SmartFleet.Service.Vehicles
{
    public class VehicleService : IVehicleService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private SmartFleetObjectContext _db;

        public VehicleService(IDbContextScopeFactory dbContextScopeFactory)
        {
             _dbContextScopeFactory = dbContextScopeFactory;

        }

        public async Task<bool> AddNewVehicleAsync(Vehicle vehicle)
        {
            try
            {
                using (var contextFScope = _dbContextScopeFactory.Create())
                {
                    _db = contextFScope.DbContexts.Get<SmartFleetObjectContext>();

                    vehicle.MileStoneUpdateUtc = DateTime.Now;
                   if(vehicle.Box_Id.HasValue)
                       vehicle.VehicleStatus = VehicleStatus.Active;
                    var boxId = vehicle.Box_Id;
                    var box = await _db.Boxes.FirstOrDefaultAsync(b => b.Id == boxId).ConfigureAwait(false);

                    if (box != null)
                    {
                        box.VehicleId = vehicle.Id;
                        box.BoxStatus = BoxStatus.Valid;
                        _db.Entry(box).State = EntityState.Modified; 

                    }
                    _db.Vehicles.Add(vehicle);
                    await contextFScope.SaveChangesAsync().ConfigureAwait(false);

                    return true;
                }

               
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<Vehicle[]> GetVehiclesFromCustomerAsync(Guid customerId)
        {
            using (var contextFScope = _dbContextScopeFactory.Create())
            {
                _db = contextFScope.DbContexts.Get<SmartFleetObjectContext>();
                return await _db.Vehicles.Where(x => x.CustomerId == customerId).ToArrayAsync().ConfigureAwait(false);
            }
        }
        public async Task<Vehicle> GetVehicleByIdAsync(Guid id)
        {
            using (var contextFScope = _dbContextScopeFactory.Create())
            {
                _db = contextFScope.DbContexts.Get<SmartFleetObjectContext>();
                return await _db.Vehicles.FindAsync(id).ConfigureAwait(false);
            }
        }
        public async Task<Vehicle> GetVehicleByIdWithDetailAsync(Guid id)
        {
            using (var contextFScope = _dbContextScopeFactory.Create())
            {
                _db = contextFScope.DbContexts.Get<SmartFleetObjectContext>();
                return await _db.Vehicles.Include(x => x.Brand).Include(x => x.Customer).Include(x => x.Model)
                    .Include(x => x.Boxes).FirstOrDefaultAsync(v => v.Id == id).ConfigureAwait(false);
            }
        }
        public async Task<List<Vehicle>> GetAllVehiclesQueryAsync()
        {
            using (var contextFScope = _dbContextScopeFactory.Create())
            {
                _db = contextFScope.DbContexts.Get<SmartFleetObjectContext>();
                return await _db.Vehicles
                    .Include("Brand")
                    .Include("Model")
                    .Include("Customer")
                    .ToListAsync()
                    .ConfigureAwait(false);
            }
        }

        public async Task<List<Vehicle>> GetAllVehiclesOfCustomerAsync(Guid customerId)
        {
            using (var contextFScope = _dbContextScopeFactory.Create())
            {
                _db = contextFScope.DbContexts.Get<SmartFleetObjectContext>();
                return await _db.Vehicles
                    .Where(v=>v.CustomerId == customerId)
                    .ToListAsync().ConfigureAwait(false);
            }
        }

        public IQueryable<Vehicle> GetVehiclesOfCustomer(Guid customerId)
        {
            var contextFScope = _dbContextScopeFactory.Create();
            
                _db = contextFScope.DbContexts.Get<SmartFleetObjectContext>();
                return _db.Vehicles.Where(v => v.CustomerId == customerId)
                    .Include("Brand")
                    .Include("Model");
        }
        public IQueryable<Vehicle> GetAllVehicles()
        {
            var contextFScope = _dbContextScopeFactory.Create();

            _db = contextFScope.DbContexts.Get<SmartFleetObjectContext>();
            return _db.Vehicles
                .Include("Brand")
                .Include("Model");
        }

        public double GetFuelConsumptionByPeriod(DateTime start, DateTime end,  Guid vehicleId, double totalDistance)
        {
            var contextFScope = _dbContextScopeFactory.Create();
            _db = contextFScope.DbContexts.Get<SmartFleetObjectContext>();
            var l100Fuel = default(double);
            var fuelRecords = FuelConsumptions(start, end, vehicleId);

            if (fuelRecords.Count <= 0) return l100Fuel;
            var nextRecord = GetNextFuelConsumption(end, vehicleId);
            if (nextRecord != null)
                fuelRecords.Add(nextRecord);

            //Gestion du nombre de litres consommés qui revient à 0 à chaque ignition OFF
            var vehicleFuelZeroDetected = false;
            var vehicleFuelConsumedReset = false;
            var previousValidFuelConsumed = default(Int32);
            var previousFuelConsumed = fuelRecords.First().FuelUsed;
            var fuel = 0;
            foreach (var record in fuelRecords)
            {
                //Si on détecté un retour à zéro et qu'une nouvelle valeure positif arrive
                if (vehicleFuelZeroDetected && record.FuelUsed > 0)
                {
                    //Détection d'un véhicule dont le nombre de litres de consommés revient à zéro à chaque ignition off
                    if (record.FuelUsed < previousValidFuelConsumed)
                        vehicleFuelConsumedReset = true;
                    //Sinon il ne s'agissait que d'un pic négatif à zéro mais le nombre de litres consommés évolue normalement
                    else
                        fuel -= previousValidFuelConsumed;
                    vehicleFuelZeroDetected = false;
                }
                //Si le nombre de litres consommés diminue
                else if (previousFuelConsumed - 5 > record.FuelUsed)
                {
                    vehicleFuelZeroDetected = true;
                    previousValidFuelConsumed = previousFuelConsumed;
                    fuel += previousFuelConsumed;
                }

                previousFuelConsumed = record.FuelUsed;
            }

            //S'il s'agit un véhicule dont le nombre total de litres consommés revient à zéro à chque ignition OFF
            if (vehicleFuelConsumedReset)
                fuel += fuelRecords.Last().FuelUsed - fuelRecords.First().FuelUsed;
            else
                fuel = fuelRecords.Last(p => p.FuelUsed > 0).FuelUsed -
                       fuelRecords.First(p => p.FuelUsed > 0).FuelUsed;
             
            // ReSharper disable once ComplexConditionExpression
            l100Fuel = Math.Round(fuel * 100 / totalDistance, 1);

            return l100Fuel;
        }

        private List<FuelConsumption> FuelConsumptions(DateTime start, DateTime end, Guid vehicleId)
        {
            var fuelRecords = _db.FuelConsumptions
                .Where(p => p.VehicleId == vehicleId &&
                            p.DateTimeUtc >= start &&
                            p.DateTimeUtc < end)
                .OrderBy(p => p.DateTimeUtc)
                .ToList();
            return fuelRecords;
        }

        private FuelConsumption GetNextFuelConsumption(DateTime end, Guid vehicleId)
        {
            var nextRecord = _db.FuelConsumptions
                .OrderBy(p => p.DateTimeUtc)
                .FirstOrDefault(p => p.VehicleId == vehicleId &&
                                     p.DateTimeUtc >= end);
            return nextRecord;
        }

        public IEnumerable<FuelConsumption> GetFuelConsumptionList(DateTime start, DateTime end, Guid vehicleId)
        {
            using (var contextFScope = _dbContextScopeFactory.Create())
            {
                _db = contextFScope.DbContexts.Get<SmartFleetObjectContext>();
                return FuelConsumptions(start, end, vehicleId);
            }
        }
    }
}
