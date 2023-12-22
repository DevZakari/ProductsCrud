using System;
using System.Threading.Tasks;
using CrudProducts.Controllers;
using CrudProducts.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CrudProducts.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly CategoriesController _categoriesController;

        public DeleteModel(CategoriesController categoriesController)
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

            // Utilisez le résultat de la méthode Delete dans le controller
            var result = await _categoriesController.Delete(id);

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
            if (Category.Id == 0)
            {
                return NotFound();
            }

            var result = await _categoriesController.DeleteConfirmed(Category.Id);

            if (result is RedirectToActionResult redirectResult)
            {
                // Si la suppression réussit, redirigez vers la page d'index
                return RedirectToPage("./Index");
            }

            // Si la suppression échoue, affichez la page actuelle avec le modèle actuel
            return Page();
        }


    }

}
