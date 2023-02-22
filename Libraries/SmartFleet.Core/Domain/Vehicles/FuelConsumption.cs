using System;
using System.ComponentModel.DataAnnotations.Schema;
using SmartFleet.Core.Domain.Customers;

namespace SmartFleet.Core.Domain.Vehicles
{
    public class FuelConsumption: BaseEntity
    {
        [Index("IX_VehicleId")]
        public Guid VehicleId { get; set; }
        [Index("IX_CustomerId")]
        public Guid CustomerId { get; set; }
        public Int32 FuelLevel { get; set; }
        public Int32 FuelUsed { get; set; }
        public double Milestone { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public Customer Customer { get; set; }
        public Vehicle Vehicle { get; set; }
        public Int32 TotalFuelConsumed { get; set; }
    }
}
