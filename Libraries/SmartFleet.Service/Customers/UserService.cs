using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SmartFleet.Core.Domain.Users;

namespace SmartFleet.Service.Customers
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Boolean> AddUser(User user , string password)
        {
            var passwordHash = new PasswordHasher();
            user.PasswordHash = passwordHash.HashPassword(password);
            await _userManager.CreateAsync(user);
            await _userManager.AddToRolesAsync(user.Id, "user");
            
            return true;

        }
        public async Task<Boolean> AddAdmin(User user, string password)
        {
            var passwordHash = new PasswordHasher();
            user.PasswordHash = passwordHash.HashPassword(password);
            await _userManager.CreateAsync(user);
            await _userManager.AddToRolesAsync(user.Id, "admin");

            return true;

        }
    }
}
