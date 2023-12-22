using System.Threading.Tasks;
using CrudProducts.Controllers;
using CrudProducts.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;

namespace CrudProducts.Pages.Products
{
    public class DeleteModel : PageModel
    {
        private readonly ProductsController _productsController;

        public DeleteModel(ProductsController productsController)
        {
            _productsController = productsController;
        }

        [BindProperty]
        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _productsController.Delete(id);

            if (result is ViewResult viewResult)
            {
                Product = viewResult.Model as Product;
                return Page();
            }

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Product.Id == 0)
            {
                return NotFound();
            }
            var result = await _productsController.DeleteConfirmed(Product.Id);


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
