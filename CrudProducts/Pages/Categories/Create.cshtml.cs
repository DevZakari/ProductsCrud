using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CrudProducts.Data;
using CrudProducts.Model;
using CrudProducts.Controllers;

namespace CrudProducts.Pages.Categories
{
    

    public class CreateModel : PageModel
    {
        private readonly CategoriesController _categoriesController;

        public CreateModel(CategoriesController categoriesController)
        {
            _categoriesController = categoriesController;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Category Category { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _categoriesController.Create(Category);

            return RedirectToPage("./Index");
        }
    }

}
