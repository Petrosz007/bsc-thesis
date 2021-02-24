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
    public class AppointmentRepositoryTest
    {
        [Fact]
        public async Task Create()
        {
            // Arrange
            var appointments = new List<Appointment>();
            var mockContext = new MockDbContextBuilder { Appointments = appointments }.Build();
            var repo = new AppointmentRepository(mockContext.Object);

            // Act
            var appointment = new Appointment();
            await repo.CreateAsync(appointment);

            // Assert
            Assert.Single(appointments);
            Assert.Equal(appointment, appointments.First());
            mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task Delete()
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
            var repo = new AppointmentRepository(mockContext.Object);

            // Act
            await repo.DeleteAsync(appointments[1]);

            // Assert
            mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
            mockContext.Verify(c => c.Appointments.Remove(appointments[0]), Times.Never());
            mockContext.Verify(c => c.Appointments.Remove(appointments[1]), Times.Once());
            mockContext.Verify(c => c.Appointments.Remove(appointments[2]), Times.Never());
        }

        [Fact]
        public async Task Update()
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
            var repo = new AppointmentRepository(mockContext.Object);

            // Act
            appointments[1].Id = 10;
            await repo.UpdateAsync(appointments[1]);

            // Assert
            mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
            mockContext.Verify(c => c.Update(appointments[0]), Times.Never());
            mockContext.Verify(c => c.Update(appointments[1]), Times.Once());
            mockContext.Verify(c => c.Update(appointments[2]), Times.Never());
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 20)]
        [InlineData(2, 30)]
        public void GetById(int input, int expected)
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
            var repo = new AppointmentRepository(mockContext.Object);

            // Act
            var result = repo.GetById(input);

            // Assert
            Assert.Equal(expected, result.MaxAttendees);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(100, false)]
        public void Exists(int input, bool expected)
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
            var repo = new AppointmentRepository(mockContext.Object);

            // Act
            var result = repo.Exists(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetByIdNotFound()
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
            var repo = new AppointmentRepository(mockContext.Object);

            // Act
            // Assert
            Assert.Throws<NotFoundException>(() => repo.GetById(100));
        }

        [Fact]
        public void GetAllContractorsAppointments()
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
                        Category = new Category { Owner = users[0] },
                    },
                    new Appointment
                    {
                        Id = 1,
                        Category = new Category { Owner = users[0] },
                    },
                    new Appointment
                    {
                        Id = 2,
                        Category = new Category { Owner = users[1] },
                    },
                };
            var mockContext = new MockDbContextBuilder
            {
                Appointments = appointments,
                Users = users,
            }.Build();
            var repo = new AppointmentRepository(mockContext.Object);

            // Act
            var result = repo.GetContractorsAllAppointments("Contractor").ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.True(result.All(a => a.Category.Owner == users[0]));
        }
    }
}
