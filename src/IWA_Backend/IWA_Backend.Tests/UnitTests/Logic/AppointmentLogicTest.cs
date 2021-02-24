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

namespace IWA_Backend.Tests.UnitTests
{
    public class AppointmentLogicTest
    {
        public class GetById
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

                var mockAppointmentRepo = new Mock<IAppointmentRepository>();
                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockAppointmentRepo.Setup(r => r.GetById(0)).Returns(appointment);
                var logic = new AppointmentLogic(mockAppointmentRepo.Object, mockCategoryRepo.Object);

                // Act
                var result = logic.GetAppointmentById(0, "TestUser");

                // Assert
                Assert.Equal(appointment, result);
            }

            [Fact]
            public void NotFound()
            {
                // Arrange
                var mockAppointmentRepo = new Mock<IAppointmentRepository>();
                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockAppointmentRepo.Setup(r => r.GetById(It.IsAny<int>())).Throws(new NotFoundException(""));
                var logic = new AppointmentLogic(mockAppointmentRepo.Object, mockCategoryRepo.Object);

                // Act
                // Assert
                Assert.Throws<NotFoundException>(() => logic.GetAppointmentById(0, "TestUser"));
            }

            [Fact]
            public void Unauthorised()
            {
                // Arrange
                var appointment = new Appointment
                {
                    Category = new Category { Owner = new User { UserName = "Owner User" } }
                };

                var mockAppointmentRepo = new Mock<IAppointmentRepository>();
                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockAppointmentRepo.Setup(r => r.GetById(0)).Returns(appointment);
                var logic = new AppointmentLogic(mockAppointmentRepo.Object, mockCategoryRepo.Object);

                // Act
                // Assert
                Assert.Throws<UnauthorisedException>(() => logic.GetAppointmentById(0, "Definitly not allowed user"));
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
                var mockAppointmentRepo = new Mock<IAppointmentRepository>();
                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockCategoryRepo.Setup(r => r.GetById(10)).Returns(category);
                var logic = new AppointmentLogic(mockAppointmentRepo.Object, mockCategoryRepo.Object);

                // Act
                var result = logic.HasWriteAccess(10, "OwnerUser");

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
                var mockAppointmentRepo = new Mock<IAppointmentRepository>();
                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockCategoryRepo.Setup(r => r.GetById(10)).Returns(category);
                var logic = new AppointmentLogic(mockAppointmentRepo.Object, mockCategoryRepo.Object);

                // Act
                var result = logic.HasWriteAccess(10, "Not allowed User");

                // Assert
                Assert.False(result);
            }
        }

        public class Update
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

                var mockAppointmentRepo = new Mock<IAppointmentRepository>();
                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockAppointmentRepo.Setup(r => r.Exists(10)).Returns(true);
                mockCategoryRepo.Setup(r => r.GetById(2)).Returns(appointment.Category);
                var logic = new AppointmentLogic(mockAppointmentRepo.Object, mockCategoryRepo.Object);

                // Act
                await logic.UpdateAppointmentAsync(appointment, "Owner");

                // Assert
                mockAppointmentRepo.Verify(r => r.UpdateAsync(appointment), Times.Once());
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

                var mockAppointmentRepo = new Mock<IAppointmentRepository>();
                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockAppointmentRepo.Setup(r => r.Exists(10)).Throws(new NotFoundException(""));
                mockCategoryRepo.Setup(r => r.GetById(2)).Returns(appointment.Category);
                var logic = new AppointmentLogic(mockAppointmentRepo.Object, mockCategoryRepo.Object);

                // Act
                // Assert
                await Assert.ThrowsAsync<NotFoundException>(() => logic.UpdateAppointmentAsync(appointment, "Owner"));

                mockAppointmentRepo.Verify(r => r.UpdateAsync(appointment), Times.Never());
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

                var mockAppointmentRepo = new Mock<IAppointmentRepository>();
                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockAppointmentRepo.Setup(r => r.Exists(10)).Returns(true);
                mockCategoryRepo.Setup(r => r.GetById(2)).Returns(appointment.Category);
                var logic = new AppointmentLogic(mockAppointmentRepo.Object, mockCategoryRepo.Object);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => logic.UpdateAppointmentAsync(appointment, "NotOwner"));

                mockAppointmentRepo.Verify(r => r.UpdateAsync(appointment), Times.Never());
            }
        }

        public class Create
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

                var mockAppointmentRepo = new Mock<IAppointmentRepository>();
                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockCategoryRepo.Setup(r => r.GetById(2)).Returns(appointment.Category);
                var logic = new AppointmentLogic(mockAppointmentRepo.Object, mockCategoryRepo.Object);

                // Act
                await logic.CreateAppointmentAsync(appointment, "Owner");

                // Assert
                mockAppointmentRepo.Verify(r => r.CreateAsync(appointment), Times.Once());
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

                var mockAppointmentRepo = new Mock<IAppointmentRepository>();
                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockCategoryRepo.Setup(r => r.GetById(2)).Returns(appointment.Category);
                var logic = new AppointmentLogic(mockAppointmentRepo.Object, mockCategoryRepo.Object);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => logic.CreateAppointmentAsync(appointment, "NotOwner"));

                mockAppointmentRepo.Verify(r => r.CreateAsync(appointment), Times.Never());
            }
        }

        public class Delete
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

                var mockAppointmentRepo = new Mock<IAppointmentRepository>();
                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockAppointmentRepo.Setup(r => r.GetById(10)).Returns(appointment);
                mockCategoryRepo.Setup(r => r.GetById(2)).Returns(appointment.Category);
                var logic = new AppointmentLogic(mockAppointmentRepo.Object, mockCategoryRepo.Object);

                // Act
                await logic.DeleteAppointmentAsync(10, "Owner");

                // Assert
                mockAppointmentRepo.Verify(r => r.DeleteAsync(appointment), Times.Once());
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

                var mockAppointmentRepo = new Mock<IAppointmentRepository>();
                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockAppointmentRepo.Setup(r => r.GetById(10)).Returns(appointment);
                mockCategoryRepo.Setup(r => r.GetById(2)).Returns(appointment.Category);
                var logic = new AppointmentLogic(mockAppointmentRepo.Object, mockCategoryRepo.Object);

                // Act
                // Assert
                await Assert.ThrowsAsync<UnauthorisedException>(() => logic.DeleteAppointmentAsync(10, "NotOwner"));

                mockAppointmentRepo.Verify(r => r.DeleteAsync(appointment), Times.Never());
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

                var mockAppointmentRepo = new Mock<IAppointmentRepository>();
                var mockCategoryRepo = new Mock<ICategoryRepository>();
                mockAppointmentRepo.Setup(r => r.GetById(100)).Throws(new NotFoundException(""));
                var logic = new AppointmentLogic(mockAppointmentRepo.Object, mockCategoryRepo.Object);

                // Act
                // Assert
                await Assert.ThrowsAsync<NotFoundException>(() => logic.DeleteAppointmentAsync(100, "Owner"));

                mockAppointmentRepo.Verify(r => r.DeleteAsync(appointment), Times.Never());
            }
        }
    }
}
