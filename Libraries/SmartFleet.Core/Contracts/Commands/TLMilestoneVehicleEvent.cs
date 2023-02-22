using System;

namespace SmartFleet.Core.Contracts.Commands
{
    public class TLMilestoneVehicleEvent
    {
        public double Milestone { get; set; }
        public DateTime EventUtc { get; set; }
        public Guid VehicleId { get; set; }

    }
}
