using System;
using Newtonsoft.Json;
using SmartFleet.Core.Domain.Gpsdevices;

namespace SmartFleet.Core.Contracts.Commands
{
    [Serializable]
    public class CreateBoxCommand
    {
        [JsonProperty(PropertyName = "id", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public Guid Id { get; set; }
        [JsonProperty(PropertyName = "imei", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Imei { get; set; }
        [JsonProperty(PropertyName = "utc", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]

        public DateTime? LastValidGpsDataUtc { get; set; }
        public string Address { get; set; }
        [JsonProperty(PropertyName = "status", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]

        public BoxStatus BoxStatus { get; set; }
        [JsonProperty(PropertyName = "cstid", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]

        public Guid? CustomerId { get; set; }
        [JsonProperty(PropertyName = "vclid", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]

        public Guid? VehicleId { get; set; }
        [JsonProperty(PropertyName = "srl", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]

        public string SerialNumber { get; set; }
        [JsonProperty(PropertyName = "iccid", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]

        public string Icci { get; set; }
        [JsonProperty(PropertyName = "lg", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]

        public double Longitude { get; set; }
        [JsonProperty(PropertyName = "lat", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]

        public dynamic Latitude { get; set; }
        [JsonProperty(PropertyName = "speed", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]

        public double Speed { get; set; }
        [JsonProperty(PropertyName = "phone", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]

        public string PhoneNumber { get; set; }
        [JsonProperty(PropertyName = "type", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]

        public string Type { get; set; }
    }
}
