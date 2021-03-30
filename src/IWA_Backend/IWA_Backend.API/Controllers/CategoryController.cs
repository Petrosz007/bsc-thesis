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
    public class CategoryController : ControllerBase
    {
        private readonly CategoryLogic Logic;
        private readonly IDTOMapper<Category, CategoryDTO> Mapper;
        public CategoryController(CategoryLogic logic, IDTOMapper<Category, CategoryDTO> mapper)
        {
            Logic = logic;
            Mapper = mapper;
        }

        private string? CurrentUserName => User.Identity?.Name;

        [HttpGet("{id}")]
        public ActionResult<CategoryDTO> GetCategoryById(int id)
        {
            try
            {
                var category = Logic.GetCategoryById(id, CurrentUserName);
                var categoryDTO = Mapper.ToDTO(category);
                return Ok(categoryDTO);
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorisedException) { return Unauthorized(); }
        }

        [HttpGet("Contractor/{userName}")]
        public ActionResult<IEnumerable<CategoryDTO>> GetOwnCategories(string userName)
        {
            try
            {
                var categories = Logic.GetContractorsCategories(userName, CurrentUserName);
                var categoriesDTO = categories.Select(Mapper.ToDTO);
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
                var category = Mapper.ToEntity(dto);
                await Logic.CreateCategoryAsync(category, CurrentUserName);
                var createdCategory = Mapper.ToDTO(category);
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, createdCategory);
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
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

                var category = Mapper.ToEntity(categoryDTO);
                await Logic.UpdateCategoryAsync(category, CurrentUserName);
                return NoContent();
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorisedException) { return Unauthorized(); }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Contractor")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                await Logic.DeleteCategoryAsync(id, CurrentUserName);
                return NoContent();
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorisedException) { return Unauthorized(); }
        }
    }
}
