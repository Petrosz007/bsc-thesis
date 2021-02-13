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
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ContractorPage> ContractorPages { get; set; }

        public IWAContext(DbContextOptions<IWAContext> options)
            : base(options)
        {
        }
    }
}
