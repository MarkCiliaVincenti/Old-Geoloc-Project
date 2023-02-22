using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using SmartFleet.Core.Domain.Users;

namespace SmartFleet.Service.Authentication
{
    public interface IAuthenticationService
    {
        Task<IdentityUser> AuthenticationAsync(string userName, string password, bool remember);
        IEnumerable<string> GetRoleByUserId(string userId);
        IAuthenticationManager AuthenticationManager {  get; set; }
        Task<IdentityUser> GetUserByNameAsync(string name);
        void Logout();
    }
}