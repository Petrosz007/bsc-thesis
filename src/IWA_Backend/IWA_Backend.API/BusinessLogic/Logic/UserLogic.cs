using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.BusinessLogic.Logic
{
    public class UserLogic
    {
        private readonly IUserRepository UserRepository;

        public UserLogic(IUserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        public User GetUserByUserName(string userName)
        {
            var user = UserRepository.GetByUserName(userName);
            return user;
        }

        public async Task UpdateUserAsync(User user, string? userName)
        {
            if(user.UserName != userName)
                throw new UnauthorisedException("Unauthorised to update user.");

            if(!UserRepository.Exists(user.UserName))
                throw new NotFoundException($"User with username '{user.UserName}' not found.");

            await UserRepository.UpdateAsync(user);
        }
    }
}
