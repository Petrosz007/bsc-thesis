using System.Collections.Generic;
using System.Threading.Tasks;
using IWA_Backend.API.BusinessLogic.Entities;

namespace IWA_Backend.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User GetByUserName(string? userName);
        IEnumerable<User> GetContractors();
        Task UpdateAsync(User user);
        bool Exists(string? userName);
        IEnumerable<User> GetAllUsers();
    }
}
