using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IWAContext Context;
        public UserRepository(IWAContext context) =>
            Context = context;

        public User GetByUserName(string id) =>
            Context.Users
                .FirstOrDefault(user => user.UserName == id)
                ?? throw new NotFoundException($"User with user name '{id}' not found.");

    }
}
