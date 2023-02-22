using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SmartFLEET.Web.Controllers
{
    public class LanguageController : Controller
    {
        // GET: Language
        /// <summary>
        /// 
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public ActionResult ChangeLanguage(string language)
        {
            if (language != null)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(language);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            }

            HttpCookie cookie = new HttpCookie("culture")
            {
                Value = language
            };

            Response.Cookies.Add(cookie);

            return RedirectToAction("Index", "Home", new {culture = language});
        }
    }
}