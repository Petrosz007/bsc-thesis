using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace IWA_Backend.API.BusinessLogic.Logic
{
    public class UserLogic
    {
        private readonly IUserRepository UserRepository;
        private readonly IAvatarRepository AvatarRepository;
        private readonly IMapper Mapper;

        public UserLogic(IUserRepository userRepository, IMapper mapper, IAvatarRepository avatarRepository)
        {
            UserRepository = userRepository;
            Mapper = mapper;
            AvatarRepository = avatarRepository;
        }

        public User GetUserByUserName(string userName)
        {
            var user = UserRepository.GetByUserName(userName);
            return user;
        }

        public IEnumerable<User> GetContractors() =>
            UserRepository.GetContractors();

        public async Task UpdateUserAsync(UserUpdateDTO userUpdateDto, string? userName)
        {
            var user = UserRepository.GetByUserName(userName);

            Mapper.Map(userUpdateDto, user);

            await UserRepository.UpdateAsync(user);
        }

        public async Task UpdateAvatar(IFormFile file, string userName)
        {
            var user = UserRepository.GetByUserName(userName);
            var previousAvatar = user.ContractorPage!.Avatar;

            var size = file.Length;
            if (size == 0)
                throw new InvalidAvatarFileException("Empty file");

            var extension = AvatarFileTypesExtensions.FromExtension(
                    Path.GetExtension(file.FileName).ToLowerInvariant()
            );

            var id = await AvatarRepository.CreateAsync(file, extension);
            user.ContractorPage!.Avatar = id;
            await UserRepository.UpdateAsync(user);
            
            if(previousAvatar is not null)
                await AvatarRepository.DeleteAsync(previousAvatar);
        }

        public async Task<(byte[] Bytes, string MIMEType)> GetAvatarAsync(string? userName)
        {
            var user = UserRepository.GetByUserName(userName);
            if(user.ContractorPage is null)
                throw new NotContractorException("User is not a contractor");

            if (user.ContractorPage.Avatar is null)
            {
                var (bytes, extension) = await AvatarRepository.GetDefaultAsync();
                return (bytes, extension.GetMIMEType());
            }
            else
            {
                var (bytes, extension) = await AvatarRepository.GetByIdAsync(user.ContractorPage.Avatar);
                return (bytes, extension.GetMIMEType());
            }
        }
    }
}
