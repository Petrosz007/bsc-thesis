using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.BusinessLogic.Mappers
{
    public class CategoryMapper : IMapper<Category, CategoryDTO>
    {
        private readonly IRepository Repository;
        public CategoryMapper(IRepository repository) =>
            Repository = repository;

        public CategoryDTO ToDTO(Category entity) =>
            new CategoryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                AllowedUserNames = entity.AllowedUsers.Select(u => u.UserName),
                EveryoneAllowed = entity.EveryoneAllowed,
                OwnerUserName = entity.Owner.UserName,
                MaxAttendees = entity.MaxAttendees,
                Price = entity.Price,
            };

        public Category ToEntity(CategoryDTO dto) =>
            new Category    
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                AllowedUsers = dto.AllowedUserNames.Select(Repository.GetUserByUserName).ToList(),
                EveryoneAllowed = dto.EveryoneAllowed,
                Owner = Repository.GetUserByUserName(dto.OwnerUserName),
                MaxAttendees = dto.MaxAttendees,
                Price = dto.Price,
            };
    }
}
