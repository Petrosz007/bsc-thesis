using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.BusinessLogic.Mappers
{
    public static class AppointmentMapper
    {
        public static Appointment OntoEntity(Appointment entity, AppointmentDTO dto, Category category, IEnumerable<User> attendees)
        {
            entity.Id = dto.Id;
            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;
            entity.Category = category;
            entity.MaxAttendees = dto.MaxAttendees;

            entity.Attendees.Clear();
            entity.Attendees.AddRange(attendees);

            return entity;
        }

        public static Appointment IntoEntity(AppointmentDTO dto, Category category, IEnumerable<User> attendees) =>
            OntoEntity(new(), dto, category, attendees);
        
        public static AppointmentDTO ToDTO(Appointment entity) =>
            new()
            {
                Id = entity.Id,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                CategoryId = entity.Category.Id,
                AttendeeUserNames = entity.Attendees.Select(a => a.UserName),
                MaxAttendees = entity.MaxAttendees,
            };

    }
}
