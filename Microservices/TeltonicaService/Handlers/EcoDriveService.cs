using System;
using SmartFleet.Core.Contracts.Commands;
using SmartFleet.Core.Domain.Vehicles;

namespace TeltonicaService.Handlers
{
    public class EcoDriveService
    {
        private readonly CreateTeltonikaGps _data;

        public EcoDriveService(CreateTeltonikaGps data)
        {
            _data = data;
        }

        public TLFuelMilstoneEvent ProceedTNCANFilters()
        {
            var fuelLevel = default(UInt32?);
            var milestone = default(UInt32?);
            var fuelUsed = default(UInt32?);
            if (_data.AllIoElements != null &&
                _data.AllIoElements.ContainsKey(TNIoProperty.High_resolution_total_vehicle_distance_X))
                milestone = Convert.ToUInt32(_data.AllIoElements[TNIoProperty.High_resolution_total_vehicle_distance_X]);

            if (_data.AllIoElements != null && _data.AllIoElements.ContainsKey(TNIoProperty.Engine_total_fuel_used))
                fuelUsed = Convert.ToUInt32(_data.AllIoElements[TNIoProperty.Engine_total_fuel_used]);

            if (_data.AllIoElements != null && _data.AllIoElements.ContainsKey(TNIoProperty.Fuel_level_1_X))
                fuelLevel = Convert.ToUInt32(_data.AllIoElements[TNIoProperty.Fuel_level_1_X]);
            // ReSharper disable once ComplexConditionExpression
            if (fuelLevel != default(UInt32) && fuelLevel > 0 && fuelUsed > 0)
                return new TLFuelMilstoneEvent
                {
                    FuelConsumption = Convert.ToInt32(fuelUsed),
                    Milestone = Convert.ToInt32(milestone),
                    DateTimeUtc = _data.DateTimeUtc,
                    FuelLevel = Convert.ToInt32(fuelLevel)
                };
            return null;
        }

        public TLExcessSpeedEvent ProceedTLSpeedingAlert( Guid vehicleId, Guid? customerId)
        {
            return new TLExcessSpeedEvent
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                VehicleId = vehicleId,
                VehicleEventType = VehicleEvent.EXCESS_SPEED,
                EventUtc = _data.DateTimeUtc,
                Latitude = (float?)_data.Lat,
                Longitude = (float?)_data.Long,
                Address = _data.Address,
                Speed = _data.Speed
            };


        }


        public TLEcoDriverAlertEvent ProceedEcoDriverEvents( Guid vehicleId, Guid? customerId)
        {
            var @event = default(TLEcoDriverAlertEvent);
            if (_data.DataEventIO == (int)TNIoProperty.Engine_speed_X)
                @event = new TLEcoDriverAlertEvent
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    VehicleId = vehicleId,
                    VehicleEventType = VehicleEvent.EXCESS_ENGINE_SPEED,
                    EventUtc = _data.DateTimeUtc,
                    Latitude = (float?)_data.Lat,
                    Longitude = (float?)_data.Long,
                    Address = _data.Address
                    //Speed = _data.Speed
                };
            else if (_data.AllIoElements.ContainsKey(TNIoProperty.ECO_driving_type))
            {
                switch (Convert.ToByte(_data.AllIoElements[TNIoProperty.ECO_driving_type]))
                {
                    case 1:
                        if (Convert.ToByte(_data.AllIoElements[TNIoProperty.ECO_driving_value]) > 31)
                            @event = new TLEcoDriverAlertEvent
                            {
                                Id = Guid.NewGuid(),
                                CustomerId = customerId,
                                VehicleId = vehicleId,
                                VehicleEventType = VehicleEvent.EXCESS_ACCELERATION,
                                EventUtc = _data.DateTimeUtc,
                                Latitude = (float?)_data.Lat,
                                Longitude = (float?)_data.Long,
                                Address = _data.Address
                                //Speed = _data.Speed
                            };
                        break;
                    case 2:
                        if (Convert.ToByte(_data.AllIoElements[TNIoProperty.ECO_driving_value]) > 38)
                            @event = new TLEcoDriverAlertEvent
                            {
                                Id = Guid.NewGuid(),
                                CustomerId = customerId,
                                VehicleId = vehicleId,
                                VehicleEventType = VehicleEvent.SUDDEN_BRAKING,
                                EventUtc = _data.DateTimeUtc,
                                Latitude = (float?)_data.Lat,
                                Longitude = (float?)_data.Long,
                                Address = _data.Address
                                //Speed = _data.Speed
                            };
                        break;
                    case 3:
                        if (Convert.ToByte(_data.AllIoElements[TNIoProperty.ECO_driving_value]) > 45)
                            @event = new TLEcoDriverAlertEvent
                            {
                                Id = Guid.NewGuid(),
                                CustomerId = customerId,
                                VehicleId = vehicleId,
                                VehicleEventType = VehicleEvent.FAST_CORNER,
                                EventUtc = _data.DateTimeUtc,
                                Latitude = (float?)_data.Lat,
                                Longitude = (float?)_data.Long,
                                Address = _data.Address
                                //Speed = _data.Speed
                            };
                        break;
                    default:
                        break;

                }
            }
            return @event;
        }

    }
}
