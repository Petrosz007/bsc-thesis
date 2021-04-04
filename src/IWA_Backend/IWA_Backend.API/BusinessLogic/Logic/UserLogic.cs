using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IWA_Backend.API.BusinessLogic.DTOs;

namespace IWA_Backend.API.BusinessLogic.Logic
{
    public class UserLogic
    {
        private readonly IUserRepository UserRepository;
        private readonly IMapper Mapper;

        public UserLogic(IUserRepository userRepository, IMapper mapper)
        {
            UserRepository = userRepository;
            Mapper = mapper;
        }

        public User GetUserByUserName(string userName)
        {
            var user = UserRepository.GetByUserName(userName);
            return user;
        }

        public async Task UpdateUserAsync(UserUpdateDTO userUpdateDto, string? userName)
        {
            var user = UserRepository.GetByUserName(userName);

            Mapper.Map(userUpdateDto, user);

            await UserRepository.UpdateAsync(user);
        }
    }
}
