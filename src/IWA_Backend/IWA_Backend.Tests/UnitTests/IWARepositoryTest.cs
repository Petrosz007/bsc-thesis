using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.Repositories;
using IWA_Backend.Tests.Mocks;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IWA_Backend.Tests.UnitTests
{
    public class IWARepositoryTest
    {
        [Fact]
        public async Task TestCreateAppointment()
        {
            // Arrange
            var appointments = new List<Appointment>();
            var mockContext = new MockDbContextBuilder{ Appointments = appointments }.Build();
            var repo = new IWARepository(mockContext.Object);

            // Act
            var appointment = new Appointment();
            await repo.CreateAppointment(appointment);

            // Assert
            Assert.Single(appointments);
            Assert.Equal(appointment, appointments.First());
            mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task TestDeleteAppointment()
        {
            // Arrange
            var appointments = new List<Appointment>
            { 
                new Appointment
                {
                    Id = 0,
                },
                new Appointment
                {
                    Id = 1,
                },
                new Appointment
                {
                    Id = 2,
                },
            };
            var mockContext = new MockDbContextBuilder { Appointments = appointments }.Build();
            var repo = new IWARepository(mockContext.Object);

            // Act
            await repo.DeleteAppointment(appointments[1]);

            // Assert
            mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
            mockContext.Verify(c => c.Appointments.Remove(appointments[0]), Times.Never());
            mockContext.Verify(c => c.Appointments.Remove(appointments[1]), Times.Once());
            mockContext.Verify(c => c.Appointments.Remove(appointments[2]), Times.Never());
        }

        [Fact]
        public async Task TestUpdateAppointment()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new Appointment
                {
                    Id = 0,
                },
                new Appointment
                {
                    Id = 1,
                },
                new Appointment
                {
                    Id = 2,
                },
            };
            var mockContext = new MockDbContextBuilder { Appointments = appointments }.Build();
            var repo = new IWARepository(mockContext.Object);

            // Act
            appointments[1].Id = 10;
            await repo.UpdateAppointment(appointments[1]);

            // Assert
            mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
            mockContext.Verify(c => c.Update(appointments[0]), Times.Never());
            mockContext.Verify(c => c.Update(appointments[1]), Times.Once());
            mockContext.Verify(c => c.Update(appointments[2]), Times.Never());
        }

        [Theory]
        [InlineData(0,10)]
        [InlineData(1,20)]
        [InlineData(2,30)]
        [InlineData(100, null)]
        public void TestGetAppointmentById(int input, int? expected)
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new Appointment
                {
                    Id = 0,
                    MaxAttendees = 10,
                },
                new Appointment
                {
                    Id = 1,
                    MaxAttendees = 20,
                },
                new Appointment
                {
                    Id = 2,
                    MaxAttendees = 30,
                },
            };
            var mockContext = new MockDbContextBuilder { Appointments = appointments }.Build();
            var repo = new IWARepository(mockContext.Object);

            // Act
            var result = repo.GetAppointmentById(input);

            // Assert
            Assert.Equal(expected, result?.MaxAttendees);
        }

        [Fact]
        public void TestGetAllContractorsAppointments()
        {
            // Arrange
            var users = new List<User>
            {
                new User
                {
                    UserName = "Contractor",
                    ContractorPage = new(),
                },
                new User
                {
                    UserName = "Other Contractor",
                    ContractorPage = new(),
                },
            };

            var appointments = new List<Appointment>
            {
                new Appointment
                {
                    Id = 0,
                    Owner = users[0],
                },
                new Appointment
                {
                    Id = 1,
                    Owner = users[0],
                },
                new Appointment
                {
                    Id = 2,
                    Owner = users[1],
                },
            };
            var mockContext = new MockDbContextBuilder 
            { 
                Appointments = appointments,
                Users = users,
            }.Build();
            var repo = new IWARepository(mockContext.Object);

            // Act
            var result = repo.GetAllContractorsAppointments("Contractor").ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.True(result.All(a => a.Owner == users[0]));
        }
    }
}
