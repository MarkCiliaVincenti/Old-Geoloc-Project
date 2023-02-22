using System;
using System.Linq;
using System.Web.Mvc;
using SmartFleet.Data;
using SmartFLEET.Web.Areas.Administrator.Models;
using SmartFLEET.Web.Controllers;
using WebGrease.Css.Extensions;

namespace SmartFLEET.Web.Areas.Administrator.Controllers
{
    [Authorize(Roles = "admin,customer")]
    public class UserController : BaseController
    {
       
        public UserController(SmartFleetObjectContext objectContext ) :base(objectContext)
        {
           
        }
        // GET
        public ActionResult Index()
        {
            if (User.IsInRole("customer"))
            {
                var currentUser = ObjectContext.UserAccounts.FirstOrDefault(x=>x.UserName == User.Identity.Name);
                if (currentUser!=null)
                    return View(ObjectContext.UserAccounts.Include("Customer").Where(u=>u.CustomerId == currentUser.CustomerId).ToList().Select(x => new UserViewModel(x)));

            }

            return View(ObjectContext.UserAccounts.Include("Customer").ToList().Select(x=>new UserViewModel(x)));
        }

        public ActionResult Users()
        {
            return PartialView("_Users");
        }
        public ActionResult NewUser()
        {
            return PartialView("_NewUser");
        }

        public JsonResult GetTimeZones()
        {
            return Json(TimeZoneInfo.GetSystemTimeZones(), JsonRequestBehavior.AllowGet);
        }
    }
}