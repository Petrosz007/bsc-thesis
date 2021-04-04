using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.Contexts;
using IWA_Backend.API.Repositories.Interfaces;

namespace IWA_Backend.API.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly IWAContext Context;
        public UserRepository(IWAContext context) =>
            Context = context;

        public bool Exists(string? userName) =>
            Context.Users
                .Any(u => u.UserName == userName);

        public User GetByUserName(string? id) =>
            Context.Users
                .FirstOrDefault(user => user.UserName == id)
                ?? throw new NotFoundException($"User with user name '{id}' not found.");

        public IEnumerable<User> GetContractors() =>
            Context.Users
                .Where(user => user.ContractorPage != null)
                .ToList();

        public async Task UpdateAsync(User user)
        {
            Context.Update(user);
            await Context.SaveChangesAsync();
        }
    }
}
