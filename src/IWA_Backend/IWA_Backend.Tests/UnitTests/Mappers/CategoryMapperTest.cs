using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Mappers;
using IWA_Backend.API.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IWA_Backend.Tests.UnitTests.Mappers
{
    public class CategoryMapperTest
    {
        [Fact]
        public void ToDTO()
        {
            // Arrange
            var entity = new Category
            {
                Id = 10,
                Name = "Test Category",
                Description = "Description of test category",
                AllowedUsers = new List<User> { new() { UserName = "test1" }, new() { UserName = "test2" } },
                EveryoneAllowed = false,
                Owner = new User { UserName = "owner" },
                MaxAttendees = 12,
                Price = 5000,
            };

            // Act
            var result = CategoryMapper.ToDTO(entity);

            // Assert
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.Description, result.Description);
            Assert.Equal(entity.EveryoneAllowed, result.EveryoneAllowed);
            Assert.Equal(entity.Owner.UserName, result.OwnerUserName);
            Assert.Equal(entity.MaxAttendees, result.MaxAttendees);
            Assert.Equal(entity.Price, result.Price);

            var dataAllowedUsers = entity.AllowedUsers.Select(a => a.UserName);
            Assert.True(dataAllowedUsers.SequenceEqual(result.AllowedUserNames));
        }

        [Fact]
        public void ToEntity()
        {
            // Arrange
            var owner = new User { UserName = "owner" };
            var allowedCustomers = new List<User> { new User { UserName = "test1" }, new User { UserName = "test2" } };

            var dto = new CategoryDTO
            {
                Id = 10,
                Name = "Test Category",
                Description = "Description of test category",
                AllowedUserNames = allowedCustomers.Select(u => u.UserName),
                EveryoneAllowed = false,
                OwnerUserName = owner.UserName,
                MaxAttendees = 12,
                Price = 5000,
            };

            // Act
            var result = new Category();
            CategoryMapper.OntoEntity(result, dto, allowedCustomers, owner);

            // Assert
            Assert.Equal(dto.Id, result.Id);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Description, result.Description);
            Assert.Equal(dto.EveryoneAllowed, result.EveryoneAllowed);
            Assert.Equal(dto.OwnerUserName, result.Owner.UserName);
            Assert.Equal(dto.MaxAttendees, result.MaxAttendees);
            Assert.Equal(dto.Price, result.Price);

            var resultAllowedUserNames = result.AllowedUsers.Select(a => a.UserName);
            Assert.True(dto.AllowedUserNames.SequenceEqual(resultAllowedUserNames));
        }
    }
}
