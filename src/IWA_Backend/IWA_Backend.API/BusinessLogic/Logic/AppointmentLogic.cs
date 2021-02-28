using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.Repositories;
using IWA_Backend.API.BusinessLogic.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Mappers;

namespace IWA_Backend.API.BusinessLogic.Logic
{
    public class AppointmentLogic
    {
        private readonly IAppointmentRepository AppointmentRepository;
        private readonly ICategoryRepository CategoryRepository;
        public AppointmentLogic(IAppointmentRepository appointmentRepository, ICategoryRepository categoryRepository)
        {
            AppointmentRepository = appointmentRepository;
            CategoryRepository = categoryRepository;
        }

        public Appointment GetAppointmentById(int id, string? userName)
        {
            var appointment = AppointmentRepository.GetById(id);

            if(!HasReadAccess(appointment, userName))
                throw new UnauthorisedException($"You are unauthorized to view this appointment.");

            return appointment;
        }

        public static bool HasReadAccess(Appointment appointment, string? userName)
        {
            bool everyoneAllowed = appointment.Category.EveryoneAllowed;
            // TODO: If owner.UserName == null and userName is null this condition is true
            // Should not happen, because every user is registered with one
            bool isOwner = appointment.Category.Owner.UserName == userName;
            bool isAttendee = appointment.Attendees.Any(user => user.UserName == userName);
            bool isInCategory = appointment.Category.AllowedUsers.Any(user => user.UserName == userName);

            return everyoneAllowed || isOwner || isAttendee || isInCategory;
        }

        public IEnumerable<Appointment> GetBookedAppointments(string currentUserName) =>
            AppointmentRepository.GetBookedAppointments(currentUserName);

        public async Task BookAppointmentAsync(int appointmentId, string userName)
        {
            var appointment = AppointmentRepository.GetById(appointmentId);

            if (!HasReadAccess(appointment, userName))
                throw new UnauthorisedException("You are unauthorized to view this appointment.");

            if (appointment.Attendees.Any(u => u.UserName == userName))
                throw new AlreadyBookedException("Appointment already booked.");

            await AppointmentRepository.BookAppointmentAsync(appointment, userName);
        }

        public async Task UnBookAppointmentAsync(int appointmentId, string userName)
        {
            var appointment = AppointmentRepository.GetById(appointmentId);

            if (!HasReadAccess(appointment, userName))
                throw new UnauthorisedException("You are unauthorized to view this appointment.");

            if (!appointment.Attendees.Any(u => u.UserName == userName))
                throw new NotBookedException("Appointment already booked.");

            await AppointmentRepository.UnBookAppointmentAsync(appointment, userName);
        }

        public bool HasWriteAccess(int categoryId, string? userName)
        {
            var category = CategoryRepository.GetById(categoryId);
            var isOwner = category.Owner.UserName == userName;

            return isOwner;
        }

        public async Task CreateAppointmentAsync(Appointment appointment, string? userName)
        {
            if (!HasWriteAccess(appointment.Category.Id, userName))
                throw new UnauthorisedException("Unauthorised to create this appointment.");

            await AppointmentRepository.CreateAsync(appointment);
        }

        public async Task UpdateAppointmentAsync(Appointment appointment, string? userName)
        {
            if (!HasWriteAccess(appointment.Category.Id, userName))
                throw new UnauthorisedException("Unauthorised to update this appointment");

            if (!AppointmentRepository.Exists(appointment.Id))
                throw new NotFoundException($"Appointment with id '{appointment.Id}' doesn't exist.");

            await AppointmentRepository.UpdateAsync(appointment);
        }

        public async Task DeleteAppointmentAsync(int appointmentId, string? userName)
        {
            var appointment = AppointmentRepository.GetById(appointmentId);

            if (!HasWriteAccess(appointment.Category.Id, userName))
                throw new UnauthorisedException("Unauthorised to delete this appointment");

            await AppointmentRepository.DeleteAsync(appointment);
        }
    }
}
