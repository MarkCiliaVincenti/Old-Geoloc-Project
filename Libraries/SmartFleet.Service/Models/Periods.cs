using System;
using SmartFleet.Core.Domain.Movement;

namespace SmartFleet.Service.Models
{
    public class Periods
    {
        public Periods(DateTime? timestamp, DateTime? start, MotionStatus? currentStatus)
        {
            if (start != null)
                this.Start = start.Value;
            this.End = timestamp.Value;
            this.MotionStatus = currentStatus.Value;
            
        }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public MotionStatus MotionStatus { get; set; }
    }
}