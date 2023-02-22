using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SmartFleet.Core.Domain.Customers;
using SmartFleet.Core.Domain.Users;
using SmartFleet.Core.Domain.Vehicles;

namespace SmartFLEET.Web.Areas.Administrator.Models
{
    public class CustomerVm
    {
       
        public Guid Id { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public string CustomerStatus { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public DateTime CreationDate { get; set; }
        public string Tel { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}