using System;
using MediatR;

namespace SmartFleet.MobileUnit.Domain.MobileUnit.Commands
{
    public class CreateMobileUnitCommand : IRequest
    {
        public CreateMobileUnitCommand()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Imei { get; set; }
        public string SerialNumber { get; set; }
        public string PhoneNumber  { get; set; }
        public string Brand { get; set; }
        public string ICCID { get; set; }
    }
}
