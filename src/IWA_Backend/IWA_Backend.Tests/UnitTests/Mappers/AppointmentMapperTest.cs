﻿using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Mappers;
using IWA_Backend.API.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IWA_Backend.Tests.UnitTests.Mappers
{
    public class AppointmentMapperTest
    {
        [Fact]
        public void ToDTO()
        {
            // Arrange
            var category = new Category { Id = 20 };
            var owner = new User { UserName = "owner" };
            var attendees = new List<User> { new User { UserName = "test1" }, new User { UserName = "test2" } };

            var mockRepo = new Mock<IRepository>();
            var mapper = new AppointmentMapper(mockRepo.Object);

            var data = new Appointment
            {
                Id = 10,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                Category = category,
                Attendees = attendees,
                MaxAttendees = 12,
            };

            // Act
            var result = mapper.ToDTO(data);

            // Assert
            Assert.Equal(data.Id, result.Id);
            Assert.Equal(data.StartTime, result.StartTime);
            Assert.Equal(data.EndTime, result.EndTime);
            Assert.Equal(data.Category.Id, result.CategoryId);
            Assert.Equal(data.MaxAttendees, result.MaxAttendees);

            var dataAttendees = data.Attendees.Select(a => a.UserName);
            Assert.True(dataAttendees.SequenceEqual(result.AttendeeUserNames));
        }

        [Fact]
        public void ToEntity()
        {
            // Arrange
            var category = new Category { Id = 20 };
            var owner = new User { UserName = "owner" };
            var attendees = new List<User> { new User { UserName = "test1" }, new User { UserName = "test2" } };

            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(r => r.GetCategoryById(20)).Returns(category);
            mockRepo.Setup(r => r.GetUserByUserName("owner")).Returns(owner);
            mockRepo.Setup(r => r.GetUserByUserName("test1")).Returns(attendees[0]);
            mockRepo.Setup(r => r.GetUserByUserName("test2")).Returns(attendees[1]);
            var mapper = new AppointmentMapper(mockRepo.Object);

            var data = new AppointmentDTO
            {
                Id = 10,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                CategoryId = category.Id,
                AttendeeUserNames = attendees.Select(a => a.UserName),
                MaxAttendees = 12,
            };

            // Act
            var result = mapper.ToEntity(data);

            // Assert
            Assert.Equal(data.Id, result.Id);
            Assert.Equal(data.StartTime, result.StartTime);
            Assert.Equal(data.EndTime, result.EndTime);
            Assert.Equal(data.CategoryId, result.Category.Id);
            Assert.Equal(data.MaxAttendees, result.MaxAttendees);

            var resultAttendees = result.Attendees.Select(a => a.UserName);
            Assert.True(data.AttendeeUserNames.SequenceEqual(resultAttendees));
        }
    }
}