using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrudProducts.Data;
using CrudProducts.Model;
using Microsoft.AspNetCore.Mvc;
using CrudProducts.Controllers;
using System;
using Microsoft.AspNetCore.Authorization;

namespace CrudProducts.Pages.Products
{

    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ProductsController _productsController;

        public IndexModel(ProductsController productsController)
        {
            _productsController = productsController;
        }

        public IList<Product> Product { get; set; }
        public IList<Category> Categories { get; set; }

        [BindProperty(SupportsGet = true)]
        public string prd { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? minPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? maxPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public string selectedCategory { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var result = await _productsController.Index();

            if (result is ViewResult viewResult)
            {
                Product = viewResult.Model as List<Product>;

                var categoriesResult = await _productsController.GetCategories();

                if (categoriesResult != null)
                {
                    Categories = categoriesResult as List<Category>;
                }
                else
                {
                    Console.WriteLine("Categories result is null.");
                }
            }
            else
            {
                Console.WriteLine($"Unexpected result type for products: {result?.GetType().FullName}");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostFilterAsync()
        {
            // Appel de la méthode de filtrage du contrôleur avec les paramètres de filtrage
            var result = await _productsController.FilterProducts(prd, minPrice, maxPrice, selectedCategory);

            if (result is OkObjectResult okObjectResult)
            {
                // Mettez à jour seulement la variable Product avec les résultats du filtrage
                Product = okObjectResult.Value as List<Product>;
            }

            // Chargez les catégories indépendamment de l'opération de filtrage
            Categories = await _productsController.GetCategories();

            return Page();
        }





    }


}
