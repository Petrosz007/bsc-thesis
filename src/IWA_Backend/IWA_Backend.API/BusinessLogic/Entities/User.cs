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
        public override string UserName { get; set; } = "";
        public override string Email { get; set; } = "";
        public string Name { get; set; } = "";
        public byte[]? Avatar { get; set; }
        public ContractorPage? ContractorPage { get; set; }

        public bool IsContractor { get => ContractorPage is not null; }
    }
}
