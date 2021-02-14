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
        [Fact]
        public void TestGetAppointmentById()
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
        public void TestGetAppointmentByIdNotFound()
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
        public void TestGetAppointmentByIdUnauthorised()
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
}
