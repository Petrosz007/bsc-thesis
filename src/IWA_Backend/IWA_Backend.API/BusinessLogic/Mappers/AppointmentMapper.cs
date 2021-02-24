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
        private readonly ICategoryRepository CategoryRepository;
        private readonly IUserRepository UserRepository;
        public AppointmentMapper(ICategoryRepository categoryRepository, IUserRepository userRepository)
        {
            CategoryRepository = categoryRepository;
            UserRepository = userRepository;
        }

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
                Category = CategoryRepository.GetById(dto.CategoryId),
                Attendees = dto.AttendeeUserNames.Select(UserRepository.GetByUserName).ToList(),
                MaxAttendees = dto.MaxAttendees,
            };
    }
}
