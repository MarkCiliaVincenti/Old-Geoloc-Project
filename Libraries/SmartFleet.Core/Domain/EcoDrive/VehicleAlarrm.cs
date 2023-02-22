using System;
using SmartFleet.Core.Domain.Vehicles;

namespace SmartFleet.Core.Domain.EcoDrive
{
    public class VehicleAlarrm: BaseEntity
    {
        public Guid? VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        public DateTime AlarmTime { get; set; }
        public AlarmType AlarmType { get; set; }
    }
}
