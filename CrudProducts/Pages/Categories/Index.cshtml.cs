using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CrudProducts.Controllers;
using CrudProducts.Model;

namespace CrudProducts.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly CategoriesController _categoriesController;

        public IndexModel(CategoriesController categoriesController)
        {
            _categoriesController = categoriesController;
        }

        public IList<Category> Category { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var result = await _categoriesController.Index();
            if (result is ViewResult viewResult)
            {
                Category = viewResult.Model as List<Category>;
            }

            return Page();
        }
    }
}
