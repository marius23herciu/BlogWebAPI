using BlogWebAPI.BusinessLayers;
using BlogWebAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoriesBusinessLayer categoriesBusinessLayer;

        public CategoriesController(CategoriesBusinessLayer categoriesBusinessLayer)
        {
            this.categoriesBusinessLayer = categoriesBusinessLayer;
        }



        [HttpPost("{newCategory}")]
        public async Task<ActionResult<bool>> CreateCategory([FromRoute] string newCategory)
        {
            return Ok(await categoriesBusinessLayer.CreateCategory(newCategory));
        }

        [HttpPut]
        [Route("edit-{categToEdit}-to-{newCategory}")]
        public async Task<ActionResult<bool>> EditCategory([FromRoute] string categToEdit,[FromRoute] string newCategory)
        {
            return Ok(await categoriesBusinessLayer.EditCategory(categToEdit, newCategory));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            return Ok(await categoriesBusinessLayer.GetAllCategories());
        }

        [HttpDelete]
        [Route("{categoryToDelete}")]
        public async Task<ActionResult<bool>> DeleteCategory([FromRoute] string categoryToDelete)
        {
            var categoryDeleted = await categoriesBusinessLayer.DeleteCategory(categoryToDelete);
            if (categoryDeleted == null)
            {
                return NotFound("Category not found");
            }
            return Ok(categoryDeleted);
        }
    }
}
