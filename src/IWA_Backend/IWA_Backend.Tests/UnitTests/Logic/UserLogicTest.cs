using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.BusinessLogic.Logic;
using IWA_Backend.API.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IWA_Backend.Tests.UnitTests.Logic
{
    public class UserLogicTest
    {
        public class GetById
        {
            [Fact]
            public void Successful()
            {
                // Arrange
                var user = new User
                {
                   UserName = "TestUser",
                };

                var mockRepo = new Mock<IUserRepository>();
                mockRepo.Setup(r => r.GetByUserName("TestUser")).Returns(user);
                var logic = new UserLogic(mockRepo.Object);

                // Act
                var result = logic.GetUserByUserName("TestUser");

                // Assert
                Assert.Equal(user, result);
            }

            [Fact]
            public void NotFound()
            {
                // Arrange
                var mockRepo = new Mock<IUserRepository>();
                mockRepo.Setup(r => r.GetByUserName("TestUser")).Throws(new NotFoundException(""));
                var logic = new UserLogic(mockRepo.Object);

                // Act
                // Assert
                Assert.Throws<NotFoundException>(() => logic.GetUserByUserName("TestUser"));
            }
        }

        public class Update
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var user = new User
                {
                    UserName = "Test User",
                };
                var mockRepo = new Mock<IUserRepository>();
                mockRepo.Setup(r => r.Exists(user.UserName)).Returns(true);
                var logic = new UserLogic(mockRepo.Object);

                // Act
                await logic.UpdateUserAsync(user, "Test User");

                // Assert
                mockRepo.Verify(r => r.UpdateAsync(user), Times.Once());
            }

            [Fact]
            public async Task Unauthorized()
            {
                // Arrange
                var user = new User
                {
                    UserName = "Test User",
                };
                var mockRepo = new Mock<IUserRepository>();
                mockRepo.Setup(r => r.Exists(user.UserName)).Returns(true);
                var logic = new UserLogic(mockRepo.Object);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => logic.UpdateUserAsync(user, "Not the test user"));
                mockRepo.Verify(r => r.UpdateAsync(user), Times.Never());
            }

            [Fact]
            public async Task NotFound()
            {
                // Arrange
                var user = new User
                {
                    UserName = "Test User",
                };
                var mockRepo = new Mock<IUserRepository>();
                mockRepo.Setup(r => r.Exists(user.UserName)).Returns(false);
                var logic = new UserLogic(mockRepo.Object);

                // Act
                // Assert
                await Assert.ThrowsAsync<NotFoundException>(() => logic.UpdateUserAsync(user, "Test User"));
                mockRepo.Verify(r => r.UpdateAsync(user), Times.Never());
            }
        }
    }
}
