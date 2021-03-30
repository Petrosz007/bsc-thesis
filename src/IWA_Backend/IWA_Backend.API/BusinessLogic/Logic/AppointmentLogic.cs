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
        private readonly IUserRepository UserRepository;
        public AppointmentLogic(IAppointmentRepository appointmentRepository, ICategoryRepository categoryRepository, IUserRepository userRepository)
        {
            AppointmentRepository = appointmentRepository;
            CategoryRepository = categoryRepository;
            UserRepository = userRepository;
        }

        public Appointment GetAppointmentById(int id, string? userName)
        {
            var appointment = AppointmentRepository.GetById(id);

            if(!HasReadAccess(appointment, userName))
                throw new UnauthorisedException($"You are unauthorized to view this appointment.");

            return appointment;
        }

        public IEnumerable<Appointment> GetContractorsAppointments(string contractorUserName, string? userName)
        {
            if (!UserRepository.Exists(contractorUserName))
                throw new NotFoundException($"Contractor with username {contractorUserName} not found.");

            var appointments = AppointmentRepository.GetContractorsAllAppointments(contractorUserName)
                .ToList()
                .Where(a => HasReadAccess(a, userName));
            return appointments;
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
            AppointmentRepository.GetBookedAppointments(currentUserName).ToList();

        public async Task BookAppointmentAsync(int appointmentId, string userName)
        {
            var appointment = AppointmentRepository.GetById(appointmentId);

            if (!HasReadAccess(appointment, userName))
                throw new UnauthorisedException("You are unauthorized to view this appointment.");

            if (appointment.Attendees.Any(u => u.UserName == userName))
                throw new AlreadyBookedException("Appointment already booked.");

            var user = UserRepository.GetByUserName(userName);
            appointment.Attendees.Add(user);

            await AppointmentRepository.UpdateAsync(appointment);
        }

        public async Task UnBookAppointmentAsync(int appointmentId, string userName)
        {
            var appointment = AppointmentRepository.GetById(appointmentId);

            if (!HasReadAccess(appointment, userName))
                throw new UnauthorisedException("You are unauthorized to view this appointment.");

            if (!appointment.Attendees.Any(u => u.UserName == userName))
                throw new NotBookedException("Appointment already booked.");

            appointment.Attendees.RemoveAll(u => u.UserName == userName);

            await AppointmentRepository.UpdateAsync(appointment);
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
