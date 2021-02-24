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
        private readonly ICategoryRepository Repository;

        public CategoryLogic(ICategoryRepository repository)
        {
            Repository = repository;
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
            var category = Repository.GetById(categoryId);
            var isOwner = category.Owner.UserName == userName;

            return isOwner;
        }

        public Category GetCategoryById(int id, string? userName)
        {
            var category = Repository.GetById(id);

            if (!HasReadAccess(category, userName))
                throw new UnauthorisedException($"You are unauthorized to view this category.");

            return category;
        }

        public async Task CreateCategoryAsync(Category category, string? userName)
        {
            await Repository.CreateAsync(category);
        }

        public async Task UpdateCategoryAsync(Category category, string? userName)
        {
            if (!HasWriteAccess(category.Id, userName))
                throw new UnauthorisedException("Unauthorised to update this appointment");

            await Repository.UpdateAsync(category);
        }

        public async Task DeleteCategoryAsync(int categoryId, string? userName)
        {
            var category = Repository.GetById(categoryId);

            if (!HasWriteAccess(category.Id, userName))
                throw new UnauthorisedException("Unauthorised to delete this category.");

            await Repository.DeleteAsync(category);
        }
    }
}
