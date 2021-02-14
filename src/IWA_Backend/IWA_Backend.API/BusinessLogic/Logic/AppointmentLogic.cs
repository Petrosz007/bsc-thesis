using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.Repositories;
using IWA_Backend.API.BusinessLogic.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.BusinessLogic.Logic
{
    public class AppointmentLogic
    {
        private readonly IRepository Repository;
        public AppointmentLogic(IRepository repository) =>
            Repository = repository;

        public Appointment GetAppointmentById(int id, string? userName)
        {
            var appointment = Repository.GetAppointmentById(id)
                ?? throw new NotFoundException($"Appointment with id '{id}' not found.");

            if(!HasAccess(appointment, userName))
            {
                throw new UnauthorisedException($"You are unauthorized to view this appointment.");
            }

            return appointment;
        }

        public static bool HasAccess(Appointment appointment, string? userName)
        {
            bool everyoneAllowed = appointment.Category.EveryoneAllowed;
            // TODO: If owner.UserName == null and userName is null this condition is true
            // Should not happen, because every user is registered with one
            bool isOwner = appointment.Owner.UserName == userName;
            bool isAttendee = appointment.Attendees.Any(user => user.UserName == userName);
            bool isInCategory = appointment.Category.AllowedCustomers.Any(user => user.UserName == userName);

            return everyoneAllowed || isOwner || isAttendee || isInCategory ;
        }
    }
}
