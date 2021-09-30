using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.Repositories;
using IWA_Backend.API.BusinessLogic.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Mappers;
using IWA_Backend.API.Repositories.Interfaces;

namespace IWA_Backend.API.BusinessLogic.Logic
{
    public class AppointmentLogic
    {
        private readonly IAppointmentRepository AppointmentRepository;
        private readonly ICategoryRepository CategoryRepository;
        private readonly IUserRepository UserRepository;
        public AppointmentLogic(IAppointmentRepository appointmentRepository, ICategoryRepository categoryRepository, IUserRepository userRepository)
        {
            AppointmentRepository = appointmentRepository;
            CategoryRepository = categoryRepository;
            UserRepository = userRepository;
        }
        
        public static bool HasReadAccess(Appointment appointment, string? userName)
        {
            if(appointment.Category.EveryoneAllowed) return true;
            if(appointment.Category.Owner.UserName == userName) return true;
            if(appointment.Attendees.Any(user => user.UserName == userName)) return true;
            if(appointment.Category.AllowedUsers.Any(user => user.UserName == userName)) return true;

            return false;
        }
        
        public static bool HasWriteAccess(Category category, string? userName)
        {
            if (category.Owner.UserName == userName) return true;

            return false;
        }
        
        public Appointment GetAppointmentById(int id, string? userName)
        {
            var appointment = AppointmentRepository.GetById(id);

            if(!HasReadAccess(appointment, userName))
                throw new UnauthorisedException("Nem vagy jogosult megtekinteni ezt az időpontot.");

            return appointment;
        }

        public IEnumerable<Appointment> GetContractorsAppointments(string contractorUserName, string? userName)
        {
            if (!UserRepository.Exists(contractorUserName))
                throw new NotFoundException($"Vállalkozó '{contractorUserName}' felhasználónévvel nem található.");

            var appointments = AppointmentRepository.GetContractorsAllAppointments(contractorUserName)
                .Where(a => HasReadAccess(a, userName));
            return appointments;
        }

        public IEnumerable<Appointment> GetBookedAppointments(string currentUserName) =>
            AppointmentRepository.GetBookedAppointments(currentUserName);

        public async Task BookAppointmentAsync(int appointmentId, string userName)
        {
            var appointment = AppointmentRepository.GetById(appointmentId);

            if (!HasReadAccess(appointment, userName))
                throw new UnauthorisedException("Nem vagy jogosult megtekinteni ezt az időpontot.");

            if (appointment.Attendees.Any(u => u.UserName == userName))
                throw new AlreadyBookedException("Már lefoglaltad ezt az időpontot.");

            if (appointment.Attendees.Count >= appointment.MaxAttendees)
                throw new InvalidOperationException("Ez az időpont már betelt, nem lehet foglalni rá.");

            var user = UserRepository.GetByUserName(userName);
            appointment.Attendees.Add(user);

            await AppointmentRepository.UpdateAsync(appointment);
        }

        public async Task UnBookAppointmentAsync(int appointmentId, string userName)
        {
            var appointment = AppointmentRepository.GetById(appointmentId);

            if (!HasReadAccess(appointment, userName))
                throw new UnauthorisedException("Nem vagy jogosult megtekinteni ezt az időpontot.");

            if (appointment.Attendees.All(u => u.UserName != userName))
                throw new NotBookedException("Még nem foglaltad le ezt az időpontot.");

            appointment.Attendees.RemoveAll(u => u.UserName == userName);

            await AppointmentRepository.UpdateAsync(appointment);
        }

        public async Task<Appointment> CreateAppointmentAsync(AppointmentDTO appointmentDto, string? userName)
        {
            var category = CategoryRepository.GetById(appointmentDto.CategoryId);
            var attendees = appointmentDto.AttendeeUserNames
                .Select(UserRepository.GetByUserName);
            
            if (!HasWriteAccess(category, userName))
                throw new UnauthorisedException("Nem vagy jogosult létrehozni ezt az időpontot.");
            
            var appointment = AppointmentMapper.IntoEntity(appointmentDto, category, attendees);
            
            await AppointmentRepository.CreateAsync(appointment);

            return appointment;
        }

        public async Task UpdateAppointmentAsync(AppointmentDTO appointmentDto, string? userName)
        {
            var appointment = AppointmentRepository.GetById(appointmentDto.Id);
            var newCategory = CategoryRepository.GetById(appointmentDto.CategoryId);
            var attendees = appointmentDto.AttendeeUserNames
                .Select(UserRepository.GetByUserName);
            
            if (!HasWriteAccess(appointment.Category, userName) || !HasWriteAccess(newCategory, userName))
                throw new UnauthorisedException("Nem vagy jogosult frissíteni ezt az időpontot.");

            AppointmentMapper.OntoEntity(appointment, appointmentDto, newCategory, attendees);
            
            await AppointmentRepository.UpdateAsync(appointment);
        }

        public async Task DeleteAppointmentAsync(int appointmentId, string? userName)
        {
            var appointment = AppointmentRepository.GetById(appointmentId);

            if (!HasWriteAccess(appointment.Category, userName))
                throw new UnauthorisedException("Nem vagy jogosult törölni ezt az időpontot.");

            await AppointmentRepository.DeleteAsync(appointment);
        }
    }
}
