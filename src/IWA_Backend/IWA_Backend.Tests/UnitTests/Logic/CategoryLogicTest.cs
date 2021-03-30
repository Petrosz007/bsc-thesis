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
    public class CategoryLogicTest
    {
        public class GetById
        {
            [Fact]
            public void ReturnsCorrect()
            {
                // Arrange
                var category = new Category
                {
                    Owner = new User { UserName = "TestUser" },
                };

                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockCategoryRepo.Setup(r => r.GetById(0)).Returns(category);
                var mockUserRepo = new Mock<IUserRepository>();
                var logic = new CategoryLogic(mockCategoryRepo.Object, mockUserRepo.Object);

                // Act
                var result = logic.GetCategoryById(0, "TestUser");

                // Assert
                Assert.Equal(category, result);
            }

            [Fact]
            public void NotFound()
            {
                // Arrange
                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockCategoryRepo.Setup(r => r.GetById(0)).Throws(new NotFoundException(""));
                var mockUserRepo = new Mock<IUserRepository>();
                var logic = new CategoryLogic(mockCategoryRepo.Object, mockUserRepo.Object);

                // Act
                // Assert
                Assert.Throws<NotFoundException>(() => logic.GetCategoryById(0, "TestUser"));
            }

            [Fact]
            public void Unauthorised()
            {
                // Arrange
                var category = new Category
                {
                    Owner = new User { UserName = "TestUser" },
                };

                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockCategoryRepo.Setup(r => r.GetById(0)).Returns(category);
                var mockUserRepo = new Mock<IUserRepository>();
                var logic = new CategoryLogic(mockCategoryRepo.Object, mockUserRepo.Object);

                // Act
                var result = logic.GetCategoryById(0, "TestUser");

                // Assert
                Assert.Throws<UnauthorisedException>(() => logic.GetCategoryById(0, "Not the owner")); ;
            }
        }

        public class Update
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

                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockCategoryRepo.Setup(r => r.Exists(2)).Returns(true);
                mockCategoryRepo.Setup(r => r.GetById(2)).Returns(category);
                var mockUserRepo = new Mock<IUserRepository>();
                var logic = new CategoryLogic(mockCategoryRepo.Object, mockUserRepo.Object);

                // Act
                await logic.UpdateCategoryAsync(category, "Owner");

                // Assert
                mockCategoryRepo.Verify(r => r.UpdateAsync(category), Times.Once());
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

                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockCategoryRepo.Setup(r => r.Exists(2)).Returns(false);
                mockCategoryRepo.Setup(r => r.GetById(2)).Throws(new NotFoundException(""));
                var mockUserRepo = new Mock<IUserRepository>();
                var logic = new CategoryLogic(mockCategoryRepo.Object, mockUserRepo.Object);

                // Act
                // Assert
                await Assert.ThrowsAsync<NotFoundException>(() => logic.UpdateCategoryAsync(category, "Owner"));
                mockCategoryRepo.Verify(r => r.UpdateAsync(category), Times.Never());
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

                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockCategoryRepo.Setup(r => r.Exists(2)).Returns(true);
                mockCategoryRepo.Setup(r => r.GetById(2)).Returns(category);
                var mockUserRepo = new Mock<IUserRepository>();
                var logic = new CategoryLogic(mockCategoryRepo.Object, mockUserRepo.Object);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => logic.UpdateCategoryAsync(category, "Not Owner"));
                mockCategoryRepo.Verify(r => r.UpdateAsync(category), Times.Never());
            }

            [Fact]
            public async Task UnauthorisedNewowner()
            {
                // Arrange
                var category = new Category
                {
                    Id = 2,
                    Owner = new User { UserName = "Owner" }
                };

                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockCategoryRepo.Setup(r => r.Exists(2)).Returns(true);
                mockCategoryRepo.Setup(r => r.GetById(2)).Returns(category);
                var mockUserRepo = new Mock<IUserRepository>();
                var logic = new CategoryLogic(mockCategoryRepo.Object, mockUserRepo.Object);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => logic.UpdateCategoryAsync(category, "Not Owner"));
                mockCategoryRepo.Verify(r => r.UpdateAsync(category), Times.Never());
            }
        }

        public class Create
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

                var mockCategoryRepo = new Mock<ICategoryRepository>();
                var mockUserRepo = new Mock<IUserRepository>();
                var logic = new CategoryLogic(mockCategoryRepo.Object, mockUserRepo.Object);

                // Act
                await logic.CreateCategoryAsync(category, "Owner");

                // Assert
                mockCategoryRepo.Verify(r => r.CreateAsync(category), Times.Once());
            }
        }

        public class Delete
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

                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockCategoryRepo.Setup(r => r.GetById(2)).Returns(category);
                var mockUserRepo = new Mock<IUserRepository>();
                var logic = new CategoryLogic(mockCategoryRepo.Object, mockUserRepo.Object);

                // Act
                await logic.DeleteCategoryAsync(2, "Owner");

                // Assert
                mockCategoryRepo.Verify(r => r.DeleteAsync(category), Times.Once());
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

                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockCategoryRepo.Setup(r => r.GetById(2)).Returns(category);
                var mockUserRepo = new Mock<IUserRepository>();
                var logic = new CategoryLogic(mockCategoryRepo.Object, mockUserRepo.Object);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => logic.DeleteCategoryAsync(2, "Not Owner"));
                mockCategoryRepo.Verify(r => r.DeleteAsync(category), Times.Never());
            }

            [Fact]
            public async Task DoesntExist()
            {
                // Arrange
                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockCategoryRepo.Setup(r => r.GetById(2)).Throws(new NotFoundException(""));
                var mockUserRepo = new Mock<IUserRepository>();
                var logic = new CategoryLogic(mockCategoryRepo.Object, mockUserRepo.Object);

                // Act
                // Assert
                await Assert.ThrowsAsync<NotFoundException>(() => logic.DeleteCategoryAsync(2, "Not Owner"));
                mockCategoryRepo.Verify(r => r.DeleteAsync(It.IsAny<Category>()), Times.Never());
            }
        }
    }
}
