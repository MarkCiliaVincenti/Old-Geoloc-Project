using System.ComponentModel.DataAnnotations;

namespace SmartFLEET.Web.Models.Account
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Identifiant *")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password *")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Se souvenir de moi")]
        public bool RememberMe { get; set; }
    }
}