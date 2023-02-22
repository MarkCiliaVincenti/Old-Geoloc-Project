using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using SmartFleet.Core.Domain.Customers;
using SmartFleet.Data;
using SmartFleet.Service.Customers;
using SmartFLEET.Web.Helpers;
using SmartFLEET.Web.Models;

namespace SmartFLEET.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class InterestAreaController : BaseController
    {
        private readonly ICustomerService _customerService;
        public InterestAreaController(ICustomerService customerService,  SmartFleetObjectContext objectContext,
            IMapper mapper) : base(objectContext, mapper)
        {
            _customerService = customerService;
        }
        // GET: InterestArea
        public ActionResult Index()
        {
            return PartialView("_List");
        }
        public ActionResult Create()
        {
            return PartialView("_Create");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> AddNewZone(InterestAreaVm model)
        {
            if (ModelState.IsValid)
            {
                var customer = await _customerService.GetCustomerByNameAsync(User.Identity.Name).ConfigureAwait(false);
                if (customer != null)
                {
                    var area = Mapper.Map<InterestArea>(model);
                    area.Id = Guid.NewGuid();
                    area.CustomerId = customer.Id;
                    var r = _customerService.AddArea(area);
                    return Json(r, JsonRequestBehavior.AllowGet);
                }
            }

            var validation = ValidationViewModel();
            return Json(validation, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetZones()
        {
            // ReSharper disable once TooManyChainedReferences
            var q = RequestHelper.GetDataGridParams(Request);

            return Json(await _customerService.GetAllAreasAsync(User.Identity.Name, q.Item1, q.Item2).ConfigureAwait(false), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> GetAllZones()
        {
            return Json(await _customerService.GetAllAreasAsync(User.Identity.Name).ConfigureAwait(false), JsonRequestBehavior.AllowGet);
        }

    }
}