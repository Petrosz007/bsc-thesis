﻿using IWA_Backend.API.BusinessLogic.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IWA_Backend.API.Contexts
{
    public class IWAContext : IdentityDbContext<User, UserRole, int>
    {
        public virtual DbSet<Appointment> Appointments { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<ContractorPage> ContractorPages { get; set; } = null!;

        public IWAContext() { }
        public IWAContext(DbContextOptions<IWAContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Appointment>()
                .HasMany(a => a.Attendees)
                .WithMany(u => u.AttendeeOnAppointments)
                .UsingEntity<AttendeeOnAppointments>(
                    j => j.HasOne(x => x.User)
                        .WithMany(x => x.AttendeeOnAppointmentsJoin)
                        .HasForeignKey(x => x.UserId),
                    j => j.HasOne(x => x.Appointment)
                        .WithMany(x => x.AttendeeOnAppointmentsJoin)
                        .HasForeignKey(x => x.AppointmentId),
                    j => {
                        j.HasKey(x => new { x.AppointmentId, x.UserId });
                        j.ToTable("AttendeeOnAppointments");
                    }
                );

            builder.Entity<Category>()
                .HasMany(c => c.AllowedUsers)
                .WithMany(u => u.AllowedUserOnCategories)
                .UsingEntity<AllowedUserOnCategories>(
                    j => j.HasOne(x => x.User)
                        .WithMany(x => x.AllowedUserOnCategoriesJoin)
                        .HasForeignKey(x => x.UserId),
                    j => j.HasOne(x => x.Category)
                        .WithMany(x => x.AllowedUserOnCategoriesJoin)
                        .HasForeignKey(x => x.CategoryId),
                    j =>
                    {
                        j.HasKey(x => new { x.CategoryId, x.UserId });
                        j.ToTable("AllowedUserOnCategories");
                    }
                );

            builder.Entity<Category>()
                .HasOne(c => c.Owner)
                .WithMany(u => u.OwnerOfCategories);
        }
    }
}
