using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.Tests.Utilities;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IWA_Backend.Tests.IntegrationTests
{
    public class UserControllerIntegrationTest : IntegrationTestBase
    {
        [Collection("Sequential")]
        public class GetByusername : UserControllerIntegrationTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var client = Factory.CreateClient();
                var userName = "contractor1";
                var testUser = Mapper.Map<UserInfoDTO>(Context.Users.First(a => a.UserName == userName));

                // Act
                var response = await client.GetAsync($"/User/Info/{userName}");

                // Assert
                Assert.True(response.IsSuccessStatusCode);
                var user = await response.Content.ReadAsAsync<UserInfoDTO>();
                Assert.Equal(testUser, user);
            }

            [Fact]
            public async Task NotFound()
            {
                // Arrange
                var client = Factory.CreateClient();
                var userName = "Nonexistent";

                // Act
                var response = await client.GetAsync($"/User/Info/{userName}");

                // Assert
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Collection("Sequential")]
        public class Update : UserControllerIntegrationTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var client = Factory.CreateClient();
                var userName = "contractor1";
                var user = new UserUpdateDTO
                {
                    Email = "newmail@example.com",
                    Name = "Most már nem Karcsi",
                    ContractorPage = new ContractorPageDTO
                    {
                        Title = "Ez már nem karcsi oldala",
                        Bio = "New bio",
                    },
                };

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.PutAsJsonAsync($"/User", user);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.True(response.IsSuccessStatusCode);
                var dbUser = Context.Users.First(u => u.UserName == userName);

                Assert.Equal(user.Email, dbUser.Email);
                Assert.Equal(user.Name, dbUser.Name);
                Assert.Equal(user.ContractorPage.Title, dbUser.ContractorPage?.Title);
                Assert.Equal(user.ContractorPage.Bio, dbUser.ContractorPage?.Bio);
            }

            [Fact]
            public async Task NotFound()
            {
                // Arrange
                var client = Factory.CreateClient();
                var userName = "Fake User";
                var user = new UserUpdateDTO
                {
                    Email = "newmail@example.com",
                    Name = "Most már nem Karcsi",
                    ContractorPage = new ContractorPageDTO
                    {
                        Title = "Ez már nem karcsi oldala",
                        Bio = "New bio",
                    },
                };

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.PutAsJsonAsync($"/User", user);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }

            [Fact]
            public async Task UnauthorisedRedirect()
            {
                // Arrange
                var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });
                var userName = "Fake User";
                var user = new UserUpdateDTO
                {
                    Email = "newmail@example.com",
                    Name = "Most már nem Karcsi",
                    ContractorPage = new ContractorPageDTO
                    {
                        Title = "Ez már nem karcsi oldala",
                        Bio = "New bio",
                    },
                };

                // Act
                var response = await client.PutAsJsonAsync($"/User", user);

                // Assert
                Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
                Assert.StartsWith("http://localhost/Account/Login", response?.Headers?.Location?.OriginalString);
            }
        }
    }
}
