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
        private readonly IRepository Repository;
        public AppointmentLogic(IRepository repository)
        {
            Repository = repository;
        }

        public Appointment GetAppointmentById(int id, string? userName)
        {
            var appointment = Repository.GetAppointmentById(id);

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

            return everyoneAllowed || isOwner || isAttendee || isInCategory ;
        }

        public bool HasWriteAccess(int categoryId, string? userName)
        {
            var category = Repository.GetCategoryById(categoryId);
            var isOwner = category.Owner.UserName == userName;

            return isOwner;
        }

        public async Task CreateAppointment(Appointment appointment, string? userName)
        {
            if (!HasWriteAccess(appointment.Category.Id, userName))
                throw new UnauthorisedException("Unauthorised to create this appointment.");

            //if(!IsValid(appointment))
            //    throw new InvalidEntityException($"Appointment is not valid.");

            await Repository.CreateAppointment(appointment);
        }

        //public bool IsValid(Appointment appointment)
        //{
        //    // TODO: Fix
        //    return true;
        //}

        public async Task UpdateAppointment(Appointment appointment, string? userName)
        {
            if (!HasWriteAccess(appointment.Category.Id, userName))
                throw new UnauthorisedException("Unauthorised to update this appointment");

            if (!Repository.AppointmentExists(appointment.Id))
                throw new NotFoundException($"Appointment with id '{appointment.Id}' doesn't exist.");

            await Repository.UpdateAppointment(appointment);
        }

        public async Task DeleteAppointment(int appointmentId, string? userName)
        {
            var appointment = Repository.GetAppointmentById(appointmentId);

            if (!HasWriteAccess(appointment.Category.Id, userName))
                throw new UnauthorisedException("Unauthorised to delete this appointment");

            await Repository.DeleteAppointment(appointment);
        }
    }
}
