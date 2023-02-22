using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SmartFleet.Service.Authentication;
using SmartFLEET.Web.Models.Account;

namespace SmartFLEET.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authenticationService"></param>
        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
           
        }

        private void GetCurrentAuthService()
        {
            _authenticationService.AuthenticationManager = _authenticationService.AuthenticationManager?? HttpContext.GetOwinContext().Authentication;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
               return View(new LoginModel());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginModel model)
        { 
            if (!ModelState.IsValid) return View(model);
            GetCurrentAuthService();
            var userExists = await _authenticationService.AuthenticationAsync(model.UserName, model.Password, model.RememberMe).ConfigureAwait(false);
            if (userExists == null) return View();
            return _authenticationService.GetRoleByUserId(userExists.Id).Any(identityUserRole => identityUserRole.Equals("customer") || identityUserRole.Equals("user")) ?
                RedirectToAction("Index", "Home") :
                RedirectToAction("Index", "Admin", new {area = "Administrator"});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Logout()
        {
            GetCurrentAuthService();

            _authenticationService.Logout();
            return RedirectToAction("Login", "Account", new { area = "" });
        }
    }
}