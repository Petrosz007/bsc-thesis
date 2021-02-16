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
#nullable disable
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ContractorPage> ContractorPages { get; set; }
# nullable enable
        public IWAContext() { }
        public IWAContext(DbContextOptions<IWAContext> options)
            : base(options)
        {
        }
    }
}
