using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Mappers;
using IWA_Backend.API.Contexts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWA_Backend.Tests.IntegrationTests
{
    public class IntegrationTestBase
    {
        protected readonly TestWebApplicationFactory<TestStartup> Factory = new();
        protected IWAContext Context =>
            Factory.Services.GetRequiredService<IWAContext>();
        protected IMapper<Appointment, AppointmentDTO> AppointmentMapper =>
            Factory.Services.GetRequiredService<IMapper<Appointment, AppointmentDTO>>();

        protected IMapper<Category, CategoryDTO> CategoryMapper =>
            Factory.Services.GetRequiredService<IMapper<Category, CategoryDTO>>();
    }
}
