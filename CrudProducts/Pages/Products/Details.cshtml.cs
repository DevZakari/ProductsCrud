using System.Threading.Tasks;
using CrudProducts.Controllers;
using CrudProducts.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CrudProducts.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly ProductsController _productsController;

        public DetailsModel(ProductsController productsController)
        {
            _productsController = productsController;
        }

        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _productsController.Details(id);

            if (result is ViewResult viewResult)
            {
                Product = viewResult.Model as Product;
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
