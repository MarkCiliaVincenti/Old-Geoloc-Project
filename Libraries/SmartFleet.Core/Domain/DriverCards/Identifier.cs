using System;
using SmartFleet.Core.Domain.Customers;
using SmartFleet.Core.Domain.Vehicles;

namespace SmartFleet.Core.Domain.DriverCards
{
    public class Identifier : BaseEntity
    {

        public Guid? DriverId { get; set; }
        public long CardNumber { get; set; }
        public Driver Driver { get; set; }
        public DateTime? CardIssueDate { get; set; }
        public DateTime? CardValidityBegin { get; set; }
        public DateTime? CardExpiryDate { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
