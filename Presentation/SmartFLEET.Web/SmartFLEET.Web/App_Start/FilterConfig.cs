using System.Web.Mvc;
using SmartFLEET.Web.Helpers;

namespace SmartFLEET.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CultureFilter(defaultCulture: "en"));
            filters.Add(new HandleErrorAttribute());
        }
    }
}
