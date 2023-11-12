using BlogWebAPI.Data;
using BlogWebAPI.Models.DTOs;
using BlogWebAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogWebAPI.BusinessLayers
{
    public class CategoriesBusinessLayer
    {
        private readonly BlogDbContext dbContext;
        private readonly ToDto toDtos;

        public CategoriesBusinessLayer(BlogDbContext dbContext, ToDto toDto)
        {
            this.dbContext = dbContext;
            this.toDtos = toDto;
        }

        public async Task<bool> CreateCategory(string category)
        {
            var checkCategory = await dbContext.Categories.Where(c=>c.Name==category).FirstOrDefaultAsync();
            if (checkCategory != null)
            {
                return false;
            }

            var newCategory = new Category
            {
                Id = new Guid(),
                Name = category,
            };

            dbContext.Categories.Add(newCategory);
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EditCategory(string oldCategory, string newCategory)
        {
            var checkCategory = await dbContext.Categories.Where(c => c.Name == oldCategory).FirstOrDefaultAsync();
            if (checkCategory == null)
            {
                return false;
            }

            var checkIfNewCategoryExists = await dbContext.Categories.Where(c => c.Name == newCategory).FirstOrDefaultAsync();
            if (checkIfNewCategoryExists != null)
            {
                return false;
            }

            checkCategory.Name = newCategory;

            await dbContext.SaveChangesAsync();

            return true;
        }


        public async Task<List<string>> GetAllCategories()
        {
           var allCategories =  await dbContext.Categories.Select(c => c.Name).OrderBy(a => a).ToListAsync();

            return allCategories;
        }

        public async Task<bool?> DeleteCategory(string categoryToDelete)
        {
            var categ = await dbContext.Categories.Where(q => q.Name == categoryToDelete).FirstOrDefaultAsync();

            if (categ == null)
            {
                return null;
            }

            dbContext.Categories.Remove(categ);
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
