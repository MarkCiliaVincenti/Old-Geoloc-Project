using System;
using SmartFleet.Core.Domain.Customers;

namespace SmartFleet.Core.Domain.Vehicles
{
    public class VehicleAlert:BaseEntity
    {
        public  Guid CustomerId { get; set; }
        public Guid VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        public Customer Customer { get; set; }
        public DateTime EventUtc { get; set; }
        public VehicleEvent VehicleEvent { get; set; }
        public int? Speed { get; set; }
    }
}
