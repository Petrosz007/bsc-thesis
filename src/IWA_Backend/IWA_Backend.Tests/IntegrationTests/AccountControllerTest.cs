using IWA_Backend.API.BusinessLogic.DTOs;
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
        [Collection("Sequential")]
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

        [Collection("Sequential")]
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
                var logoutResponse = await client.PostAsync("/Account/Logout", null);

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
                var login = new LoginDTO { UserName = "contractor1", Password = "kebab" };

                // Act
                var response = await client.PostAsync("/Account/Logout", null!);

                // Assert
                Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
                Assert.StartsWith("http://localhost/Account/Login", response.Headers?.Location?.OriginalString);
            }
        }

        [Collection("Sequential")]
        public class Register : AccountControllerTest
        {
            [Fact]
            public async Task Successful()
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
