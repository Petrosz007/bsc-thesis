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
        private readonly IRepository Repository;

        public CategoryLogic(IRepository repository)
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
            var category = Repository.GetCategoryById(categoryId);
            var isOwner = category.Owner.UserName == userName;

            return isOwner;
        }

        public Category GetCategoryById(int id, string? userName)
        {
            var category = Repository.GetCategoryById(id);

            if (!HasReadAccess(category, userName))
                throw new UnauthorisedException($"You are unauthorized to view this category.");

            return category;
        }

        public async Task CreateCategory(Category category, string? userName)
        {
            await Repository.CreateCategory(category);
        }

        public async Task UpdateCategory(Category category, string? userName)
        {
            if (!HasWriteAccess(category.Id, userName))
                throw new UnauthorisedException("Unauthorised to update this appointment");

            await Repository.UpdateCategory(category);
        }

        public async Task DeleteCategory(int categoryId, string? userName)
        {
            var category = Repository.GetCategoryById(categoryId);

            if (!HasWriteAccess(category.Id, userName))
                throw new UnauthorisedException("Unauthorised to delete this category.");

            await Repository.DeleteCategory(category);
        }
    }
}
