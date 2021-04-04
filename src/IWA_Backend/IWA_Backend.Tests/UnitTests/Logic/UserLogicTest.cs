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
using AutoMapper;
using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Mappers;
using Xunit;

namespace IWA_Backend.Tests.UnitTests.Logic
{
    public class UserLogicTest
    {
        private readonly Mock<IUserRepository> MockUserRepo = new();
        private readonly IMapper Mapper;
        private readonly UserLogic Logic;
        
        protected UserLogicTest()
        {
            Mapper = new MapperConfiguration(c => c.AddProfile<AutoMapping>()).CreateMapper();
            Logic = new(MockUserRepo.Object, Mapper);
        }
        
        public class GetById : UserLogicTest
        {
            [Fact]
            public void Successful()
            {
                // Arrange
                var user = new User
                {
                   UserName = "TestUser",
                };

                MockUserRepo.Setup(r => r.GetByUserName("TestUser")).Returns(user);

                // Act
                var result = Logic.GetUserByUserName("TestUser");

                // Assert
                Assert.Equal(user, result);
            }

            [Fact]
            public void NotFound()
            {
                // Arrange
                MockUserRepo.Setup(r => r.GetByUserName("TestUser")).Throws(new NotFoundException(""));

                // Act
                // Assert
                Assert.Throws<NotFoundException>(() => Logic.GetUserByUserName("TestUser"));
            }
        }

        public class GetContractors : UserLogicTest
        {
            [Fact]
            public void Successful()
            {
                // Arrange
                var contractors = new List<User>
                {
                    new()
                    {
                        UserName = "Contractor1",
                        ContractorPage = new(),
                    },
                    new()
                    {
                        UserName = "Contractor2",
                        ContractorPage = new(),
                    },
                };

                MockUserRepo.Setup(r => r.GetContractors()).Returns(contractors);

                // Act
                var result = Logic.GetContractors();
                
                // Assert
                Assert.Equal(2, contractors.Count);
                Assert.True(contractors.SequenceEqual(result));
            }
        }

        public class Update : UserLogicTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var user = new User
                {
                    UserName = "Test User",
                };

                var dto = new UserUpdateDTO
                {
                    Name = "Test User Changed",
                    Email = "newemail@exmaple.com",
                    ContractorPage = new ContractorPageDTO
                    {
                        Title = "New title",
                        Bio = "New bio",
                    },
                };
                
                MockUserRepo.Setup(r => r.GetByUserName(user.UserName)).Returns(user);

                // Act
                await Logic.UpdateUserAsync(dto, "Test User");

                // Assert
                MockUserRepo.Verify(r => r.UpdateAsync(user), Times.Once());
                Assert.Equal(dto.Name, user.Name);
                Assert.Equal(dto.Email, user.Email);
                Assert.Equal(dto.ContractorPage?.Title, user.ContractorPage?.Title);
                Assert.Equal(dto.ContractorPage?.Bio, user.ContractorPage?.Bio);
            }
        }
    }
}
