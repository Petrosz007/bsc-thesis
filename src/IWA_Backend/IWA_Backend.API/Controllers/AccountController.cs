﻿using AutoMapper;
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
        private readonly IMapper Mapper;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, IMapper mapper)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            Mapper = mapper;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            var result = await signInManager.PasswordSignInAsync(login.UserName, login.Password, isPersistent: true, lockoutOnFailure: false);
            if(!result.Succeeded)
            {
                return BadRequest("Rossz felhasználónév vagy jelszó!");
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
                ContractorPage = Mapper.Map<ContractorPage>(register.ContractorPage),
            };

            var result = await userManager.CreateAsync(user, register.Password);
            if(!result.Succeeded)
            {
                if(result.Errors.Any())
                {
                    if(result.Errors.First().Code == "DuplicateUserName")
                    {
                        return BadRequest("Ez a felhasználónév már foglalt!");
                    }
                }

                return BadRequest(result.Errors);
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
