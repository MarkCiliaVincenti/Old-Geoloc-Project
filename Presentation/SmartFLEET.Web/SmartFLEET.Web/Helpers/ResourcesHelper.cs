using System.Collections;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using Newtonsoft.Json;

namespace SmartFLEET.Web.Helpers
{
    public class ResourcesHelper
    {
        public static string GenerateResxJSON<T>()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            ResourceManager rm = new ResourceManager(typeof(T));
            var entries = rm.GetResourceSet(currentCulture, true, true).OfType<DictionaryEntry>().ToDictionary(x => x.Key, y => y.Value);
            return JsonConvert.SerializeObject(entries);
        }
    }
}