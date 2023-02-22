using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MediatR;
using SmartFleet.Core.Domain.Vehicles;
using SmartFleet.Customer.Domain.Queries.Brands;
using SmartFleet.Customer.Domain.Queries.Customers;
using SmartFleet.Customer.Domain.Queries.Models;
using SmartFleet.MobileUnit.Domain.MobileUnit.Queries;

namespace SmartFLEET.Web.Areas.Administrator.Models
{
    //[Validator(typeof(AddVehicleValidator))]
    public  class AddVehicleViewModel
    {
        private readonly IMediator _mediator;


        public AddVehicleViewModel(IMediator mediator)
        {
            _mediator = mediator;

            VehicleTypes = new List<KeyValuePair<short, string>>();
            foreach (var vehicleType in Enum.GetValues(typeof(VehicleType)) .Cast<VehicleType>())
            {
                VehicleTypes.Add(new KeyValuePair<short, string>((short) vehicleType, vehicleType.ToString()));
            }
        }

        public AddVehicleViewModel()
        {
            
        }
       
       // public Guid Id { get; set; }
        [Required]
        public string VehicleName { get; set; }
        public string LicensePlate { get; set; }
        public string Vin { get; set; }

        public Guid? BrandId { get; set; }
        [Required]
        public Guid? ModelId { get; set; }
        [Required]
        public Guid? CustomerId { get; set; }
        public VehicleStatus VehicleStatus { get; set; }
        [Required]
        public short VehicleType { get; set; }
        [Required]
        public Guid? BoxId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Brand> Brands => _mediator?
            .Send(new GetBrandsListQuery())
            .GetAwaiter()
            .GetResult();
        public List<Model> Models => _mediator?
            .Send(new GetModelsListQuery())
            .GetAwaiter()
            .GetResult();
        /// <summary>
        /// 
        /// </summary>
        public  List<CustomerItemViewModel> Customers => _mediator?
            .Send(new GetCustomersListQuery())
            .GetAwaiter()
            .GetResult()
            .Select(c=>new CustomerItemViewModel(){Id = c.Id, Name = c.Name}).ToList();
        public List<BoxItemModelView> Boxes => _mediator?.Send(new GetMobileUnitsWithoutVehicleIdQuery())
            .GetAwaiter()
            .GetResult()
            .Select(b=>new BoxItemModelView() {Id =b.Id, Imei = b.Imei}).ToList();
        public List<KeyValuePair<short, string>> VehicleTypes { get; set; }
    }
}