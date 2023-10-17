using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrudProducts.Data;
using CrudProducts.Model;
using System.Text.Json;


namespace CrudProducts.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly CrudProducts.Data.CrudProductsContext _context;

        public IndexModel(CrudProducts.Data.CrudProductsContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get;set; }

        [BindProperty(SupportsGet = true)]
        public string prd { get; set; }
        [BindProperty(SupportsGet = true)]
        public decimal? minPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? maxPrice { get; set; }


        public async Task OnGetAsync()
        {
            IQueryable<Product> productsQuery = _context.Product;

            if (!string.IsNullOrEmpty(prd))
            {
                // Filter based on the search term
                productsQuery = productsQuery
                    .Where(p => p.Name.Contains(prd));
            }
            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            // Affectez les résultats filtrés à la propriété Product
            Product = await productsQuery.ToListAsync();
        }
    }
}

