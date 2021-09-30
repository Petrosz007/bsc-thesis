using IWA_Backend.API;
using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Mappers;
using IWA_Backend.API.Contexts;
using IWA_Backend.Tests.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IWA_Backend.Tests.IntegrationTests
{
    public class CategoryControllerTest : IntegrationTestBase
    {
        public class GetById : CategoryControllerTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var client = Factory.CreateClient();
                int id = 1;
                var testCategory = CategoryMapper.ToDTO(Context.Categories.First(a => a.Id == id));

                // Act
                var response = await client.GetAsync($"/Category/{id}");

                // Assert
                Assert.True(response.IsSuccessStatusCode);
                var category = await response.Content.ReadAsAsync<CategoryDTO>();
                Assert.True(testCategory.ValuesEqual(category));
            }

            [Fact]
            public async Task NotFound()
            {
                // Arrange
                var client = Factory.CreateClient();
                int id = 10000;

                // Act
                var response = await client.GetAsync($"/Category/{id}");

                // Assert
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }

            [Fact]
            public async Task UnauthorizedLoggedOut()
            {
                // Arrange
                var client = Factory.CreateClient();
                int id = 4;
                var testAppointment = AppointmentMapper.ToDTO(Context.Appointments.First(a => a.Id == id));

                // Act
                var response = await client.GetAsync($"/Category/{id}");

                // Assert
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

            [Fact]
            public async Task UnauthorizedLoggedIn()
            {
                // Arrange
                var client = Factory.CreateClient();
                int id = 4;

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "customer1", Password = "kebab" });
                var response = await client.GetAsync($"/Category/{id}");

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

            [Fact]
            public async Task AuthorizedLoggedIn()
            {
                // Arrange
                var client = Factory.CreateClient();
                int id = 2;
                var testCategory = CategoryMapper.ToDTO(Context.Categories.First(a => a.Id == id));

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "customer1", Password = "kebab" });
                var response = await client.GetAsync($"/Category/{id}");

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.True(response.IsSuccessStatusCode);
                var category = await response.Content.ReadAsAsync<CategoryDTO>();
                Assert.True(testCategory.ValuesEqual(category));
            }
        }

        public class Create : CategoryControllerTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var client = Factory.CreateClient();
                var category = new CategoryDTO
                {
                    Id = 10,
                    Name = "Test Category",
                    Description = "Description of test category",
                    AllowedUserNames = new string[] { "customer1", "customer2" },
                    EveryoneAllowed = false,
                    OwnerUserName = "contractor1",
                    MaxAttendees = 12,
                    Price = 5000,
                };

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.PostAsJsonAsync("/Category", category);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.True(response.IsSuccessStatusCode);
                var responseCategory = await response.Content.ReadAsAsync<CategoryDTO>();
                var dbCategory = CategoryMapper.ToDTO(Context.Categories.First(a => a.Id == responseCategory.Id));
                Assert.True(dbCategory.ValuesEqual(category with { Id = responseCategory.Id }));
            }

            [Fact]
            public async Task AllowedUserNotFound()
            {
                // Arrange
                var client = Factory.CreateClient();
                var category = new CategoryDTO
                {
                    Id = 10,
                    Name = "Test Category",
                    Description = "Description of test category",
                    AllowedUserNames = new string[] { "customer1", "Made up user" },
                    EveryoneAllowed = false,
                    OwnerUserName = "contractor2",
                    MaxAttendees = 12,
                    Price = 5000,
                };

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.PostAsJsonAsync("/Category", category);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }

            [Fact]
            public async Task NotLoggedIn()
            {
                // Arrange
                var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });
                var category = new CategoryDTO
                {
                    Id = 10,
                    Name = "Test Category",
                    Description = "Description of test category",
                    AllowedUserNames = new string[] { "customer1", "customer2" },
                    EveryoneAllowed = false,
                    OwnerUserName = "contractor2",
                    MaxAttendees = 12,
                    Price = 5000,
                };

                // Act
                var response = await client.PostAsJsonAsync("/Category", category);

                // Assert
                Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
                Assert.StartsWith("http://localhost/Account/Login", response?.Headers?.Location?.OriginalString);
            }
        }

        public class Update : CategoryControllerTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var client = Factory.CreateClient();
                int id = 1;
                var category = new CategoryDTO
                {
                    Id = id,
                    Name = "Test Category",
                    Description = "Description of test category",
                    AllowedUserNames = new string[] { "customer1", "customer2" },
                    EveryoneAllowed = false,
                    OwnerUserName = "contractor1",
                    MaxAttendees = 12,
                    Price = 5000,
                };

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.PutAsJsonAsync($"/Category/{id}", category);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.True(response.IsSuccessStatusCode);
                var dbCategory = CategoryMapper.ToDTO(Context.Categories.First(a => a.Id == category.Id));
                Assert.True(dbCategory.ValuesEqual(category));
            }

            [Fact]
            public async Task AllowedUserNotFound()
            {
                // Arrange
                var client = Factory.CreateClient();
                int id = 1;
                var category = new CategoryDTO
                {
                    Id = id,
                    Name = "Test Category",
                    Description = "Description of test category",
                    AllowedUserNames = new string[] { "customer1", "Made up user" },
                    EveryoneAllowed = false,
                    OwnerUserName = "contractor2",
                    MaxAttendees = 12,
                    Price = 5000,
                };

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.PutAsJsonAsync($"/Category/{id}", category);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }

            [Fact]
            public async Task NotLoggedIn()
            {
                // Arrange
                var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });
                int id = 1;
                var category = new CategoryDTO
                {
                    Id = id,
                    Name = "Test Category",
                    Description = "Description of test category",
                    AllowedUserNames = new string[] { "customer1", "customer2" },
                    EveryoneAllowed = false,
                    OwnerUserName = "contractor2",
                    MaxAttendees = 12,
                    Price = 5000,
                };

                // Act
                var response = await client.PutAsJsonAsync($"/Category/{id}", category);

                // Assert
                Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
                Assert.StartsWith("http://localhost/Account/Login", response?.Headers?.Location?.OriginalString);
            }

            [Fact]
            public async Task UnauthorisedLoggedIn()
            {
                // Arrange
                var client = Factory.CreateClient();
                int id = 1;
                var category = new CategoryDTO
                {
                    Id = id,
                    Name = "Test Category",
                    Description = "Description of test category",
                    AllowedUserNames = new string[] { "customer1", "customer2" },
                    EveryoneAllowed = false,
                    OwnerUserName = "contractor2",
                    MaxAttendees = 12,
                    Price = 5000,
                };

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor2", Password = "kebab" });
                var response = await client.PutAsJsonAsync($"/Category/{id}", category);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        public class GetOwnCategories : CategoryControllerTest
        {
            [Fact]
            public async Task NotLoggedIn()
            {
                // Arrange
                var client = Factory.CreateClient();
                var userName = "contractor1";
                var ids = new[] { 1 };
                var testCategories = Context.Categories
                    .Where(a => ids.Contains(a.Id))
                    .ToList()
                    .Select(CategoryMapper.ToDTO);

                // Act
                var response = await client.GetAsync($"/Category/Contractor/{userName}");

                // Assert
                Assert.True(response.IsSuccessStatusCode);
                var categories = await response.Content.ReadAsAsync<IEnumerable<CategoryDTO>>();
                Assert.True(testCategories.All(c => c.ValuesEqual(categories.First(cat => cat.Id == c.Id))));
            }
            
            [Fact]
            public async Task LoggedIn()
            {
                // Arrange
                var client = Factory.CreateClient();
                var userName = "contractor1";
                var ids = new[] { 1, 2 };
                var testCategories = Context.Categories
                    .Where(a => ids.Contains(a.Id))
                    .ToList()
                    .Select(CategoryMapper.ToDTO);

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "customer1", Password = "kebab" });
                var response = await client.GetAsync($"/Category/Contractor/{userName}");

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.True(response.IsSuccessStatusCode);
                var categories = await response.Content.ReadAsAsync<IEnumerable<CategoryDTO>>();
                Assert.True(testCategories.All(c => c.ValuesEqual(categories.First(cat => cat.Id == c.Id))));
            }

            [Fact]
            public async Task NotFound()
            {
                // Arrange
                var client = Factory.CreateClient();
                var userName = "Fake user";

                // Act
                var response = await client.GetAsync($"/Category/Contractor/{userName}");

                // Assert
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}
