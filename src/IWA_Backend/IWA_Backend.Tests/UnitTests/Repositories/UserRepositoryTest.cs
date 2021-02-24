using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.Repositories;
using IWA_Backend.Tests.Mock;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IWA_Backend.Tests.UnitTests.Repositories
{
    public class UserRepositoryTest
    {
        [Theory]
        [InlineData("test1", "email1")]
        [InlineData("test2", "email2")]
        [InlineData("test3", "email3")]
        public void GetUserByUserName(string input, string expected)
        {
            // Arrange
            var users = new List<User>
                {
                    new User
                    {
                        UserName = "test1",
                        Email = "email1",
                    },
                    new User
                    {
                        UserName = "test2",
                        Email = "email2",
                    },
                    new User
                    {
                        UserName = "test3",
                        Email = "email3",
                    },
                };
            var mockContext = new MockDbContextBuilder { Users = users }.Build();
            var repo = new UserRepository(mockContext.Object);

            // Act
            var result = repo.GetByUserName(input);

            // Assert
            Assert.Equal(expected, result.Email);
        }

        [Fact]
        public void GetUserByUserNameNotFound()
        {
            // Arrange
            var users = new List<User>
                {
                    new User
                    {
                        UserName = "test1",
                        Email = "email1",
                    },
                    new User
                    {
                        UserName = "test2",
                        Email = "email2",
                    },
                    new User
                    {
                        UserName = "test3",
                        Email = "email3",
                    },
                };
            var mockContext = new MockDbContextBuilder { Users = users }.Build();
            var repo = new UserRepository(mockContext.Object);

            // Act
            // Assert
            Assert.Throws<NotFoundException>(() => repo.GetByUserName("Nonexistent"));
        }
    }
}
