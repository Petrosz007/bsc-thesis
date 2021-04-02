using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using System.Collections.Generic;
using System.Linq;

namespace IWA_Backend.API.BusinessLogic.Mappers
{
    public static class CategoryMapper
    {
        public static Category OntoEntity(Category entity, CategoryDTO dto, IEnumerable<User> users, User owner)
        {
            entity.Id = dto.Id;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.EveryoneAllowed = dto.EveryoneAllowed;
            entity.Owner = owner;
            entity.MaxAttendees = dto.MaxAttendees;
            entity.Price = dto.Price;

            entity.AllowedUsers.Clear();
            entity.AllowedUsers.AddRange(users);

            return entity;
        }

        public static Category IntoEntity(CategoryDTO dto, IEnumerable<User> users, User owner) =>
            OntoEntity(new(), dto, users, owner);
        
        public static CategoryDTO ToDTO(Category entity) =>
            new ()
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
    }
}
