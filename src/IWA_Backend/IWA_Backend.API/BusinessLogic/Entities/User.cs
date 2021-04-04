using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IWA_Backend.API.BusinessLogic.Entities
{
    public class User : IdentityUser<int>
    {
        public override string UserName { get; set; } = null!;
        public override string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public virtual ContractorPage? ContractorPage { get; set; } = null;

        public bool IsContractor { get => ContractorPage is not null; }

        public virtual List<Appointment> AttendeeOnAppointments { get; set; }  = null!;
        public virtual List<AttendeeOnAppointments> AttendeeOnAppointmentsJoin { get; set; }  = null!;
        public virtual List<Category> AllowedUserOnCategories { get; set; } = null!;
        public virtual List<AllowedUserOnCategories> AllowedUserOnCategoriesJoin { get; set; }  = null!;
        public virtual List<Category> OwnerOfCategories { get; set; } = null!;
    }

    public class AttendeeOnAppointments
    {
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
        public int AppointmentId { get; set; }
        public virtual Appointment Appointment { get; set; } = null!;
    }

    public class AllowedUserOnCategories
    {
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;
    }
}
