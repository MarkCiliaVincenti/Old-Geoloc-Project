using System.Linq;
using System.Reflection;
using StackExchange.Redis;

namespace SmartFleet.Core.Extension
{
    public static class ObjectExtension
    {
        public static HashEntry[] ToHashEntries(this object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            return properties
                .Where(x => x.GetValue(obj) != null) // <-- PREVENT NullReferenceException
                .Select(property => new HashEntry(property.Name, property.GetValue(obj)
                    .ToString())).ToArray();
        }
    }
}
