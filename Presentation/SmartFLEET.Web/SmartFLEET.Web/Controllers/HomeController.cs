using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using SmartFleet.Core.Domain.Vehicles;
using SmartFleet.Service.Customers;
using SmartFleet.Service.Tracking;
using SmartFleet.Service.Vehicles;
using SmartFLEET.Web.Models;
using SmartFleet.Resources;
namespace SmartFLEET.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize(Roles = "user,customer")]
    public class HomeController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        public static string CurrentGroup { get; set; }
        private readonly IPositionService _positionService;
        private readonly ICustomerService _customerService;
        private readonly IVehicleService _vehicleService;
        /// <inheritdoc />
        // ReSharper disable once TooManyDependencies
        public HomeController(IMapper mapper,
            IPositionService positionService, 
            ICustomerService customerService , 
            IVehicleService vehicleService) : base( mapper)
        {
            _positionService = positionService;
            _customerService = customerService;
            _vehicleService = vehicleService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var user = User.Identity;
            var cst = await _customerService.GetCustomerByNameAsync(user.Name).ConfigureAwait(false);
            CurrentGroup = cst?.Id.ToString();
            ViewBag.GroupName = CurrentGroup;
            return View();
        }

      
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> LoadNodes()
        {
            var user = User.Identity;
            var cst = await _customerService.GetCustomerWithZonesAndVehiclesAsync(user.Name).ConfigureAwait(false);
            // add root nodes
            var nodes = AddRootNodes();
            if (cst == null)
                return Json(nodes, JsonRequestBehavior.AllowGet);
            // add customer zones to root node
            nodes.AddRange(AddAreaNodes(cst.Areas.Select(x => x.Name).ToArray()));

            foreach (var area in cst.Areas)
                nodes.AddRange(cst.Vehicles.Where(x => x.InterestAreaId == area.Id).Select(x => AddVehicleNodes(x, area.Name)));
            // add not assigned vehicles
            nodes.AddRange(cst.Vehicles.Where(v=> cst.Areas.All(a => a.Id != v.InterestAreaId)).Select(x=>AddVehicleNodes(x, "not-assigned")));
            return Json(nodes, JsonRequestBehavior.AllowGet);
        }
       
        [NonAction]
        private static JsTreeModel AddVehicleNodes(Vehicle vehicle, string area)
        {
            var node = new JsTreeModel();
            node.id = vehicle.Id.ToString();
            // ReSharper disable once ComplexConditionExpression
            node.text = vehicle.VehicleName == string.Empty ?
                vehicle.LicensePlate :
                " " + vehicle.VehicleName;
            node.parent = area +"-"+ Guid.Empty;
            node.icon = "la la-car ";
            return node;
        }
        [NonAction]
        private static List<JsTreeModel> AddAreaNodes(string [] areas)
        {
            // ReSharper disable once ComplexConditionExpression
            var nodes = areas.Select(area => new JsTreeModel
                {
                    id = area + "-" + Guid.Empty,
                    parent = "vehicles",
                    text = area,
                }) .ToList();
            nodes.Add(new JsTreeModel
            {
                id = "not-assigned-" + Guid.Empty,
                parent = "vehicles",
                text = Resource.NotAssigned,
            });
           
            return nodes;
        }

        private static List<JsTreeModel> AddRootNodes()
        {
            var nodes = new List<JsTreeModel>();
           
            nodes.Add(new JsTreeModel
            {
                id = "vehicles",
                parent = "#",
                text = Resource.Vehicles,
            });
            nodes.Add(new JsTreeModel
            {
                id = "drivers" ,
                parent = "#",
                text = Resource.Drivers,
            });
            return nodes;
        }

    }
}