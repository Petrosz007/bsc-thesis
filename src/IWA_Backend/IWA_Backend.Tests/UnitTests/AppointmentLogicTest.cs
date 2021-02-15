using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.BusinessLogic.Logic;
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
        public class TestGetAppointmentById
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
                };

                var mockRepo = new Mock<IRepository>();
                mockRepo.Setup(r => r.GetAppointmentById(It.IsAny<int>())).Returns(appointment);
                var logic = new AppointmentLogic(mockRepo.Object);

                // Act
                var result = logic.GetAppointmentById(0, "TestUser");

                // Assert
                Assert.Equal(appointment, result);
            }

            [Fact]
            public void NotFound()
            {
                // Arrange
                var appointment = new Appointment();

                var mockRepo = new Mock<IRepository>();
                mockRepo.Setup(r => r.GetAppointmentById(It.IsAny<int>())).Returns((Appointment?)null);
                var logic = new AppointmentLogic(mockRepo.Object);

                // Act
                // Assert
                Assert.Throws<NotFoundException>(() => logic.GetAppointmentById(0, "TestUser"));
            }

            [Fact]
            public void Unauthorised()
            {
                // Arrange
                var appointment = new Appointment();

                var mockRepo = new Mock<IRepository>();
                mockRepo.Setup(r => r.GetAppointmentById(It.IsAny<int>())).Returns(appointment);
                var logic = new AppointmentLogic(mockRepo.Object);

                // Act
                // Assert
                Assert.Throws<UnauthorisedException>(() => logic.GetAppointmentById(0, "Definitly not allowed user"));
            }
        }

        public class TestHasAccess
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
                    },
                };

                // Act
                var result = AppointmentLogic.HasAccess(appointment, "Test User");

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
                    Owner = user,
                };

                // Act
                var result = AppointmentLogic.HasAccess(appointment, user.UserName);

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
                };

                // Act
                var result = AppointmentLogic.HasAccess(appointment, user.UserName);

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
                        AllowedCustomers = new List<User> { user },
                    },
                };

                // Act
                var result = AppointmentLogic.HasAccess(appointment, user.UserName);

                // Assert
                Assert.True(result);
            }

            [Fact]
            public void NullUserNotAllowed()
            {
                // Arrange
                var appointment = new Appointment();

                // Act
                var result = AppointmentLogic.HasAccess(appointment, null);

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
                    },
                };

                // Act
                var result = AppointmentLogic.HasAccess(appointment, null);

                // Assert
                Assert.True(result);
            }

            [Fact]
            public void NotAllowed()
            {
                // Arrange
                var appointment = new Appointment();

                // Act
                var result = AppointmentLogic.HasAccess(appointment, "Not allowed User");

                // Assert
                Assert.False(result);
            }
        }
    }
}
