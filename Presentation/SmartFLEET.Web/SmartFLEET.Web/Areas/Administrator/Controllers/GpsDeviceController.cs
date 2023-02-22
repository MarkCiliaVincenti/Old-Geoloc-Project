using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SmartFleet.Data;
using SmartFleet.MobileUnit.Domain.MobileUnit.Commands;
using SmartFleet.MobileUnit.Domain.MobileUnit.Queries;
using SmartFLEET.Web.Controllers;

namespace SmartFLEET.Web.Areas.Administrator.Controllers
{
    public class GpsDeviceController : BaseController
    {
        // GET
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            return View(await SendAsync(new GetMobileUnitsListQuery()).ConfigureAwait(false));
        }

        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<ActionResult> AddMobileUnit(CreateMobileUnitCommand command)
        {
            try
            {
                await SendAsync(command).ConfigureAwait(false);
                return Json("ok");
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }
        public GpsDeviceController(SmartFleetObjectContext objectContext , IMediator mediator) : base(objectContext, mediator)
        {
        }
        
        
    }
}