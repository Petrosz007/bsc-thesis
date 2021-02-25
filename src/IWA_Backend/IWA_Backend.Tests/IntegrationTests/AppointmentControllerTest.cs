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
    public class AppointmentControllerTest : IntegrationTestBase
    {
        //protected readonly TestWebApplicationFactory<TestStartup> Factory = new();
        //protected IWAContext Context =>
        //    Factory.Services.GetRequiredService<IWAContext>();
        //protected IMapper<Appointment, AppointmentDTO> AppointmentMapper =>
        //    Factory.Services.GetRequiredService<IMapper<Appointment, AppointmentDTO>>();

        [Collection("Sequential")]
        public class GetById : AppointmentControllerTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var client = Factory.CreateClient();
                int id = 1;
                var testAppointment = AppointmentMapper.ToDTO(Context.Appointments.First(a => a.Id == id));

                // Act
                var response = await client.GetAsync($"/Appointment/{id}");

                // Assert
                Assert.True(response.IsSuccessStatusCode);
                var appointment = await response.Content.ReadAsAsync<AppointmentDTO>();
                Assert.True(testAppointment.ValuesEqual(appointment));
            }

            [Fact]
            public async Task NotFound()
            {
                // Arrange
                var client = Factory.CreateClient();
                int id = 10000;

                // Act
                var response = await client.GetAsync($"/Appointment/{id}");

                // Assert
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }

            [Fact]
            public async Task UnauthorizedLoggedOut()
            {
                // Arrange
                var client = Factory.CreateClient();
                int id = 7;
                var testAppointment = AppointmentMapper.ToDTO(Context.Appointments.First(a => a.Id == id));

                // Act
                var response = await client.GetAsync($"/Appointment/{id}");

                // Assert
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

            [Fact]
            public async Task UnauthorizedLoggedIn()
            {
                // Arrange
                var client = Factory.CreateClient();
                int id = 7;
                var testAppointment = AppointmentMapper.ToDTO(Context.Appointments.First(a => a.Id == id));

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "customer1", Password = "kebab" });
                var response = await client.GetAsync($"/Appointment/{id}");

                // Assert
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

            [Fact]
            public async Task AuthorizedLoggedIn()
            {
                // Arrange
                var client = Factory.CreateClient();
                var testAppointment = AppointmentMapper.ToDTO(Context.Appointments.First(a => a.Id == 3));

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "customer1", Password = "kebab" });
                var response = await client.GetAsync("/Appointment/3");

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.True(response.IsSuccessStatusCode);
                var appointment = await response.Content.ReadAsAsync<AppointmentDTO>();
                Assert.True(testAppointment.ValuesEqual(appointment));
            }
        }

        [Collection("Sequential")]
        public class Create : AppointmentControllerTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var client = Factory.CreateClient();
                var appointment = new AppointmentDTO
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    CategoryId = 1,
                    AttendeeUserNames = new string[] { "customer1" },
                    MaxAttendees = 1,
                };

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.PostAsJsonAsync("/Appointment", appointment);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.True(response.IsSuccessStatusCode);
                var responseAppointment = await response.Content.ReadAsAsync<AppointmentDTO>();
                var dbAppointment = AppointmentMapper.ToDTO(Context.Appointments.First(a => a.Id == responseAppointment.Id));
                Assert.True(dbAppointment.ValuesEqual(appointment with { Id = responseAppointment.Id }));
            }

            [Fact]
            public async Task CategoryNotFound()
            {
                // Arrange
                var client = Factory.CreateClient();
                var appointment = new AppointmentDTO
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    CategoryId = 100,
                    AttendeeUserNames = new string[] { "customer1" },
                    MaxAttendees = 1,
                };

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.PostAsJsonAsync("/Appointment", appointment);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }

            [Fact]
            public async Task AttendeeNotFound()
            {
                // Arrange
                var client = Factory.CreateClient();
                var appointment = new AppointmentDTO
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    CategoryId = 1,
                    AttendeeUserNames = new string[] { "NotValidCustomer" },
                    MaxAttendees = 1,
                };

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.PostAsJsonAsync("/Appointment", appointment);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }

            [Fact]
            public async Task UnauthorisedCategory()
            {
                // Arrange
                var client = Factory.CreateClient();
                var appointment = new AppointmentDTO
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    CategoryId = 3,
                    AttendeeUserNames = new string[] { "customer1" },
                    MaxAttendees = 1,
                };

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.PostAsJsonAsync("/Appointment", appointment);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

            [Fact]
            public async Task NotLoggedIn()
            {
                // Arrange
                var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });
                var appointment = new AppointmentDTO
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    CategoryId = 1,
                    AttendeeUserNames = new string[] { "customer1" },
                    MaxAttendees = 1,
                };

                // Act
                var response = await client.PostAsJsonAsync("/Appointment", appointment);

                // Assert
                Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
                Assert.StartsWith("http://localhost/Account/Login", response?.Headers?.Location?.OriginalString);
            }
        }

        [Collection("Sequential")]
        public class Update : AppointmentControllerTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var client = Factory.CreateClient();
                var appointment = new AppointmentDTO
                {
                    Id = 1,
                    StartTime = DateTime.Now.AddYears(2),
                    EndTime = DateTime.Now.AddYears(2).AddHours(1),
                    CategoryId = 1,
                    AttendeeUserNames = new string[] { "customer1" },
                    MaxAttendees = 100,
                };

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.PutAsJsonAsync("/Appointment/1", appointment);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.True(response.IsSuccessStatusCode);
                var dbAppointment = AppointmentMapper.ToDTO(Context.Appointments.First(a => a.Id == appointment.Id));
                Assert.True(dbAppointment.ValuesEqual(appointment));
            }

            [Fact]
            public async Task DifferentId()
            {
                // Arrange
                var client = Factory.CreateClient();
                var appointment = new AppointmentDTO
                {
                    Id = 1,
                    StartTime = DateTime.Now.AddYears(2),
                    EndTime = DateTime.Now.AddYears(2).AddHours(1),
                    CategoryId = 1,
                    AttendeeUserNames = new string[] { "customer1" },
                    MaxAttendees = 100,
                };

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.PutAsJsonAsync("/Appointment/100000", appointment);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }

            [Fact]
            public async Task CategoryNotFound()
            {
                // Arrange
                var client = Factory.CreateClient();
                var appointment = new AppointmentDTO
                {
                    Id = 1,
                    StartTime = DateTime.Now.AddYears(2),
                    EndTime = DateTime.Now.AddYears(2).AddHours(1),
                    CategoryId = 100,
                    AttendeeUserNames = new string[] { "customer1" },
                    MaxAttendees = 100,
                };

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.PutAsJsonAsync("/Appointment/1", appointment);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }

            [Fact]
            public async Task AttendeeNotFound()
            {
                // Arrange
                var client = Factory.CreateClient();
                var appointment = new AppointmentDTO
                {
                    Id = 1,
                    StartTime = DateTime.Now.AddYears(2),
                    EndTime = DateTime.Now.AddYears(2).AddHours(1),
                    CategoryId = 1,
                    AttendeeUserNames = new string[] { "Non Existent User" },
                    MaxAttendees = 100,
                };

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.PutAsJsonAsync("/Appointment/1", appointment);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }

            [Fact]
            public async Task UnauthorisedCategory()
            {
                // Arrange
                var client = Factory.CreateClient();
                var appointment = new AppointmentDTO
                {
                    Id = 1,
                    StartTime = DateTime.Now.AddYears(2),
                    EndTime = DateTime.Now.AddYears(2).AddHours(1),
                    CategoryId = 3,
                    AttendeeUserNames = new string[] { "customer1" },
                    MaxAttendees = 100,
                };

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.PutAsJsonAsync("/Appointment/1", appointment);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

            [Fact]
            public async Task NotLoggedIn()
            {
                // Arrange
                var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });
                var appointment = new AppointmentDTO
                {
                    Id = 1,
                    StartTime = DateTime.Now.AddYears(2),
                    EndTime = DateTime.Now.AddYears(2).AddHours(1),
                    CategoryId = 1,
                    AttendeeUserNames = new string[] { "customer1" },
                    MaxAttendees = 100,
                };

                // Act
                var response = await client.PutAsJsonAsync("/Appointment/1", appointment);

                // Assert
                Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
                Assert.StartsWith("http://localhost/Account/Login", response?.Headers?.Location?.OriginalString);
            }
        }

        [Collection("Sequential")]
        public class Delete : AppointmentControllerTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var client = Factory.CreateClient();

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.DeleteAsync("/Appointment/1");

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.True(response.IsSuccessStatusCode);
                var appointmentExists = Context.Appointments.Any(a => a.Id == 1);
                Assert.False(appointmentExists);
            }

            [Fact]
            public async Task NotFound()
            {
                // Arrange
                var client = Factory.CreateClient();

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.DeleteAsync("/Appointment/1000");

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

                // Act
                var response = await client.DeleteAsync("/Appointment/1");

                // Assert
                Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
                Assert.StartsWith("http://localhost/Account/Login", response?.Headers?.Location?.OriginalString);
            }

            [Fact]
            public async Task UnauthorizedCustomer()
            {
                // Arrange
                var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "customer1", Password = "kebab" });
                var response = await client.DeleteAsync("/Appointment/1");

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
                Assert.StartsWith("http://localhost/Account/AccessDenied", response?.Headers?.Location?.OriginalString);
            }

            [Fact]
            public async Task UnauthorizedContractor()
            {
                // Arrange
                var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });
                var apps = Context.Appointments.ToList().Select(a => (a.Id, a.Category.Id, a.Category.Name)).ToList();

                // Act
                var loginResponse = await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor2", Password = "kebab" });
                var response = await client.DeleteAsync("/Appointment/1");

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }
    }
}
