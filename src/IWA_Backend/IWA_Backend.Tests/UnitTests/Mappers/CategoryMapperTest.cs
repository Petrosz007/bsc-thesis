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
            var category = new Category { Id = 20 };
            var owner = new User { UserName = "owner" };
            var allowedUsers = new List<User> { new User { UserName = "test1" }, new User { UserName = "test2" } };

            var mockRepo = new Mock<IRepository>();
            var mapper = new CategoryMapper(mockRepo.Object);

            var data = new Category
            {
                Id = 10,
                Name = "Test Category",
                Description = "Description of test category",
                AllowedUsers = allowedUsers,
                EveryoneAllowed = false,
                Owner = owner,
                MaxAttendees = 12,
                Price = 5000,
            };

            // Act
            var result = mapper.ToDTO(data);

            // Assert
            Assert.Equal(data.Id, result.Id);
            Assert.Equal(data.Name, result.Name);
            Assert.Equal(data.Description, result.Description);
            Assert.Equal(data.EveryoneAllowed, result.EveryoneAllowed);
            Assert.Equal(data.Owner.UserName, result.OwnerUserName);
            Assert.Equal(data.MaxAttendees, result.MaxAttendees);
            Assert.Equal(data.Price, result.Price);

            var dataAllowedUsers = data.AllowedUsers.Select(a => a.UserName);
            Assert.True(dataAllowedUsers.SequenceEqual(result.AllowedUserNames));
        }

        [Fact]
        public void ToEntity()
        {
            // Arrange
            var owner = new User { UserName = "owner" };
            var allowedCustomers = new List<User> { new User { UserName = "test1" }, new User { UserName = "test2" } };

            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(r => r.GetUserByUserName("owner")).Returns(owner);
            mockRepo.Setup(r => r.GetUserByUserName("test1")).Returns(allowedCustomers[0]);
            mockRepo.Setup(r => r.GetUserByUserName("test2")).Returns(allowedCustomers[1]);
            var mapper = new CategoryMapper(mockRepo.Object);

            var data = new CategoryDTO
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
            var result = mapper.ToEntity(data);

            // Assert
            Assert.Equal(data.Id, result.Id);
            Assert.Equal(data.Name, result.Name);
            Assert.Equal(data.Description, result.Description);
            Assert.Equal(data.EveryoneAllowed, result.EveryoneAllowed);
            Assert.Equal(data.OwnerUserName, result.Owner.UserName);
            Assert.Equal(data.MaxAttendees, result.MaxAttendees);
            Assert.Equal(data.Price, result.Price);

            var resultAllowedUserNames = result.AllowedUsers.Select(a => a.UserName);
            Assert.True(data.AllowedUserNames.SequenceEqual(resultAllowedUserNames));
        }
    }
}
