using FluentValidation.Attributes;
using SmartFLEET.Web.Areas.Administrator.Validation;

namespace SmartFLEET.Web.Areas.Administrator.Models
{
   [Validator(typeof(UserVmValidator))]
    public class UserVm
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string TimeZoneInfo { get; set; }
        public string Role { get; set; }

    }
}