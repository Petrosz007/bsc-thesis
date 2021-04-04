using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.Repositories;
using IWA_Backend.Tests.Utilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IWA_Backend.API.Repositories.Implementations;
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
        
        [Fact]
        public void getContractors()
        {
            // Arrange
            var users = new List<User>
            {
                new()
                {
                    UserName = "test1",
                    Email = "email1",
                    ContractorPage = new(),
                },
                new()
                {
                    UserName = "test2",
                    Email = "email2",
                },
                new()
                {
                    UserName = "test3",
                    Email = "email3",
                    ContractorPage = new(),
                },
            };
            var mockContext = new MockDbContextBuilder { Users = users }.Build();
            var repo = new UserRepository(mockContext.Object);

            // Act
            var result = repo.GetContractors();
            
            // Assert
            Assert.True(new[] {users[0], users[2]}.SequenceEqual(result));
        }

        [Fact]
        public async Task Update()
        {
            // Arrange
            var users = new List<User>
            {
                new User
                {
                    UserName = "user1",
                },
                new User
                {
                        UserName = "user2",
                },
                new User
                {
                        UserName = "user3",
                },
            };
            var mockContext = new MockDbContextBuilder { Users = users }.Build();
            var repo = new UserRepository(mockContext.Object);

            // Act
            users[1].Email = "newmail@example.com";
            await repo.UpdateAsync(users[1]);

            // Assert
            mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
            mockContext.Verify(c => c.Update(users[0]), Times.Never());
            mockContext.Verify(c => c.Update(users[1]), Times.Once());
            mockContext.Verify(c => c.Update(users[2]), Times.Never());
        }

        [Theory]
        [InlineData("user1", true)]
        [InlineData("user2", true)]
        [InlineData("user3", true)]
        [InlineData("Nonexistent", false)]
        public void Exists(string input, bool expected)
        {
            // Arrange
            var users = new List<User>
            {
                new User
                {
                    UserName = "user1",
                },
                new User
                {
                    UserName = "user2",
                },
                new User
                {
                    UserName = "user3",
                },
            };
            var mockContext = new MockDbContextBuilder { Users = users }.Build();
            var repo = new UserRepository(mockContext.Object);

            // Act
            var result = repo.Exists(input);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
