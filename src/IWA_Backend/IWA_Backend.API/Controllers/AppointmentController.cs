using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.BusinessLogic.Logic;
using IWA_Backend.API.BusinessLogic.Mappers;
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
        private readonly IMapper<Appointment, AppointmentDTO> Mapper;
        public AppointmentController(AppointmentLogic logic, IMapper<Appointment, AppointmentDTO> mapper)
        {
            Logic = logic;
            Mapper = mapper;
        }

        private string? CurrentUserName { get => User.Identity?.Name; }

        [HttpGet("{id}")]
        public ActionResult<AppointmentDTO> GetAppointmentById(int id)
        {
            try
            {
                var asd = User.Identity?.Name;
                var appointment = Logic.GetAppointmentById(id, CurrentUserName);
                var appointmentDTO = Mapper.ToDTO(appointment);
                return Ok(appointmentDTO);
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorisedException) { return Unauthorized(); }
        }

        [HttpPost]
        [Authorize(Roles = "Contractor")]
        public async Task<ActionResult> CreateAppointment([FromBody] AppointmentDTO appointmentDTO)
        {
            try
            {
                var appointment = Mapper.ToEntity(appointmentDTO);
                await Logic.CreateAppointment(appointment, CurrentUserName);
                var createdAppointmentDTO = Mapper.ToDTO(appointment);
                return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, createdAppointmentDTO);
            }
            catch(NotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorisedException) { return Unauthorized(); }
            catch(InvalidEntityException ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Contractor")]
        public async Task<ActionResult> UpdateAppointment(int id, [FromBody] AppointmentDTO appointmentDTO)
        {
            try
            {
                if (id != appointmentDTO.Id)
                    return BadRequest($"URL path id '{id}' is not equal to the appointment id '{appointmentDTO.Id}'");

                var appointment = Mapper.ToEntity(appointmentDTO);
                await Logic.UpdateAppointment(appointment, CurrentUserName);
                return NoContent();
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorisedException) { return Unauthorized(); }
            catch (InvalidEntityException ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Contractor")]
        public async Task<ActionResult> DeleteAppointment(int id)
        {
            try
            {
                await Logic.DeleteAppointment(id, CurrentUserName);
                return NoContent();
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorisedException) { return Unauthorized(); }
            catch (InvalidEntityException ex) { return BadRequest(ex.Message); }
        }
    }
}
