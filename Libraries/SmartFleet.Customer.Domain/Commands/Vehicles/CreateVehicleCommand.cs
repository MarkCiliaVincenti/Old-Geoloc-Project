using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using MediatR;
using SmartFleet.Core.Domain.Vehicles;

namespace SmartFleet.Customer.Domain.Commands.Vehicles
{
    public class CreateVehicleCommand :  IRequest<ValidationResult>
    {
        

        public CreateVehicleCommand()
        {
            CreationDate = DateTime.Now;
            CmdId = Guid.NewGuid();
        }
        public string VehicleName { get; set; }
        public string LicensePlate { get; set; }
        public string Vin { get; set; }
        public Guid? BrandId { get; set; }
        public Guid? ModelId { get; set; }
        public Guid CustomerId { get; set; }
        public VehicleStatus VehicleStatus { get; set; }
        public short VehicleType { get; set; }
        public Guid? BoxId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime MileStoneUpdateUtc { get; set; }
        public Guid CmdId { get; set; }
        public bool CANEnabled { get; set; }
    }

    public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
    {
        public CreateVehicleCommandValidator()
        {
            RuleFor(v => v.VehicleName)
                .NotEmpty();
            RuleFor(v => v.Vin)
                .NotEmpty();
            RuleFor(v => v.BrandId)
                .NotEmpty().Must(v => v != Guid.Empty);
            RuleFor(v => v.BoxId)
                .NotEmpty().Must(v => v != Guid.Empty);
            RuleFor(v => v.CustomerId)
                .NotEmpty()
                .Must(v=>v!= Guid.Empty);
        }
    }
}
