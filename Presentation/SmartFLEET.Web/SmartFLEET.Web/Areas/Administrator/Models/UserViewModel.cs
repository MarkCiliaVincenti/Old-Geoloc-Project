using System.ComponentModel.DataAnnotations;
using SmartFleet.Core.Domain.Users;

namespace SmartFLEET.Web.Areas.Administrator.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Identifiant")]
        public string UserName { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "N° tél")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Client")]
        public string Customer { get; set; }
        [Display(Name = "Fuseau horaire")]
        public string TimeZoneInfo { get; set; }
        [Display(Name = "Role")]
        public string Roles { get; set; }
        public UserViewModel(User user)
        {
            Id = user.Id;
            UserName = user.UserName;
            TimeZoneInfo = user.TimeZoneInfo;
            PhoneNumber = user.PhoneNumber;
            Customer = user.Customer?.Name;
            Email = user.Email;
        }

    }
}