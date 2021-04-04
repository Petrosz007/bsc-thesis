using IWA_Backend.API.BusinessLogic.DTOs;
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
            var data = new Appointment
            {
                Id = 10,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                Category = new Category { Id = 20 },
                Attendees = new List<User> { new() { UserName = "test1" }, new() { UserName = "test2" } },
                MaxAttendees = 12,
            };

            // Act
            var result = AppointmentMapper.ToDTO(data);

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

            var dto = new AppointmentDTO
            {
                Id = 10,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                CategoryId = category.Id,
                AttendeeUserNames = attendees.Select(a => a.UserName),
                MaxAttendees = 12,
            };

            // Act
            var result = AppointmentMapper.IntoEntity(dto, category, attendees);

            // Assert
            Assert.Equal(dto.Id, result.Id);
            Assert.Equal(dto.StartTime, result.StartTime);
            Assert.Equal(dto.EndTime, result.EndTime);
            Assert.Equal(dto.CategoryId, result.Category.Id);
            Assert.Equal(dto.MaxAttendees, result.MaxAttendees);

            var resultAttendees = result.Attendees.Select(a => a.UserName);
            Assert.True(dto.AttendeeUserNames.SequenceEqual(resultAttendees));
        }
    }
}
