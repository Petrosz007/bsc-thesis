//using IWA_Backend.API.BusinessLogic.Entities;
//using IWA_Backend.API.BusinessLogic.Exceptions;
//using IWA_Backend.API.Repositories;
//using IWA_Backend.Tests.Mock;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;

//namespace IWA_Backend.Tests.UnitTests
//{
//    public class IWARepositoryTest
//    {
//        public class AppointmentTest
//        {
//            [Fact]
//            public async Task Create()
//            {
//                // Arrange
//                var appointments = new List<Appointment>();
//                var mockContext = new MockDbContextBuilder { Appointments = appointments }.Build();
//                var repo = new IWARepository(mockContext.Object);

//                // Act
//                var appointment = new Appointment();
//                await repo.CreateAppointment(appointment);

//                // Assert
//                Assert.Single(appointments);
//                Assert.Equal(appointment, appointments.First());
//                mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
//            }

//            [Fact]
//            public async Task Delete()
//            {
//                // Arrange
//                var appointments = new List<Appointment>
//                {
//                    new Appointment
//                    {
//                        Id = 0,
//                    },
//                    new Appointment
//                    {
//                        Id = 1,
//                    },
//                    new Appointment
//                    {
//                        Id = 2,
//                    },
//                };
//                var mockContext = new MockDbContextBuilder { Appointments = appointments }.Build();
//                var repo = new IWARepository(mockContext.Object);

//                // Act
//                await repo.DeleteAsync(appointments[1]);

//                // Assert
//                mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
//                mockContext.Verify(c => c.Appointments.Remove(appointments[0]), Times.Never());
//                mockContext.Verify(c => c.Appointments.Remove(appointments[1]), Times.Once());
//                mockContext.Verify(c => c.Appointments.Remove(appointments[2]), Times.Never());
//            }

//            [Fact]
//            public async Task Update()
//            {
//                // Arrange
//                var appointments = new List<Appointment>
//                {
//                    new Appointment
//                    {
//                        Id = 0,
//                    },
//                    new Appointment
//                    {
//                        Id = 1,
//                    },
//                    new Appointment
//                    {
//                        Id = 2,
//                    },
//                };
//                var mockContext = new MockDbContextBuilder { Appointments = appointments }.Build();
//                var repo = new IWARepository(mockContext.Object);

//                // Act
//                appointments[1].Id = 10;
//                await repo.UpdateAsync(appointments[1]);

//                // Assert
//                mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
//                mockContext.Verify(c => c.Update(appointments[0]), Times.Never());
//                mockContext.Verify(c => c.Update(appointments[1]), Times.Once());
//                mockContext.Verify(c => c.Update(appointments[2]), Times.Never());
//            }

//            [Theory]
//            [InlineData(0, 10)]
//            [InlineData(1, 20)]
//            [InlineData(2, 30)]
//            public void GetById(int input, int expected)
//            {
//                // Arrange
//                var appointments = new List<Appointment>
//                {
//                    new Appointment
//                    {
//                        Id = 0,
//                        MaxAttendees = 10,
//                    },
//                    new Appointment
//                    {
//                        Id = 1,
//                        MaxAttendees = 20,
//                    },
//                    new Appointment
//                    {
//                        Id = 2,
//                        MaxAttendees = 30,
//                    },
//                };
//                var mockContext = new MockDbContextBuilder { Appointments = appointments }.Build();
//                var repo = new IWARepository(mockContext.Object);

//                // Act
//                var result = repo.GetById(input);

//                // Assert
//                Assert.Equal(expected, result.MaxAttendees);
//            }

//            [Theory]
//            [InlineData(0, true)]
//            [InlineData(1, true)]
//            [InlineData(2, true)]
//            [InlineData(100, false)]
//            public void Exists(int input, bool expected)
//            {
//                // Arrange
//                var appointments = new List<Appointment>
//                {
//                    new Appointment
//                    {
//                        Id = 0,
//                        MaxAttendees = 10,
//                    },
//                    new Appointment
//                    {
//                        Id = 1,
//                        MaxAttendees = 20,
//                    },
//                    new Appointment
//                    {
//                        Id = 2,
//                        MaxAttendees = 30,
//                    },
//                };
//                var mockContext = new MockDbContextBuilder { Appointments = appointments }.Build();
//                var repo = new IWARepository(mockContext.Object);

//                // Act
//                var result = repo.AppointmentExists(input);

//                // Assert
//                Assert.Equal(expected, result);
//            }

//            [Fact]
//            public void GetByIdNotFound()
//            {
//                // Arrange
//                var appointments = new List<Appointment>
//                {
//                    new Appointment
//                    {
//                        Id = 0,
//                        MaxAttendees = 10,
//                    },
//                    new Appointment
//                    {
//                        Id = 1,
//                        MaxAttendees = 20,
//                    },
//                    new Appointment
//                    {
//                        Id = 2,
//                        MaxAttendees = 30,
//                    },
//                };
//                var mockContext = new MockDbContextBuilder { Appointments = appointments }.Build();
//                var repo = new IWARepository(mockContext.Object);

//                // Act
//                // Assert
//                Assert.Throws<NotFoundException>(() => repo.GetById(100));
//            }

//            [Fact]
//            public void GetAllContractorsAppointments()
//            {
//                // Arrange
//                var users = new List<User>
//                {
//                    new User
//                    {
//                        UserName = "Contractor",
//                        ContractorPage = new(),
//                    },
//                    new User
//                    {
//                        UserName = "Other Contractor",
//                        ContractorPage = new(),
//                    },
//                };

//                var appointments = new List<Appointment>
//                {
//                    new Appointment
//                    {
//                        Id = 0,
//                        Category = new Category { Owner = users[0] },
//                    },
//                    new Appointment
//                    {
//                        Id = 1,
//                        Category = new Category { Owner = users[0] },
//                    },
//                    new Appointment
//                    {
//                        Id = 2,
//                        Category = new Category { Owner = users[1] },
//                    },
//                };
//                var mockContext = new MockDbContextBuilder
//                {
//                    Appointments = appointments,
//                    Users = users,
//                }.Build();
//                var repo = new IWARepository(mockContext.Object);

//                // Act
//                var result = repo.GetAllContractorsAppointments("Contractor").ToList();

//                // Assert
//                Assert.Equal(2, result.Count);
//                Assert.True(result.All(a => a.Category.Owner == users[0]));
//            }
//        }

//        public class CategoryTest
//        {
//            [Fact]
//            public async Task Create()
//            {
//                // Arrange
//                var categories = new List<Category>();
//                var mockContext = new MockDbContextBuilder { Categories = categories }.Build();
//                var repo = new IWARepository(mockContext.Object);

//                // Act
//                var category = new Category();
//                await repo.CreateAsync(category);

//                // Assert
//                Assert.Single(categories);
//                Assert.Equal(category, categories.First());
//                mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
//            }

//            [Fact]
//            public async Task Delete()
//            {
//                // Arrange
//                var appointments = new List<Appointment>
//                {
//                    new Appointment
//                    {
//                        Id = 0,
//                    },
//                    new Appointment
//                    {
//                        Id = 1,
//                    },
//                    new Appointment
//                    {
//                        Id = 2,
//                    },
//                };
//                var mockContext = new MockDbContextBuilder { Appointments = appointments }.Build();
//                var repo = new IWARepository(mockContext.Object);

//                // Act
//                await repo.DeleteAsync(appointments[1]);

//                // Assert
//                mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
//                mockContext.Verify(c => c.Appointments.Remove(appointments[0]), Times.Never());
//                mockContext.Verify(c => c.Appointments.Remove(appointments[1]), Times.Once());
//                mockContext.Verify(c => c.Appointments.Remove(appointments[2]), Times.Never());
//            }

//            [Fact]
//            public async Task Update()
//            {
//                // Arrange
//                var categories = new List<Category>
//                {
//                    new Category
//                    {
//                        Id = 0,
//                    },
//                    new Category
//                    {
//                        Id = 1,
//                    },
//                    new Category
//                    {
//                        Id = 2,
//                    },
//                };
//                var mockContext = new MockDbContextBuilder { Categories = categories }.Build();
//                var repo = new IWARepository(mockContext.Object);

//                // Act
//                categories[1].Id = 10;
//                await repo.UpdateAsync(categories[1]);

//                // Assert
//                mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
//                mockContext.Verify(c => c.Update(categories[0]), Times.Never());
//                mockContext.Verify(c => c.Update(categories[1]), Times.Once());
//                mockContext.Verify(c => c.Update(categories[2]), Times.Never());
//            }


//            [Theory]
//            [InlineData(0, 10)]
//            [InlineData(1, 20)]
//            [InlineData(2, 30)]
//            public void GetById(int input, int expected)
//            {
//                // Arrange
//                var categories = new List<Category>
//                {
//                    new Category
//                    {
//                        Id = 0,
//                        MaxAttendees = 10,
//                    },
//                    new Category
//                    {
//                        Id = 1,
//                        MaxAttendees = 20,
//                    },
//                    new Category
//                    {
//                        Id = 2,
//                        MaxAttendees = 30,
//                    },
//                };
//                var mockContext = new MockDbContextBuilder { Categories = categories }.Build();
//                var repo = new IWARepository(mockContext.Object);

//                // Act
//                var result = repo.GetById(input);

//                // Assert
//                Assert.Equal(expected, result.MaxAttendees);
//            }

//            [Fact]
//            public void GetByIdNotFound()
//            {
//                // Arrange
//                var categories = new List<Category>
//                {
//                    new Category
//                    {
//                        Id = 0,
//                        MaxAttendees = 10,
//                    },
//                    new Category
//                    {
//                        Id = 1,
//                        MaxAttendees = 20,
//                    },
//                    new Category
//                    {
//                        Id = 2,
//                        MaxAttendees = 30,
//                    },
//                };
//                var mockContext = new MockDbContextBuilder { Categories = categories }.Build();
//                var repo = new IWARepository(mockContext.Object);

//                // Act
//                // Assert
//                Assert.Throws<NotFoundException>(() => repo.GetById(100));
//            }

//            [Theory]
//            [InlineData(0, true)]
//            [InlineData(1, true)]
//            [InlineData(2, true)]
//            [InlineData(100, false)]
//            public void Exists(int input, bool expected)
//            {
//                // Arrange
//                var categories = new List<Category>
//                {
//                    new Category
//                    {
//                        Id = 0,
//                    },
//                    new Category
//                    {
//                        Id = 1,
//                    },
//                    new Category
//                    {
//                        Id = 2,
//                    },
//                };
//                var mockContext = new MockDbContextBuilder { Categories = categories }.Build();
//                var repo = new IWARepository(mockContext.Object);

//                // Act
//                var result = repo.CategoryExists(input);

//                // Assert
//                Assert.Equal(expected, result);
//            }

//        }

//        public class TestUser
//        {
//            [Theory]
//            [InlineData("test1", "email1")]
//            [InlineData("test2", "email2")]
//            [InlineData("test3", "email3")]
//            public void GetUserByUserName(string input, string expected)
//            {
//                // Arrange
//                var users = new List<User>
//                {
//                    new User
//                    {
//                        UserName = "test1",
//                        Email = "email1",
//                    },
//                    new User
//                    {
//                        UserName = "test2",
//                        Email = "email2",
//                    },
//                    new User
//                    {
//                        UserName = "test3",
//                        Email = "email3",
//                    },
//                };
//                var mockContext = new MockDbContextBuilder { Users = users }.Build();
//                var repo = new IWARepository(mockContext.Object);

//                // Act
//                var result = repo.GetUserByUserName(input);

//                // Assert
//                Assert.Equal(expected, result.Email);
//            }

//            [Fact]
//            public void GetUserByUserNameNotFound()
//            {
//                // Arrange
//                var users = new List<User>
//                {
//                    new User
//                    {
//                        UserName = "test1",
//                        Email = "email1",
//                    },
//                    new User
//                    {
//                        UserName = "test2",
//                        Email = "email2",
//                    },
//                    new User
//                    {
//                        UserName = "test3",
//                        Email = "email3",
//                    },
//                };
//                var mockContext = new MockDbContextBuilder { Users = users }.Build();
//                var repo = new IWARepository(mockContext.Object);

//                // Act
//                // Assert
//                Assert.Throws<NotFoundException>(() => repo.GetUserByUserName("Nonexistent"));
//            }
//        }
//    }
//}
