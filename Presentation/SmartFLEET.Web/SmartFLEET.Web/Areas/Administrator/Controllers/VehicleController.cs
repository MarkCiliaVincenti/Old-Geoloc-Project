using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using SmartFleet.Customer.Domain.Commands.Vehicles;
using SmartFleet.Customer.Domain.Queries.Vehicles;
using SmartFleet.Service.Vehicles;
using SmartFleet.Web.Framework.DataTables;
using SmartFLEET.Web.Areas.Administrator.Models;
using SmartFLEET.Web.Areas.Administrator.Validation;
using SmartFLEET.Web.Controllers;

namespace SmartFLEET.Web.Areas.Administrator.Controllers
{
    public class VehicleController : BaseController
    {
        private readonly IVehicleService _vehicleService;
        private readonly DataTablesLinqQueryBulider _queryBuilder;
        public VehicleController(IMediator mediator, IMapper mapper, IVehicleService vehicleService, DataTablesLinqQueryBulider queryBuilder) : base(mediator, mapper)
        {
            _vehicleService = vehicleService;
            _queryBuilder = queryBuilder;
        }

        public ActionResult Index()
        {
            return PartialView("Index");
        }
        public ActionResult GetListForCustomer()
        {
            return PartialView("_List");
        }
        //[HttpGet]
        public async Task<JsonResult> GetAllVehicles()
        {
            var data = await SendAsync(new GetVehiclesListQuery
            {
                Request = Request
            }).ConfigureAwait(false);
            try
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        //[HttpGet]
        public async Task<JsonResult> GetAllVehiclesForCustomer  (string customerId)
        {
            var query = _queryBuilder.BuildQuery(Request , _vehicleService.GetVehiclesOfCustomer(Guid.Parse(customerId)));
            var jsResult = new
            {
                recordsTotal = query.recordsTotal,
                draw = query.draw,
                recordsFiltered = query.recordsFiltered,
                data = Mapper.Map<List<VehicleViewModel>>(query.data),
                lenght = query.length
            };
            return Json(jsResult, JsonRequestBehavior.AllowGet);
        }

       
        public ActionResult Detail()
        {
            //var id = Guid.Parse(vehicleId);
            //var vehicleviewModel = Mapper.Map<VehicleViewModel>(ObjectContext.Vehicles.Include("Customer").FirstOrDefault(x=>x.Id == id));
            return PartialView("Detail");
        }
        public ActionResult Create()
        {
            //var id = Guid.Parse(vehicleId);
            //var vehicleviewModel = Mapper.Map<VehicleViewModel>(ObjectContext.Vehicles.Include("Customer").FirstOrDefault(x=>x.Id == id));
            return PartialView("_Create");
        }

        public async Task<JsonResult> GetVehicleDetail(string vehicleId)
        {
            var id = Guid.Parse(vehicleId);
            return Json(Mapper.Map<VehicleViewModel>(await _vehicleService.GetVehicleByIdWithDetailAsync(id)),
                JsonRequestBehavior.AllowGet);
        }

        

        [HttpGet]
        public ActionResult GetNewVehicle()
        {
            return Json(new AddVehicleViewModel(Mediator), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> AddNewVehicle(AddVehicleViewModel model)
        {
            var validator = new AddVehicleValidator();
            var result = await validator.ValidateAsync(model).ConfigureAwait(false);
            if (result.IsValid)
            {
                try
                {
                    var vehicle = Mapper.Map<CreateVehicleCommand>(model);
                    await SendAsync(vehicle).ConfigureAwait(false);
                    return Json( new {errors= new string[]{""}} , JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    Json(e.Message);
                }
            }
            var validationModel = ValidationViewModel(result);
            return Json(new { errors= validationModel, vehicle = model}, JsonRequestBehavior.AllowGet);
        }
    }
}