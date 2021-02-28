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
    [Route("[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentLogic Logic;
        private readonly IDTOMapper<Appointment, AppointmentDTO> Mapper;
        public AppointmentController(AppointmentLogic logic, IDTOMapper<Appointment, AppointmentDTO> mapper)
        {
            Logic = logic;
            Mapper = mapper;
        }

        private string? CurrentUserName => User.Identity?.Name;

        [HttpGet("{id}")]
        public ActionResult<AppointmentDTO> GetAppointmentById(int id)
        {
            try
            {
                var appointment = Logic.GetAppointmentById(id, CurrentUserName);
                var appointmentDTO = Mapper.ToDTO(appointment);
                return Ok(appointmentDTO);
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorisedException) { return Unauthorized(); }
        }

        [HttpPost("{id}/Book")]
        [Authorize]
        public async Task<ActionResult> BookAppointment(int id)
        {
            try
            {
                await Logic.BookAppointmentAsync(id, CurrentUserName!);
                return NoContent();
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorisedException) { return Unauthorized(); }
            catch (AlreadyBookedException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{id}/UnBook")]
        [Authorize]
        public async Task<ActionResult> UnBookAppointment(int id)
        {
            try
            {
                await Logic.UnBookAppointmentAsync(id, CurrentUserName!);
                return NoContent();
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorisedException) { return Unauthorized(); }
            catch (NotBookedException ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("Booked")]
        [Authorize]
        public ActionResult<IEnumerable<AppointmentDTO>> GetBookedAppointments()
        {
            var appointments = Logic.GetBookedAppointments(CurrentUserName!);
            var appointmentDTOs = appointments.Select(Mapper.ToDTO);
            return Ok(appointmentDTOs);
        }

        [HttpPost]
        [Authorize(Roles = "Contractor")]
        public async Task<ActionResult> CreateAppointment([FromBody] AppointmentDTO appointmentDTO)
        {
            try
            {
                var appointment = Mapper.ToEntity(appointmentDTO);
                await Logic.CreateAppointmentAsync(appointment, CurrentUserName);
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
                await Logic.UpdateAppointmentAsync(appointment, CurrentUserName);
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
                await Logic.DeleteAppointmentAsync(id, CurrentUserName);
                return NoContent();
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorisedException) { return Unauthorized(); }
            catch (InvalidEntityException ex) { return BadRequest(ex.Message); }
        }
    }
}
