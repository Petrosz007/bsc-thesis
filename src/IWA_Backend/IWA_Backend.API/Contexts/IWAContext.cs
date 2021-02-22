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
}
