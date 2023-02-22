using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SmartFleet.Core.Domain.Gpsdevices;

namespace SmartFleet.MobileUnit.Domain.MobileUnit.Dtos
{
    public class MobileUnitDto
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; }

        public string Imei { get; set; }
        public Guid? VehicleId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public BoxStatus BoxStatus { get; set; }
        public DateTime? CreationDate { get; set; }
        public string IccId { get; set; }

        public string PhoneNumber { get; set; }
        public DateTime? LastGpsInfoTime { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceType Type { get; set; }
        public string Brand { get; set; }
        public string VehicleName { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
    }
}
