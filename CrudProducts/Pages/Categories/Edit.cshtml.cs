using System.Threading.Tasks;
using CrudProducts.Controllers;
using CrudProducts.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CrudProducts.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly CategoriesController _categoriesController;

        public EditModel(CategoriesController categoriesController)
        {
            _categoriesController = categoriesController;
        }

        [BindProperty]
        public Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Utilisez le résultat de la méthode Edit dans le controller
            var result = await _categoriesController.Edit(id);

            // Vérifiez si le résultat est une vue, puis utilisez-le pour initialiser la propriété Category
            if (result is ViewResult viewResult)
            {
                Category = viewResult.Model as Category;
                return Page();
            }

            return result;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _categoriesController.Edit(Category.Id, Category);

            // Faites quelque chose avec le résultat si nécessaire

            return RedirectToPage("./Index");
        }

    }
}
