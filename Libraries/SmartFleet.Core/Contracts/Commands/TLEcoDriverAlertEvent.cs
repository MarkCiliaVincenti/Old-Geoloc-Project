using System;
using SmartFleet.Core.Domain.Vehicles;

namespace SmartFleet.Core.Contracts.Commands
{
    public class TLEcoDriverAlertEvent
    {
        public VehicleEvent VehicleEventType { get;  set; }
        public float? Latitude { get;  set; }
        public float? Longitude { get;  set; }
        public string Address { get;  set; }
        public DateTime EventUtc { get;  set; }
        public Guid VehicleId { get;  set; }
        public Guid? CustomerId { get; set; }
        public Guid Id { get; set; }
    }
}