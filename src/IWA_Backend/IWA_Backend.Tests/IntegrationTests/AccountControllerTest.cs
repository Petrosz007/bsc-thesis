using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.Tests.Utilities;
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
    public class AccountControllerTest : IntegrationTestBase
    {
        public class Login : AccountControllerTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var client = Factory.CreateClient();
                var login = new LoginDTO { UserName = "contractor1", Password = "kebab" };

                // Act
                var response = await client.PostAsJsonAsync("/Account/Login", login);

                // Assert
                Assert.True(response.IsSuccessStatusCode);
            }

            [Fact]
            public async Task NotFound()
            {
                // Arrange
                var client = Factory.CreateClient();
                var login = new LoginDTO { UserName = "Not Real User", Password = "kebab" };

                // Act
                var response = await client.PostAsJsonAsync("/Account/Login", login);

                // Assert
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        public class Logout : AccountControllerTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var client = Factory.CreateClient();
                var login = new LoginDTO { UserName = "contractor1", Password = "kebab" };

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", login);
                var logoutResponse = await client.PostAsync("/Account/Logout", null!);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.True(logoutResponse.IsSuccessStatusCode);
            }

            [Fact]
            public async Task NotLoggedIn()
            {
                // Arrange
                var client = Factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });

                // Act
                var response = await client.PostAsync("/Account/Logout", null!);

                // Assert
                Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
                Assert.StartsWith("http://localhost/Account/Login", response.Headers?.Location?.OriginalString);
            }
        }

        public class Register : AccountControllerTest
        {
            [Fact]
            public async Task SuccessfulCustomer()
            {
                // Arrange
                var client = Factory.CreateClient();
                var register = new RegisterDTO 
                { 
                    Name = "New User",
                    UserName = "newuser", 
                    Email = "newuser@example.com",
                    Password = "kebab",
                    PasswordConfirmation = "kebab",
                };

                // Act
                var response = await client.PostAsJsonAsync("/Account/Register", register);

                // Assert
                Assert.True(response.IsSuccessStatusCode);
                var user = Context.Users.First(u => u.UserName == register.UserName);
                Assert.Equal(register.UserName, user.UserName);
                Assert.Equal(register.Email, user.Email);

                var roles = await UserManager.GetRolesAsync(user);
                Assert.DoesNotContain("Contractor", roles);
            }

            [Fact]
            public async Task SuccessfulContractor()
            {
                // Arrange
                var client = Factory.CreateClient();
                var register = new RegisterDTO
                {
                    Name = "New User",
                    UserName = "newuser",
                    Email = "newuser@example.com",
                    Password = "kebab",
                    PasswordConfirmation = "kebab",
                    ContractorPage = new ContractorPageDTO
                    {
                        Title = "Test Title",
                        Bio = "Test Bio",
                    },
                };

                // Act
                var response = await client.PostAsJsonAsync("/Account/Register", register);

                // Assert
                Assert.True(response.IsSuccessStatusCode);
                var user = Context.Users.First(u => u.UserName == register.UserName);
                Assert.Equal(register.UserName, user.UserName);
                Assert.Equal(register.Email, user.Email);

                var roles = await UserManager.GetRolesAsync(user);
                Assert.Contains("Contractor", roles);
            }

            [Fact]
            public async Task DuplicateUsername()
            {
                // Arrange
                var client = Factory.CreateClient();
                var register = new RegisterDTO
                {
                    Name = "New User",
                    UserName = "contractor1",
                    Email = "newuser@example.com",
                    Password = "kebab",
                    PasswordConfirmation = "kebab",
                };

                // Act
                var response = await client.PostAsJsonAsync("/Account/Register", register);

                // Assert
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }
    }
}
