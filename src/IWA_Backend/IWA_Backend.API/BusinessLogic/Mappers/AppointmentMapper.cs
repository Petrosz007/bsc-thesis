using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.BusinessLogic.Mappers
{
    public class AppointmentMapper : IMapper<Appointment, AppointmentDTO>
    {
        private readonly IRepository Repository;
        public AppointmentMapper(IRepository repository) =>
            Repository = repository;

        public AppointmentDTO ToDTO(Appointment entity) =>
            new AppointmentDTO
            {
                Id = entity.Id,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                CategoryId = entity.Category.Id,
                AttendeeUserNames = entity.Attendees.Select(a => a.UserName),
                MaxAttendees = entity.MaxAttendees,
            };

        public Appointment ToEntity(AppointmentDTO dto) =>
            new Appointment
            {
                Id = dto.Id,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Category = Repository.GetCategoryById(dto.CategoryId),
                Attendees = dto.AttendeeUserNames.Select(Repository.GetUserByUserName).ToList(),
                MaxAttendees = dto.MaxAttendees,
            };
    }
}
