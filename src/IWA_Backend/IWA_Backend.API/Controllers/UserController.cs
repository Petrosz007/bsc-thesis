using AutoMapper;
using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.BusinessLogic.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IWA_Backend.API.Repositories.Implementations;
using Microsoft.AspNetCore.Http;

namespace IWA_Backend.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserLogic Logic;
        private readonly IMapper Mapper;

        public UserController(UserLogic logic, IMapper mapper)
        {
            Logic = logic;
            Mapper = mapper;
        }
        private string? CurrentUserName => User.Identity?.Name;

        [HttpGet("Info/{userName}")]
        public ActionResult<UserInfoDTO> GetUserInfoByUserName(string userName)
        {
            try
            {
                var user = Logic.GetUserByUserName(userName);
                var userInfoDTO = Mapper.Map<UserInfoDTO>(user);
                return Ok(userInfoDTO);
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
        }
        
        [HttpGet("Avatar/{userName}")]
        public async Task<ActionResult<UserInfoDTO>> GetAvatar(string userName)
        {
            try
            {
                var (bytes, mimeType) = await Logic.GetAvatarAsync(userName);
                return new FileContentResult(bytes, mimeType);
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
        }
        
        [HttpPost("Avatar")]
        [Authorize(Roles = "Contractor")]
        [RequestSizeLimit(3_000_000)]
        public async Task<ActionResult<UserInfoDTO>> PostAvatar(IFormFile file)
        {
            try
            {
                await Logic.UpdateAvatar(file, CurrentUserName!);
                return Ok();
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidAvatarFileException ex) { return BadRequest(ex.Message); }
        }
        
        [HttpGet("Contractors")]
        public ActionResult<UserInfoDTO> GetContractors()
        {
            var contractors = Logic.GetContractors();
            var contractorDTOs = contractors.Select(c => Mapper.Map<UserInfoDTO>(c));
            return Ok(contractorDTOs);
        }
        
        [HttpGet("All")]
        public ActionResult<UserInfoDTO> GetAllUsers()
        {
            var users = Logic.GetAllUsers();
            var userDTOs = users.Select(c => Mapper.Map<UserInfoDTO>(c));
            return Ok(userDTOs);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> UpdateUser([FromBody] UserUpdateDTO userUpdateDTO)
        {
            await Logic.UpdateUserAsync(userUpdateDTO, CurrentUserName);
            return NoContent();
        }

        [HttpGet("Self")]
        public ActionResult<IsLoggedInDTO> GetSelf()
        {
            return Ok(new IsLoggedInDTO(CurrentUserName is not null, CurrentUserName));
        }
        
        [HttpGet("SelfInfo")]
        [Authorize]
        public ActionResult<IsLoggedInDTO> GetSelfInfo()
        {
            var user = Logic.GetUserByUserName(CurrentUserName!);
            var userDto = Mapper.Map<UserSelfInfoDTO>(user);
            return Ok(userDto);
        }
    }
}
