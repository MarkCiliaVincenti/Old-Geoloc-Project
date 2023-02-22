using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SmartFLEET.Web.Areas.Administrator.Models
{
    public class VehicleViewModel
    {
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
        public string VehicleType { get; set; }
        public string CreationDate { get; set; }
        public string InitServiceDate { get; set; }

    }
}