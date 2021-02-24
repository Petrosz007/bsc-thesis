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
        public byte[]? Avatar { get; set; } = null;
        public virtual ContractorPage? ContractorPage { get; set; } = null;

        public bool IsContractor { get => ContractorPage is not null; }
    }
}
