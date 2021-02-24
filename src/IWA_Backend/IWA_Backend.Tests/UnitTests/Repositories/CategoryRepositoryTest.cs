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
    public class CategoryRepositoryTest
    {
        [Fact]
        public async Task Create()
        {
            // Arrange
            var categories = new List<Category>();
            var mockContext = new MockDbContextBuilder { Categories = categories }.Build();
            var repo = new CategoryRepository(mockContext.Object);

            // Act
            var category = new Category();
            await repo.CreateAsync(category);

            // Assert
            Assert.Single(categories);
            Assert.Equal(category, categories.First());
            mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task Delete()
        {
            // Arrange
            var categories = new List<Category>
                {
                    new Category
                    {
                        Id = 0,
                    },
                    new Category
                    {
                        Id = 1,
                    },
                    new Category
                    {
                        Id = 2,
                    },
                };
            var mockContext = new MockDbContextBuilder { Categories = categories }.Build();
            var repo = new CategoryRepository(mockContext.Object);

            // Act
            await repo.DeleteAsync(categories[1]);

            // Assert
            mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
            mockContext.Verify(c => c.Categories.Remove(categories[0]), Times.Never());
            mockContext.Verify(c => c.Categories.Remove(categories[1]), Times.Once());
            mockContext.Verify(c => c.Categories.Remove(categories[2]), Times.Never());
        }

        [Fact]
        public async Task Update()
        {
            // Arrange
            var categories = new List<Category>
                {
                    new Category
                    {
                        Id = 0,
                    },
                    new Category
                    {
                        Id = 1,
                    },
                    new Category
                    {
                        Id = 2,
                    },
                };
            var mockContext = new MockDbContextBuilder { Categories = categories }.Build();
            var repo = new CategoryRepository(mockContext.Object);

            // Act
            categories[1].Id = 10;
            await repo.UpdateAsync(categories[1]);

            // Assert
            mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
            mockContext.Verify(c => c.Update(categories[0]), Times.Never());
            mockContext.Verify(c => c.Update(categories[1]), Times.Once());
            mockContext.Verify(c => c.Update(categories[2]), Times.Never());
        }


        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 20)]
        [InlineData(2, 30)]
        public void GetById(int input, int expected)
        {
            // Arrange
            var categories = new List<Category>
                {
                    new Category
                    {
                        Id = 0,
                        MaxAttendees = 10,
                    },
                    new Category
                    {
                        Id = 1,
                        MaxAttendees = 20,
                    },
                    new Category
                    {
                        Id = 2,
                        MaxAttendees = 30,
                    },
                };
            var mockContext = new MockDbContextBuilder { Categories = categories }.Build();
            var repo = new CategoryRepository(mockContext.Object);

            // Act
            var result = repo.GetById(input);

            // Assert
            Assert.Equal(expected, result.MaxAttendees);
        }

        [Fact]
        public void GetByIdNotFound()
        {
            // Arrange
            var categories = new List<Category>
                {
                    new Category
                    {
                        Id = 0,
                        MaxAttendees = 10,
                    },
                    new Category
                    {
                        Id = 1,
                        MaxAttendees = 20,
                    },
                    new Category
                    {
                        Id = 2,
                        MaxAttendees = 30,
                    },
                };
            var mockContext = new MockDbContextBuilder { Categories = categories }.Build();
            var repo = new CategoryRepository(mockContext.Object);

            // Act
            // Assert
            Assert.Throws<NotFoundException>(() => repo.GetById(100));
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(100, false)]
        public void Exists(int input, bool expected)
        {
            // Arrange
            var categories = new List<Category>
                {
                    new Category
                    {
                        Id = 0,
                    },
                    new Category
                    {
                        Id = 1,
                    },
                    new Category
                    {
                        Id = 2,
                    },
                };
            var mockContext = new MockDbContextBuilder { Categories = categories }.Build();
            var repo = new CategoryRepository(mockContext.Object);

            // Act
            var result = repo.Exists(input);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
