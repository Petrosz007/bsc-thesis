using IWA_Backend.API;
using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Mappers;
using IWA_Backend.API.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IWA_Backend.Tests.IntegrationTests
{
    public class AppointmentControllerTest : IClassFixture<TestWebApplicationFactory<TestStartup>>
    {
        protected readonly TestWebApplicationFactory<TestStartup> Factory;
        protected IWAContext Context => 
            Factory.Services.GetRequiredService<IWAContext>();
        protected IMapper<Appointment, AppointmentDTO> AppointmentMapper => 
            Factory.Services.GetRequiredService<IMapper<Appointment, AppointmentDTO>>();

        public AppointmentControllerTest(TestWebApplicationFactory<TestStartup> factory) =>
            Factory = factory;

        [Collection("Sequential")]
        public class GetById : AppointmentControllerTest
        {
            public GetById(TestWebApplicationFactory<TestStartup> factory) : base(factory) { }

            [Fact]
            public async Task Successful()
            {
                // Arrange
                var client = Factory.CreateClient();
                var testAppointment = AppointmentMapper.ToDTO(Context.Appointments.First(a => a.Id == 1));

                // Act
                var response = await client.GetAsync("/api/Appointment/1");

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
                var testAppointment = AppointmentMapper.ToDTO(Context.Appointments.First(a => a.Id == 1));

                // Act
                var response = await client.GetAsync("/api/Appointment/100000");

                // Assert
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }

            [Fact]
            public async Task UnauthorizedLoggedOut()
            {
                // Arrange
                var client = Factory.CreateClient();
                var testAppointment = AppointmentMapper.ToDTO(Context.Appointments.First(a => a.Id == 3));

                // Act
                var response = await client.GetAsync("/api/Appointment/3");

                // Assert
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

            [Fact]
            public async Task UnauthorizedLoggedIn()
            {
                // Arrange
                var client = Factory.CreateClient();
                var testAppointment = AppointmentMapper.ToDTO(Context.Appointments.First(a => a.Id == 3));

                // Act
                var loginResponse = await client.PostAsJsonAsync("/api/Account/Login", new LoginDTO { UserName = "customer2", Password = "kebab" });
                var response = await client.GetAsync("/api/Appointment/3");

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
                var loginResponse = await client.PostAsJsonAsync("/api/Account/Login", new LoginDTO { UserName = "customer1", Password = "kebab" });
                var response = await client.GetAsync("/api/Appointment/3");

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
            public Create(TestWebApplicationFactory<TestStartup> factory) : base(factory) { }

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
                var loginResponse = await client.PostAsJsonAsync("/api/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });
                var response = await client.PostAsJsonAsync("/api/Appointment", appointment);

                // Assert
                Assert.True(loginResponse.IsSuccessStatusCode);
                Assert.True(response.IsSuccessStatusCode);
                var responseAppointment = await response.Content.ReadAsAsync<AppointmentDTO>();
                var dbAppointment = AppointmentMapper.ToDTO(Context.Appointments.First(a => a.Id == responseAppointment.Id));
                Assert.True(dbAppointment.ValuesEqual(appointment with { Id = responseAppointment.Id }));
            }
        }
    }
}
