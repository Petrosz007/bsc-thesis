using IWA_Backend.API.BusinessLogic.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }

    public static class IWAContextExtensions
    {
        public static void DetachLocal<T>(this DbContext context, T t)
            where T : class
        {
            static object? GetId(T t) => typeof(T).GetProperty("Id")?.GetValue(t);

            var local = context.Set<T>()?
                .Local?
                .FirstOrDefault(entry => GetId(entry)?.Equals(GetId(t)) ?? false);

            if (local is not null)
            {
                context.Entry(local).State = EntityState.Detached;
            }
        }
    }
}
