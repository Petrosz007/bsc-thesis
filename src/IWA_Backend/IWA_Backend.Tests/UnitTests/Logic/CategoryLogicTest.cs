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

namespace IWA_Backend.Tests.UnitTests
{
    public class CategoryLogicTest
    {
        public class TestGetById
        {
            [Fact]
            public void ReturnsCorrect()
            {
                // Arrange
                var category = new Category
                {
                    Owner = new User { UserName = "TestUser" },
                };

                var mockRepo = new Mock<IRepository>();
                mockRepo.Setup(r => r.GetCategoryById(0)).Returns(category);
                var logic = new CategoryLogic(mockRepo.Object);

                // Act
                var result = logic.GetCategoryById(0, "TestUser");

                // Assert
                Assert.Equal(category, result);
            }

            [Fact]
            public void NotFound()
            {
                // Arrange
                var mockRepo = new Mock<IRepository>();
                mockRepo.Setup(r => r.GetCategoryById(It.IsAny<int>())).Throws(new NotFoundException(""));
                var logic = new CategoryLogic(mockRepo.Object);

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

                var mockRepo = new Mock<IRepository>();
                mockRepo.Setup(r => r.GetCategoryById(0)).Returns(category);
                var logic = new CategoryLogic(mockRepo.Object);

                // Act
                var result = logic.GetCategoryById(0, "TestUser");

                // Assert
                Assert.Throws<UnauthorisedException>(() => logic.GetCategoryById(0, "Not the owner")); ;
            }
        }

        public class TestUpdate
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

                var mockRepo = new Mock<IRepository>();
                mockRepo.Setup(r => r.CategoryExists(2)).Returns(true);
                mockRepo.Setup(r => r.GetCategoryById(2)).Returns(category);
                var logic = new CategoryLogic(mockRepo.Object);

                // Act
                await logic.UpdateCategory(category, "Owner");

                // Assert
                mockRepo.Verify(r => r.UpdateCategory(category), Times.Once());
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

                var mockRepo = new Mock<IRepository>();
                mockRepo.Setup(r => r.CategoryExists(2)).Returns(false);
                mockRepo.Setup(r => r.GetCategoryById(2)).Throws(new NotFoundException(""));
                var logic = new CategoryLogic(mockRepo.Object);

                // Act
                // Assert
                await Assert.ThrowsAsync<NotFoundException>(() => logic.UpdateCategory(category, "Owner"));
                mockRepo.Verify(r => r.UpdateCategory(category), Times.Never());
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

                var mockRepo = new Mock<IRepository>();
                mockRepo.Setup(r => r.CategoryExists(2)).Returns(true);
                mockRepo.Setup(r => r.GetCategoryById(2)).Returns(category);
                var logic = new CategoryLogic(mockRepo.Object);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => logic.UpdateCategory(category, "Not Owner"));
                mockRepo.Verify(r => r.UpdateCategory(category), Times.Never());
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

                var mockRepo = new Mock<IRepository>();
                mockRepo.Setup(r => r.CategoryExists(2)).Returns(true);
                mockRepo.Setup(r => r.GetCategoryById(2)).Returns(category);
                var logic = new CategoryLogic(mockRepo.Object);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => logic.UpdateCategory(category, "Not Owner"));
                mockRepo.Verify(r => r.UpdateCategory(category), Times.Never());
            }
        }

        public class TestCreate
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

                var mockRepo = new Mock<IRepository>();
                var logic = new CategoryLogic(mockRepo.Object);

                // Act
                await logic.CreateCategory(category, "Owner");

                // Assert
                mockRepo.Verify(r => r.CreateCategory(category), Times.Once());
            }
        }

        public class TestDelete
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

                var mockRepo = new Mock<IRepository>();
                mockRepo.Setup(r => r.GetCategoryById(2)).Returns(category);
                var logic = new CategoryLogic(mockRepo.Object);

                // Act
                await logic.DeleteCategory(2, "Owner");

                // Assert
                mockRepo.Verify(r => r.DeleteCategory(category), Times.Once());
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

                var mockRepo = new Mock<IRepository>();
                mockRepo.Setup(r => r.GetCategoryById(2)).Returns(category);
                var logic = new CategoryLogic(mockRepo.Object);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => logic.DeleteCategory(2, "Not Owner"));
                mockRepo.Verify(r => r.DeleteCategory(category), Times.Never());
            }

            [Fact]
            public async Task DoesntExist()
            {
                // Arrange
                var mockRepo = new Mock<IRepository>();
                mockRepo.Setup(r => r.GetCategoryById(2)).Throws(new NotFoundException(""));
                var logic = new CategoryLogic(mockRepo.Object);

                // Act
                // Assert
                await Assert.ThrowsAsync<NotFoundException>(() => logic.DeleteCategory(2, "Not Owner"));
                mockRepo.Verify(r => r.DeleteCategory(It.IsAny<Category>()), Times.Never());
            }
        }
    }
}
