using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            var result = await signInManager.PasswordSignInAsync(login.UserName, login.Password, isPersistent: true, lockoutOnFailure: false);
            if(!result.Succeeded)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            var user = new User
            {
                Name = register.Name,
                UserName = register.UserName,
                Email = register.Email,
                // TODO: Add avatar support
                Avatar = null,
                // TODO: Add contractor page support
                ContractorPage = null,
            };

            var result = await userManager.CreateAsync(user, register.Password);
            if(!result.Succeeded)
            {
                // TODO: Better error handling
                if(result.Errors.Count() > 0)
                {
                    if(result.Errors.First().Code == "DuplicateUserName")
                    {
                        return BadRequest("Username already in use!");
                    }
                }

                return StatusCode(500);
            }

            if(user.ContractorPage is not null)
            {
                await userManager.AddToRoleAsync(user, "Contractor");
            }

            return NoContent();
        }

        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok();
        }
    }
}
