using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.Tests.Utilities;

namespace IWA_Backend.Benchmarks
{
    [MemoryDiagnoser]
    public class IWABenchmarks
    {
        private TestWebApplicationFactory<TestStartup> Factory = new ();
        
        [Benchmark]
        public async Task CreateAppointments()
        {
            // Arrange
            var client = Factory.CreateClient();

            // Act
            await client.PostAsJsonAsync("/Account/Login", new LoginDTO { UserName = "contractor1", Password = "kebab" });

            for (int i = 0; i < 500; ++i)
            {
                var appointment = new AppointmentDTO
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    CategoryId = 1,
                    AttendeeUserNames = new [] { "customer1" },
                    MaxAttendees = 1,
                };

                await client.PostAsJsonAsync("/Appointment", appointment);
            }
        }
        
        [Benchmark]
        public async Task GetContractors()
        {
            // Arrange
            var client = Factory.CreateClient();

            // Act
            for (int i = 0; i < 500; ++i)
            {
                await client.GetAsync("/User/Contractors");
            }
        }
        
        [Benchmark]
        public async Task GetAppointmentsOfContractor()
        {
            // Arrange
            var client = Factory.CreateClient();

            // Act
            for (int i = 0; i < 500; ++i)
            {
                await client.GetAsync("/Appointment/Contractor/contractor1");
            }
        }
    }
}