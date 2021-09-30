using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Mappers;
using IWA_Backend.API.Repositories.Interfaces;

namespace IWA_Backend.API.BusinessLogic.Logic
{
    public class CategoryLogic
    {
        private readonly ICategoryRepository CategoryRepository;
        private readonly IUserRepository UserRepository;

        public CategoryLogic(ICategoryRepository categoryRepository, IUserRepository userRepository)
        {
            CategoryRepository = categoryRepository;
            UserRepository = userRepository;
        }

        public bool HasReadAccess(Category category, string? userName)
        {
            if (category.EveryoneAllowed) return true;
            if (category.Owner.UserName == userName) return true;
            if (category.AllowedUsers.Any(user => user.UserName == userName)) return true;
            if (CategoryRepository.IsUserInAnAppointmentOfACategory(category.Id, userName)) return true;
            
            return false;
        }

        public static bool HasWriteAccess(Category category, string? userName)
        {
            return category.Owner.UserName == userName;
        }

        public Category GetCategoryById(int id, string? userName)
        {
            var category = CategoryRepository.GetById(id);

            if (!HasReadAccess(category, userName))
                throw new UnauthorisedException("Nem vagy jogosult megtekinteni ezt a kategóriát.");

            return category;
        }

        public IEnumerable<Category> GetContractorsCategories(string contractorUserName, string? userName)
        {
            if (!UserRepository.Exists(contractorUserName))
                throw new NotFoundException($"Vállalkozó '{contractorUserName}' felhasználónévvel nem található.");

            var categories = CategoryRepository.GetUsersCategories(contractorUserName)
                .Where(category => HasReadAccess(category, userName));
            
            return categories;
        }

        public async Task<Category> CreateCategoryAsync(CategoryDTO categoryDto, string? userName)
        {
            var allowedUsers = categoryDto.AllowedUserNames
                .Select(UserRepository.GetByUserName);
            var owner = UserRepository.GetByUserName(userName);

            if (CategoryRepository.GetUsersCategories(userName).Any(c => c.Name == categoryDto.Name))
                throw new InvalidEntityException($"'{categoryDto.Name}' nevű kategória már létezik.");
            
            var category = CategoryMapper.IntoEntity(categoryDto, allowedUsers, owner);
            
            await CategoryRepository.CreateAsync(category);

            return category;
        }

        public async Task UpdateCategoryAsync(CategoryDTO categoryDto, string? userName)
        {
            var category = CategoryRepository.GetById(categoryDto.Id);
            var allowedUsers = categoryDto.AllowedUserNames
                .Select(UserRepository.GetByUserName);

            if (!HasWriteAccess(category, userName))
                throw new UnauthorisedException("Nincs jogosultsága frissíteni ezt a kategóriát.");
            
            if (CategoryRepository.GetUsersCategories(userName).Any(c => c.Name == categoryDto.Name && c.Id != categoryDto.Id))
                throw new InvalidEntityException($"'{categoryDto.Name}' nevű kategória már létezik.");

            CategoryMapper.OntoEntity(category, categoryDto, allowedUsers, category.Owner);
            
            await CategoryRepository.UpdateAsync(category);
        }
    }
}
