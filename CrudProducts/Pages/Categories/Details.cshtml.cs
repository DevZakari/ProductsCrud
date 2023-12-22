using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrudProducts.Data;
using CrudProducts.Model;
using CrudProducts.Controllers;

namespace CrudProducts.Pages.Categories
{
    public class DetailsModel : PageModel
    {
        private readonly CategoriesController _categoriesController;

        public DetailsModel(CategoriesController categoriesController)
        {
            _categoriesController = categoriesController;
        }

        public Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Utilisez le résultat de la méthode Details dans le controller
            var result = await _categoriesController.Details(id);

            // Vérifiez si le résultat est une vue, puis utilisez-le pour initialiser la propriété Category
            if (result is ViewResult viewResult)
            {
                Category = viewResult.Model as Category;
                return Page();
            }

            return result;
        }
    }
}
