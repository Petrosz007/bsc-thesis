﻿using IWA_Backend.API.BusinessLogic.DTOs;
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
using AutoMapper;

namespace IWA_Backend.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryLogic Logic;
        public CategoryController(CategoryLogic logic)
        {
            Logic = logic;
        }

        private string? CurrentUserName => User.Identity?.Name;

        [HttpGet("{id}")]
        public ActionResult<CategoryDTO> GetCategoryById(int id)
        {
            try
            {
                var category = Logic.GetCategoryById(id, CurrentUserName);
                var categoryDTO = CategoryMapper.ToDTO(category);
                return Ok(categoryDTO);
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorisedException) { return Unauthorized(); }
        }

        [HttpGet("Contractor/{userName}")]
        public ActionResult<IEnumerable<CategoryDTO>> GetContractorsCategories(string userName)
        {
            try
            {
                var categories = Logic.GetContractorsCategories(userName, CurrentUserName);
                var categoriesDTO = categories.Select(CategoryMapper.ToDTO);
                return Ok(categoriesDTO);
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
        }

        [HttpPost]
        [Authorize(Roles = "Contractor")]
        public async Task<ActionResult> CreateCategory([FromBody] CategoryDTO categoryDTO)
        {
            try
            {
                var dto = categoryDTO with
                {
                    Id = 0,
                    OwnerUserName = CurrentUserName!,
                };
                
                var category = await Logic.CreateCategoryAsync(dto, CurrentUserName);
                var createdCategory = CategoryMapper.ToDTO(category);
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, createdCategory);
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch(InvalidEntityException ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Contractor")]
        public async Task<ActionResult> UpdateCategory(int id, [FromBody] CategoryDTO categoryDTO)
        {
            try
            {
                var dto = categoryDTO with
                {
                    Id = id,
                    OwnerUserName = CurrentUserName!,
                };

                await Logic.UpdateCategoryAsync(dto, CurrentUserName);
                return NoContent();
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorisedException) { return Unauthorized(); }
            catch(InvalidEntityException ex) { return BadRequest(ex.Message); }
        }
    }
}
