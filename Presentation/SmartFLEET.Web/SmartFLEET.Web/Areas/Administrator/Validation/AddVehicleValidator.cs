using System;
using FluentValidation;
using SmartFLEET.Web.Areas.Administrator.Models;

namespace SmartFLEET.Web.Areas.Administrator.Validation
{
    public class AddVehicleValidator: AbstractValidator<AddVehicleViewModel>
    {
        public AddVehicleValidator()
        {
            RuleFor(vehicle => vehicle.VehicleName).NotEmpty().WithMessage("Le nom du véhicule est requis");
            RuleFor(vehicle => vehicle.ModelId).NotEmpty().Must(x=>x!=Guid.Empty).WithMessage("Le champs marque est requis"); ;
            //   RuleFor(vehicle => vehicle.Model).NotEmpty().WithMessage("Le modèle de véhicule est requis");
            RuleFor(vehicle => vehicle.CustomerId).NotEmpty();
            RuleFor(po => po.Vin)
                .Matches("^[A-Za-z0-9]{17}$")
                .When(x => !string.IsNullOrEmpty(x.Vin) || !string.IsNullOrWhiteSpace(x.Vin));
        }
    }
}