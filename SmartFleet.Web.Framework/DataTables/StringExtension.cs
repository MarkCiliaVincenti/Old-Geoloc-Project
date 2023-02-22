using System;

namespace SmartFleet.Web.Framework.DataTables
{
    public static class StringExtension
    {
        public static T ToEnum<T>(this string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }
    }
}