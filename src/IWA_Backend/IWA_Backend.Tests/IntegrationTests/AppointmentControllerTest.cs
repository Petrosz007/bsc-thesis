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
    public class AppointmentControllerTest
    {
        protected readonly TestServer testServer;
        protected IWAContext Context => testServer.Services.GetRequiredService<IWAContext>();
        protected IMapper<Appointment, AppointmentDTO> AppointmentMapper => testServer.Services.GetRequiredService<IMapper<Appointment, AppointmentDTO>>();

        public AppointmentControllerTest()
        {
            testServer = new TestServer(new WebHostBuilder()
                .UseStartup<TestStartup>());
        }

        protected async Task InitialiseDb() =>
            await testServer.Services.GetRequiredService<DbInitialiser>().ReseedDataAsync();

        [Fact]
        public async Task IntegrationTestsWork()
        {
            // Arrange
            var client = testServer.CreateClient();

            // Act
            var response = await client.GetAsync("/api/Account/asd");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal("10", result);
        }

        public class GetById : AppointmentControllerTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var client = testServer.CreateClient();
                await InitialiseDb();
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
                var client = testServer.CreateClient();
                await InitialiseDb();
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
                var client = testServer.CreateClient();
                await InitialiseDb();
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
                var client = testServer.CreateClient();
                await InitialiseDb();
                var testAppointment = AppointmentMapper.ToDTO(Context.Appointments.First(a => a.Id == 3));

                // Act
                var loginResponse = await client.PostAsJsonAsync("/api/Account/Login", new LoginDTO { UserName = "customer2", Password = "kebab" });
                var response = await client.GetAsync("/api/Appointment/3");

                // Assert
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

            // TODO: Authentication doesn't work, cookie issue?
            [Fact]
            public async Task AuthorizedLoggedIn()
            {
                // Arrange
                var client = testServer.CreateClient();
                await InitialiseDb();
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
    }
}
