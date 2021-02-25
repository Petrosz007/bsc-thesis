using AutoMapper;
using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.BusinessLogic.Mappers
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<User, UserInfoDTO>();
            CreateMap<UserUpdateDTO, User>();
            CreateMap<ContractorPage, ContractorPageDTO>()
                .ReverseMap();
        }
    }
}
