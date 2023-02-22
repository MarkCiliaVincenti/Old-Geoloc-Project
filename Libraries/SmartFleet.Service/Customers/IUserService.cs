using System.Threading.Tasks;
using SmartFleet.Core.Domain.Users;

namespace SmartFleet.Service.Customers
{
    public interface IUserService
    {
        Task<bool> AddAdmin(User user, string password);
        Task<bool> AddUser(User user, string password);
    }
}