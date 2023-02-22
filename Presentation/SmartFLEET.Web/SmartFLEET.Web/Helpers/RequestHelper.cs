using System;
using System.Text.RegularExpressions;
using System.Web;

namespace SmartFLEET.Web.Helpers
{
    public static class RequestHelper
    {
        public static Tuple<int, int> GetDataGridParams(HttpRequestBase request)
        {
            var r = request.Form.ToString().Split('&');
            var page = Convert.ToInt32(Regex.Match(r[0], @"\d+").Value);
            var rows = Convert.ToInt32(Regex.Match(r[1], @"\d+").Value);
            return new Tuple<int, int>( page, rows);
        }
    }
}