using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SmartFleet.Core.Domain.Vehicles;

namespace SmartFleet.Customer.Domain.Common.Dtos
{
    [Serializable]
    public class VehicleDto
    {
        public VehicleDto()
        {
            
        }
        public VehicleDto(string vehicleName , Guid id, string customerId, VehicleType type)
        {
            VehicleName = vehicleName;
            Id = id;
            CustomerId = customerId;
            VehicleType = type;
        }

        public Guid MobileUnitId { get; set; }
        public Guid Id { get; set; }
        public string VehicleName { get; set; }
        public string LicensePlate { get; set; }
        public string Vin { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Customer { get; set; }
        public string Imei { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public string VehicleStatus { get; set; }
        public VehicleType VehicleType { get; set; }
        public string CreationDate { get; set; }
        public string InitServiceDate { get; set; }
        public string CustomerId { get; set; }
    }
}
