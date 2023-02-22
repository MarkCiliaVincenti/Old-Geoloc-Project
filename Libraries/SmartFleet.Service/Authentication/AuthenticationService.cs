using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using SmartFleet.Core.Domain.Users;

namespace SmartFleet.Service.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<IdentityUser> _userManager;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        public AuthenticationService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<string> GetRoleByUserId(string userId)
        {
            return  _userManager.GetRoles(userId);
        }

        public IAuthenticationManager AuthenticationManager { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="remember"></param>
        /// <returns></returns>
        public async Task<IdentityUser>  AuthenticationAsync(string userName, string password, bool remember)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user == null) return null;
            if (_userManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, password) !=
                PasswordVerificationResult.Success) return null;
            Authenticate(user, remember);
            return user;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="remember"></param>
        private void Authenticate(IdentityUser result, bool remember)
        {
            if (result == null) return;
            var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, result.UserName),
                    new Claim(ClaimTypes.NameIdentifier, result.Id), 
                },
                DefaultAuthenticationTypes.ApplicationCookie,
                ClaimTypes.Name, ClaimTypes.Role);
            var list = _userManager.GetRoles(result.Id);

            // if you want roles, just add as many as you want here (for loop maybe?)
            foreach (var userRole in list)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, userRole.ToLower()));
            }
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = remember }, identity);
        }

        public Task<IdentityUser> GetUserByNameAsync(string name)
        {
            return _userManager.FindByNameAsync(name);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Logout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }
    }
}
