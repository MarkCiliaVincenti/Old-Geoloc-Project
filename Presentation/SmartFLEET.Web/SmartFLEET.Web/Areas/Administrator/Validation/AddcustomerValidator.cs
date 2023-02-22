using FluentValidation;
using SmartFleet.Service.Customers;
using SmartFLEET.Web.Areas.Administrator.Models;

namespace SmartFLEET.Web.Areas.Administrator.Validation
{
    public class AddcustomerValidator: AbstractValidator<AddCustomerViewModel>
    {
        public AddcustomerValidator(ICustomerService customerService)
        {
            RuleFor(vehicle => vehicle.Name).NotEmpty();
            RuleFor(vehicle => vehicle.Email).NotEmpty();
            //   RuleFor(vehicle => vehicle.Model).NotEmpty().WithMessage("Le modèle de véhicule est requis");
           // RuleFor(vehicle => vehicle.State).NotEmpty();
            RuleFor(vehicle => vehicle.Street).NotEmpty();
            RuleForEach(x => x.UserVms).SetValidator(new UserVmValidator(customerService));
        }
    }
}