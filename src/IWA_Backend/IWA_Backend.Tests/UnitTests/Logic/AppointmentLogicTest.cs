using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.BusinessLogic.Logic;
using IWA_Backend.API.BusinessLogic.Mappers;
using IWA_Backend.API.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IWA_Backend.Tests.UnitTests.Logic
{
    public class AppointmentLogicTest
    {
        private readonly Mock<IAppointmentRepository> MockAppointmentRepo = new();
        private readonly Mock<ICategoryRepository> MockCategoryRepo = new();
        private readonly Mock<IUserRepository> MockUserRepo = new();
        private readonly AppointmentLogic Logic;

        protected AppointmentLogicTest()
        {
            Logic = new(MockAppointmentRepo.Object, MockCategoryRepo.Object, MockUserRepo.Object);
        }
        public class GetById : AppointmentLogicTest
        {
            [Fact]
            public void ReturnsCorrect()
            {
                // Arrange
                var appointment = new Appointment
                {
                    Attendees = new List<User>
                    {
                        new User
                        {
                            UserName = "TestUser",
                        },
                    },
                    Category = new Category { Owner = new User { UserName = "OwnerUser" } }
                };

                MockAppointmentRepo.Setup(r => r.GetById(0)).Returns(appointment);

                // Act
                var result = Logic.GetAppointmentById(0, "TestUser");

                // Assert
                Assert.Equal(appointment, result);
            }

            [Fact]
            public void NotFound()
            {
                // Arrange
                MockAppointmentRepo.Setup(r => r.GetById(It.IsAny<int>())).Throws(new NotFoundException(""));

                // Act
                // Assert
                Assert.Throws<NotFoundException>(() => Logic.GetAppointmentById(0, "TestUser"));
            }

            [Fact]
            public void Unauthorised()
            {
                // Arrange
                var appointment = new Appointment
                {
                    Category = new Category { Owner = new User { UserName = "Owner User" } }
                };

                MockAppointmentRepo.Setup(r => r.GetById(0)).Returns(appointment);

                // Act
                // Assert
                Assert.Throws<UnauthorisedException>(() => Logic.GetAppointmentById(0, "Definitly not allowed user"));
            }
        }

        public class HasReadAccess
        {
            [Fact]
            public void CategoryEveryoneAllowed()
            {
                // Arrange
                var appointment = new Appointment
                {
                    Category = new Category
                    {
                        EveryoneAllowed = true,
                        Owner = new User { UserName = "OwnerUser" }
                    },
                };

                // Act
                var result = AppointmentLogic.HasReadAccess(appointment, "Test User");

                // Assert
                Assert.True(result);
            }

            [Fact]
            public void OwnerAllowed()
            {
                // Arrange
                var user = new User
                {
                    UserName = "Owner",
                };

                var appointment = new Appointment
                {
                    Category = new Category
                    {
                        Owner = user,
                    },
                };

                // Act
                var result = AppointmentLogic.HasReadAccess(appointment, user.UserName);

                // Assert
                Assert.True(result);
            }

            [Fact]
            public void AttendeeAllowed()
            {
                // Arrange
                var user = new User
                {
                    UserName = "Attendee",
                };

                var appointment = new Appointment
                {
                    Attendees = new List<User> { user },
                    Category = new Category { Owner = new User { UserName = "OwnerUser" } },
                };

                // Act
                var result = AppointmentLogic.HasReadAccess(appointment, user.UserName);

                // Assert
                Assert.True(result);
            }

            [Fact]
            public void CategoryAllowed()
            {
                // Arrange
                var user = new User
                {
                    UserName = "Attendee",
                };

                var appointment = new Appointment
                {
                    Category = new Category
                    {
                        AllowedUsers = new List<User> { user },
                        Owner = new User { UserName = "OwnerUser" },
                    },
                };

                // Act
                var result = AppointmentLogic.HasReadAccess(appointment, user.UserName);

                // Assert
                Assert.True(result);
            }

            [Fact]
            public void NullUserNotAllowed()
            {
                // Arrange
                var appointment = new Appointment
                {
                    Category = new Category { Owner = new User { UserName = "OwnerUser" } },
                };

                // Act
                var result = AppointmentLogic.HasReadAccess(appointment, null);

                // Assert
                Assert.False(result);
            }

            [Fact]
            public void NullUserCategoryAllowed()
            {
                // Arrange
                var appointment = new Appointment
                {
                    Category = new Category
                    {
                        EveryoneAllowed = true,
                        Owner = new User { UserName = "OwnerUser" },
                    },
                };

                // Act
                var result = AppointmentLogic.HasReadAccess(appointment, null);

                // Assert
                Assert.True(result);
            }

            [Fact]
            public void NotAllowed()
            {
                // Arrange
                var appointment = new Appointment
                { 
                    Category = new Category { Owner = new User { UserName = "OwnerUser" } },
                };

                // Act
                var result = AppointmentLogic.HasReadAccess(appointment, "Not allowed User");

                // Assert
                Assert.False(result);
            }
        }

        public class HasWriteAccess
        {
            [Fact]
            public void Allowed()
            {
                // Arrange
                var category = new Category
                {
                    Id = 10,
                    Owner = new User { UserName = "OwnerUser" },
                };

                // Act
                var result = AppointmentLogic.HasWriteAccess(category, "OwnerUser");

                // Assert
                Assert.True(result);
            }

            [Fact]
            public void NotAllowed()
            {
                // Arrange
                var category = new Category
                {
                    Id = 10,
                    Owner = new User { UserName = "OwnerUser" },
                };

                // Act
                var result = AppointmentLogic.HasWriteAccess(category, "Not allowed User");

                // Assert
                Assert.False(result);
            }
        }

        public class Update : AppointmentLogicTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var appointment = new Appointment
                {
                    Id = 10,
                    Category = new Category { Id = 2, Owner = new User { UserName = "Owner" } },
                };
                var appointmentDto = AppointmentMapper.ToDTO(appointment);

                MockAppointmentRepo.Setup(r => r.GetById(10)).Returns(appointment);
                MockCategoryRepo.Setup(r => r.GetById(2)).Returns(appointment.Category);

                // Act
                await Logic.UpdateAppointmentAsync(appointmentDto, "Owner");

                // Assert
                MockAppointmentRepo.Verify(r => r.UpdateAsync(appointment), Times.Once());
            }

            [Fact]
            public async Task DoesntExist()
            {
                // Arrange
                var appointment = new Appointment
                {
                    Id = 10,
                    Category = new Category { Id = 2, Owner = new User { UserName = "Owner" } },
                };
                var appointmentDto = AppointmentMapper.ToDTO(appointment);

                MockAppointmentRepo.Setup(r => r.GetById(10)).Throws(new NotFoundException(""));
                MockCategoryRepo.Setup(r => r.GetById(2)).Returns(appointment.Category);

                // Act
                // Assert
                await Assert.ThrowsAsync<NotFoundException>(() => Logic.UpdateAppointmentAsync(appointmentDto, "Owner"));

                MockAppointmentRepo.Verify(r => r.UpdateAsync(appointment), Times.Never());
            }

            [Fact]
            public async Task Unauthorised()
            {
                // Arrange
                var appointment = new Appointment
                {
                    Id = 10,
                    Category = new Category { Id = 2, Owner = new User { UserName = "Owner" } },
                };
                var appointmentDto = AppointmentMapper.ToDTO(appointment);

                MockAppointmentRepo.Setup(r => r.GetById(10)).Returns(appointment);
                MockCategoryRepo.Setup(r => r.GetById(2)).Returns(appointment.Category);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => Logic.UpdateAppointmentAsync(appointmentDto, "NotOwner"));

                MockAppointmentRepo.Verify(r => r.UpdateAsync(appointment), Times.Never());
            }
        }

        public class Create : AppointmentLogicTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var appointment = new Appointment
                {
                    Id = 10,
                    Category = new Category { Id = 2, Owner = new User { UserName = "Owner" } },
                };
                var appointmentDto = AppointmentMapper.ToDTO(appointment);

                MockCategoryRepo.Setup(r => r.GetById(2)).Returns(appointment.Category);

                // Act
                await Logic.CreateAppointmentAsync(appointmentDto, "Owner");

                // Assert
                MockAppointmentRepo.Verify(r => r.CreateAsync(It.IsAny<Appointment>()), Times.Once());
            }

            [Fact]
            public async Task Unauthorised()
            {
                // Arrange
                var appointment = new Appointment
                {
                    Id = 10,
                    Category = new Category { Id = 2, Owner = new User { UserName = "Owner" } },
                };
                var appointmentDto = AppointmentMapper.ToDTO(appointment);

                MockCategoryRepo.Setup(r => r.GetById(2)).Returns(appointment.Category);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => Logic.CreateAppointmentAsync(appointmentDto, "NotOwner"));

                MockAppointmentRepo.Verify(r => r.CreateAsync(It.IsAny<Appointment>()), Times.Never());
            }
        }

        public class Delete : AppointmentLogicTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                var appointment = new Appointment
                {
                    Id = 10,
                    Category = new Category { Id = 2, Owner = new User { UserName = "Owner" } },
                };

                MockAppointmentRepo.Setup(r => r.GetById(10)).Returns(appointment);
                MockCategoryRepo.Setup(r => r.GetById(2)).Returns(appointment.Category);

                // Act
                await Logic.DeleteAppointmentAsync(10, "Owner");

                // Assert
                MockAppointmentRepo.Verify(r => r.DeleteAsync(appointment), Times.Once());
            }

            [Fact]
            public async Task Unauthorised()
            {
                // Arrange
                var appointment = new Appointment
                {
                    Id = 10,
                    Category = new Category { Id = 2, Owner = new User { UserName = "Owner" } },
                };

                MockAppointmentRepo.Setup(r => r.GetById(10)).Returns(appointment);
                MockCategoryRepo.Setup(r => r.GetById(2)).Returns(appointment.Category);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => Logic.DeleteAppointmentAsync(10, "NotOwner"));

                MockAppointmentRepo.Verify(r => r.DeleteAsync(appointment), Times.Never());
            }

            [Fact]
            public async Task DoesntExist()
            {
                // Arrange
                var appointment = new Appointment
                {
                    Id = 10,
                    Category = new Category { Id = 2, Owner = new User { UserName = "Owner" } },
                };

                MockAppointmentRepo.Setup(r => r.GetById(100)).Throws(new NotFoundException(""));

                // Act
                // Assert
                await Assert.ThrowsAsync<NotFoundException>(() => Logic.DeleteAppointmentAsync(100, "Owner"));

                MockAppointmentRepo.Verify(r => r.DeleteAsync(appointment), Times.Never());
            }
        }

        public class GetBooked : AppointmentLogicTest
        {
            [Fact]
            public void Successful()
            {
                // Arrange
                var appointments = new List<Appointment>();

                MockAppointmentRepo.Setup(r => r.GetBookedAppointments("Test User")).Returns(appointments.AsQueryable());

                // Act
                var result = Logic.GetBookedAppointments("TestUser");

                // Assert
                Assert.True(appointments.SequenceEqual(result));
            }
        }

        public class Book : AppointmentLogicTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                int id = 1;
                var userName = "Test User";
                var user = new User { UserName = userName };
                var appointment = new Appointment
                {
                    Id = id,
                    Category = new Category 
                    {
                        EveryoneAllowed = true,
                        Owner = new User { UserName = "Owner" }
                    },
                };

                MockAppointmentRepo.Setup(r => r.GetById(id)).Returns(appointment);
                MockUserRepo.Setup(r => r.GetByUserName(userName)).Returns(user);

                // Act
                await Logic.BookAppointmentAsync(id, userName);

                // Assert
                Assert.Equal(user, appointment.Attendees.First());
                MockAppointmentRepo.Verify(r => r.UpdateAsync(appointment), Times.Once());
            }

            [Fact]
            public async Task Unauthorised()
            {
                // Arrange
                int id = 1;
                var userName = "Test User";
                var appointment = new Appointment
                {
                    Id = id,
                    Category = new Category
                    {
                        EveryoneAllowed = false,
                        Owner = new User { UserName = "Owner" }
                    },
                };

                MockAppointmentRepo.Setup(r => r.GetById(id)).Returns(appointment);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => Logic.BookAppointmentAsync(id, userName));
                Assert.Empty(appointment.Attendees);
                MockAppointmentRepo.Verify(r => r.UpdateAsync(appointment), Times.Never());
            }

            [Fact]
            public async Task AlreadyBooked()
            {
                // Arrange
                int id = 1;
                var userName = "Test User";
                var appointment = new Appointment
                {
                    Id = id,
                    Attendees = new List<User>{ new User{ UserName = userName } },
                    Category = new Category
                    {
                        EveryoneAllowed = false,
                        Owner = new User { UserName = "Owner" }
                    },
                };

                MockAppointmentRepo.Setup(r => r.GetById(id)).Returns(appointment);

                // Act
                // Assert
                await Assert.ThrowsAsync<AlreadyBookedException>(() => Logic.BookAppointmentAsync(id, userName));
                MockAppointmentRepo.Verify(r => r.UpdateAsync(appointment), Times.Never());
            }
        }

        public class UnBook : AppointmentLogicTest
        {
            [Fact]
            public async Task Successful()
            {
                // Arrange
                int id = 1;
                var userName = "Test User";
                var appointment = new Appointment
                {
                    Id = id,
                    Attendees = new List<User>{ new User{ UserName = userName } },
                    Category = new Category
                    {
                        EveryoneAllowed = true,
                        Owner = new User { UserName = "Owner" }
                    },
                };

                MockAppointmentRepo.Setup(r => r.GetById(id)).Returns(appointment);

                // Act
                await Logic.UnBookAppointmentAsync(id, userName);

                // Assert
                Assert.Empty(appointment.Attendees);
                MockAppointmentRepo.Verify(r => r.UpdateAsync(appointment), Times.Once());
            }

            [Fact]
            public async Task Unauthorised()
            {
                // Arrange
                int id = 1;
                var userName = "Test User";
                var appointment = new Appointment
                {
                    Id = id,
                    Category = new Category
                    {
                        EveryoneAllowed = false,
                        Owner = new User { UserName = "Owner" }
                    },
                };

                MockAppointmentRepo.Setup(r => r.GetById(id)).Returns(appointment);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => Logic.UnBookAppointmentAsync(id, userName));
                MockAppointmentRepo.Verify(r => r.UpdateAsync(appointment), Times.Never());
            }

            [Fact]
            public async Task NotBooked()
            {
                // Arrange
                int id = 1;
                var userName = "Test User";
                var appointment = new Appointment
                {
                    Id = id,
                    Category = new Category
                    {
                        EveryoneAllowed = true,
                        Owner = new User { UserName = "Owner" }
                    },
                };

                MockAppointmentRepo.Setup(r => r.GetById(id)).Returns(appointment);

                // Act
                // Assert
                await Assert.ThrowsAsync<NotBookedException>(() => Logic.UnBookAppointmentAsync(id, userName));
                MockAppointmentRepo.Verify(r => r.UpdateAsync(appointment), Times.Never());
            }
        }
    }
}
