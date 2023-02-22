using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using SmartFleet.Data;
using SmartFLEET.Web.Controllers;

namespace SmartFLEET.Web.Areas.Administrator.Controllers
{
   public class DriverController : BaseController
    {
        // GET: Administrator/Driver
        public ActionResult Index()
        {
            return View();
        }

        public DriverController(SmartFleetObjectContext objectContext) : base(objectContext)
        {
        }

        public DriverController(SmartFleetObjectContext objectContext, IMapper mapper) : base(objectContext, mapper)
        {
        }
    }
}