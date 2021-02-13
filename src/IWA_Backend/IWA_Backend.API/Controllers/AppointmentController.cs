using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.BusinessLogic.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IWA_Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentLogic Logic;
        public AppointmentController(AppointmentLogic logic)
        {
            Logic = logic;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointmentById(int id)
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                var appointment = Logic.GetAppointmentById(id, userName);
                return Ok(appointment);
            }
            catch (NotFoundException) { return NotFound(); }
            catch (UnauthorisedException) { return NotFound(); }
        }
    }
}
