using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public static bool HasReadAccess(Category category, string? userName)
        {
            bool everyoneAllowed = category.EveryoneAllowed;
            // TODO: If owner.UserName == null and userName is null this condition is true
            // Should not happen, because every user is registered with one
            bool isOwner = category.Owner.UserName == userName;
            bool isInCategory = category.AllowedUsers.Any(user => user.UserName == userName);

            return everyoneAllowed || isOwner || isInCategory;
        }

        public bool HasWriteAccess(int categoryId, string? userName)
        {
            var category = CategoryRepository.GetById(categoryId);
            var isOwner = category.Owner.UserName == userName;

            return isOwner;
        }

        public Category GetCategoryById(int id, string? userName)
        {
            var category = CategoryRepository.GetById(id);

            if (!HasReadAccess(category, userName))
                throw new UnauthorisedException($"You are unauthorized to view this category.");

            return category;
        }

        public IEnumerable<Category> GetContractorsCategories(string contractorUserName, string? userName)
        {
            if (!UserRepository.Exists(contractorUserName))
                throw new NotFoundException($"Contractor with username {contractorUserName} not found.");

            var categories = CategoryRepository.GetUsersCategories(contractorUserName)
                .ToList()
                .Where(category => HasReadAccess(category, userName));
            return categories;
        }

        public async Task CreateCategoryAsync(Category category, string? userName)
        {
            await CategoryRepository.CreateAsync(category);
        }

        public async Task UpdateCategoryAsync(Category category, string? userName)
        {
            if (!HasWriteAccess(category.Id, userName))
                throw new UnauthorisedException("Unauthorised to update this appointment");

            await CategoryRepository.UpdateAsync(category);
        }

        public async Task DeleteCategoryAsync(int categoryId, string? userName)
        {
            var category = CategoryRepository.GetById(categoryId);

            if (!HasWriteAccess(category.Id, userName))
                throw new UnauthorisedException("Unauthorised to delete this category.");

            await CategoryRepository.DeleteAsync(category);
        }
    }
}
