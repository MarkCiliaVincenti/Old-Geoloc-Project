using System;

namespace SmartFleet.Core.Contracts.Commands
{
    public class TlIdentifierEvent: BaseEntity
    {
        public string IdentifierNumber { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? DriverId { get; set; }
        
    }
}