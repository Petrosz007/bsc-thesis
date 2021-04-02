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
using IWA_Backend.API.BusinessLogic.Mappers;
using Xunit;

namespace IWA_Backend.Tests.UnitTests.Logic
{
    public class CategoryLogicTest
    {
        private readonly Mock<ICategoryRepository> MockCategoryRepo;
        private readonly Mock<IUserRepository> MockUserRepo;
        private readonly CategoryLogic Logic;

        protected CategoryLogicTest()
        {
            MockCategoryRepo = new Mock<ICategoryRepository>();
            MockUserRepo = new Mock<IUserRepository>();
            Logic = new CategoryLogic(MockCategoryRepo.Object, MockUserRepo.Object);
        }
        
        public class GetById : CategoryLogicTest
        {
            [Fact]
            public void ReturnsCorrect()
            {
                // Arrange
                var category = new Category
                {
                    Owner = new User { UserName = "TestUser" },
                };

                MockCategoryRepo.Setup(r => r.GetById(0)).Returns(category);

                // Act
                var result = Logic.GetCategoryById(0, "TestUser");

                // Assert
                Assert.Equal(category, result);
            }

            [Fact]
            public void NotFound()
            {
                // Arrange
                MockCategoryRepo.Setup(r => r.GetById(0)).Throws(new NotFoundException(""));

                // Act
                // Assert
                Assert.Throws<NotFoundException>(() => Logic.GetCategoryById(0, "TestUser"));
            }

            [Fact]
            public void Unauthorised()
            {
                // Arrange
                var category = new Category
                {
                    Owner = new User { UserName = "TestUser" },
                };

                MockCategoryRepo.Setup(r => r.GetById(0)).Returns(category);

                // Act

                // Assert
                Logic.GetCategoryById(0, "TestUser");
                Assert.Throws<UnauthorisedException>(() => Logic.GetCategoryById(0, "Not the owner"));
            }
        }

        public class Update : CategoryLogicTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var category = new Category
                {
                    Id = 2,
                    Owner = new User { UserName = "Owner" }
                };
                var categoryDto = CategoryMapper.ToDTO(category);

                MockCategoryRepo.Setup(r => r.Exists(2)).Returns(true);
                MockCategoryRepo.Setup(r => r.GetById(2)).Returns(category);

                // Act
                await Logic.UpdateCategoryAsync(categoryDto, "Owner");

                // Assert
                MockCategoryRepo.Verify(r => r.UpdateAsync(category), Times.Once());
            }

            [Fact]
            public async Task DoesntExist()
            {
                // Arrange
                var category = new Category
                {
                    Id = 2,
                    Owner = new User { UserName = "Owner" }
                };
                var categoryDto = CategoryMapper.ToDTO(category);

                MockCategoryRepo.Setup(r => r.Exists(2)).Returns(false);
                MockCategoryRepo.Setup(r => r.GetById(2)).Throws(new NotFoundException(""));

                // Act
                // Assert
                await Assert.ThrowsAsync<NotFoundException>(() => Logic.UpdateCategoryAsync(categoryDto, "Owner"));
                MockCategoryRepo.Verify(r => r.UpdateAsync(category), Times.Never());
            }

            [Fact]
            public async Task Unauthorised()
            {
                // Arrange
                var category = new Category
                {
                    Id = 2,
                    Owner = new User { UserName = "Owner" }
                };
                var categoryDto = CategoryMapper.ToDTO(category);
                
                MockCategoryRepo.Setup(r => r.Exists(2)).Returns(true);
                MockCategoryRepo.Setup(r => r.GetById(2)).Returns(category);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => Logic.UpdateCategoryAsync(categoryDto, "Not Owner"));
                MockCategoryRepo.Verify(r => r.UpdateAsync(category), Times.Never());
            }

            [Fact]
            public async Task UnauthorisedNewOwner()
            {
                // Arrange
                var category = new Category
                {
                    Id = 2,
                    Owner = new User { UserName = "Owner" }
                };
                var categoryDto = CategoryMapper.ToDTO(category);

                MockCategoryRepo.Setup(r => r.Exists(2)).Returns(true);
                MockCategoryRepo.Setup(r => r.GetById(2)).Returns(category);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => Logic.UpdateCategoryAsync(categoryDto, "Not Owner"));
                MockCategoryRepo.Verify(r => r.UpdateAsync(category), Times.Never());
            }
        }

        public class Create : CategoryLogicTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var category = new Category
                {
                    Id = 2,
                    Owner = new User { UserName = "Owner" }
                };
                var categoryDto = CategoryMapper.ToDTO(category);

                // Act
                await Logic.CreateCategoryAsync(categoryDto, "Owner");

                // Assert
                MockCategoryRepo.Verify(r => r.CreateAsync(It.IsAny<Category>()), Times.Once());
            }
        }

        public class Delete : CategoryLogicTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var category = new Category
                {
                    Id = 2,
                    Owner = new User { UserName = "Owner" }
                };

                MockCategoryRepo.Setup(r => r.GetById(2)).Returns(category);

                // Act
                await Logic.DeleteCategoryAsync(2, "Owner");

                // Assert
                MockCategoryRepo.Verify(r => r.DeleteAsync(category), Times.Once());
            }

            [Fact]
            public async Task Unauthorised()
            {
                // Arrange
                var category = new Category
                {
                    Id = 2,
                    Owner = new User { UserName = "Owner" }
                };

                MockCategoryRepo.Setup(r => r.GetById(2)).Returns(category);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => Logic.DeleteCategoryAsync(2, "Not Owner"));
                MockCategoryRepo.Verify(r => r.DeleteAsync(category), Times.Never());
            }

            [Fact]
            public async Task DoesntExist()
            {
                // Arrange
                MockCategoryRepo.Setup(r => r.GetById(2)).Throws(new NotFoundException(""));

                // Act
                // Assert
                await Assert.ThrowsAsync<NotFoundException>(() => Logic.DeleteCategoryAsync(2, "Not Owner"));
                MockCategoryRepo.Verify(r => r.DeleteAsync(It.IsAny<Category>()), Times.Never());
            }
        }
    }
}
