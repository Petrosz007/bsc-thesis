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

namespace IWA_Backend.Tests.UnitTests
{
    public class IWARepositoryTest
    {
        public class TestAppointment
        {
            [Fact]
            public async Task CreateAppointment()
            {
                // Arrange
                var appointments = new List<Appointment>();
                var mockContext = new MockDbContextBuilder { Appointments = appointments }.Build();
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
            public async Task DeleteAppointment()
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
            public async Task UpdateAppointment()
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
            [InlineData(0, 10)]
            [InlineData(1, 20)]
            [InlineData(2, 30)]
            public void GetAppointmentById(int input, int expected)
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
                Assert.Equal(expected, result.MaxAttendees);
            }

            [Fact]
            public void GetAppointmentByIdNotFound()
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
                // Assert
                Assert.Throws<NotFoundException>(() => repo.GetAppointmentById(100));
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
                var repo = new IWARepository(mockContext.Object);

                // Act
                var result = repo.GetAllContractorsAppointments("Contractor").ToList();

                // Assert
                Assert.Equal(2, result.Count);
                Assert.True(result.All(a => a.Category.Owner == users[0]));
            }
        }

        public class TestCategory
        {
            [Theory]
            [InlineData(0, 10)]
            [InlineData(1, 20)]
            [InlineData(2, 30)]
            public void GetCategoryById(int input, int expected)
            {
                // Arrange
                var categories = new List<Category>
                {
                    new Category
                    {
                        Id = 0,
                        MaxAttendees = 10,
                    },
                    new Category
                    {
                        Id = 1,
                        MaxAttendees = 20,
                    },
                    new Category
                    {
                        Id = 2,
                        MaxAttendees = 30,
                    },
                };
                var mockContext = new MockDbContextBuilder { Categories = categories }.Build();
                var repo = new IWARepository(mockContext.Object);

                // Act
                var result = repo.GetCategoryById(input);

                // Assert
                Assert.Equal(expected, result.MaxAttendees);
            }

            [Fact]
            public void GetCategoryByIdNotFound()
            {
                // Arrange
                var categories = new List<Category>
                {
                    new Category
                    {
                        Id = 0,
                        MaxAttendees = 10,
                    },
                    new Category
                    {
                        Id = 1,
                        MaxAttendees = 20,
                    },
                    new Category
                    {
                        Id = 2,
                        MaxAttendees = 30,
                    },
                };
                var mockContext = new MockDbContextBuilder { Categories = categories }.Build();
                var repo = new IWARepository(mockContext.Object);

                // Act
                // Assert
                Assert.Throws<NotFoundException>(() => repo.GetCategoryById(100));
            }
        }

        public class TestUser
        {
            [Theory]
            [InlineData("test1", "email1")]
            [InlineData("test2", "email2")]
            [InlineData("test3", "email3")]
            public void GetUserByUserName(string input, string expected)
            {
                // Arrange
                var users = new List<User>
                {
                    new User
                    {
                        UserName = "test1",
                        Email = "email1",
                    },
                    new User
                    {
                        UserName = "test2",
                        Email = "email2",
                    },
                    new User
                    {
                        UserName = "test3",
                        Email = "email3",
                    },
                };
                var mockContext = new MockDbContextBuilder { Users = users }.Build();
                var repo = new IWARepository(mockContext.Object);

                // Act
                var result = repo.GetUserByUserName(input);

                // Assert
                Assert.Equal(expected, result.Email);
            }

            [Fact]
            public void GetUserByUserNameNotFound()
            {
                // Arrange
                var users = new List<User>
                {
                    new User
                    {
                        UserName = "test1",
                        Email = "email1",
                    },
                    new User
                    {
                        UserName = "test2",
                        Email = "email2",
                    },
                    new User
                    {
                        UserName = "test3",
                        Email = "email3",
                    },
                };
                var mockContext = new MockDbContextBuilder { Users = users }.Build();
                var repo = new IWARepository(mockContext.Object);

                // Act
                // Assert
                Assert.Throws<NotFoundException>(() => repo.GetUserByUserName("Nonexistent"));
            }

        }
    }
}
