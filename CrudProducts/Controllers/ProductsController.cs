using CrudProducts.Data;
using CrudProducts.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrudProducts.Controllers
{
    public class ProductsController : Controller
    {
        private readonly CrudProductsContext _context;
        private readonly IMemoryCache _memoryCache;

        public ProductsController(CrudProductsContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        [NonAction]
        public async Task<List<Category>> GetCategories()
        {
            return await _context.Category.ToListAsync();
        }
        // Action pour afficher la liste des produits
        public async Task<IActionResult> Index()
        {
            // Essayez de récupérer la liste des produits depuis le cache
            if (!_memoryCache.TryGetValue("ProductList", out List<Product> productList))
            {
                // Si le cache ne contient pas les données, récupérez-les depuis la base de données
                productList = await _context.Product.ToListAsync();

                // Ajoutez les données au cache avec une expiration après 5 minutes
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(1)
                };

                _memoryCache.Set("ProductList", productList, cacheEntryOptions);
            }
            DisplayCacheContent();
            return View(productList);
        }

        private void DisplayCacheContent()
        {
            Console.WriteLine("Contenu du Memory Cache :");

            // Obtenez une instance de MemoryCacheEntryOptions
            var cacheEntryOptions = new MemoryCacheEntryOptions();

            // Utilisez la méthode TryGetValue pour obtenir les entrées du cache
            var cacheEntries = _memoryCache.TryGetValue("ProductList", out List<Product> productList)
                ? new[] { new KeyValuePair<string, object>("ProductList", productList) }
                : Array.Empty<KeyValuePair<string, object>>();

            foreach (var cacheEntry in cacheEntries)
            {
                Console.WriteLine($"Clé: {cacheEntry.Key}");

                // Affichez chaque élément de la liste des produits
                if (cacheEntry.Value is List<Product> products)
                {
                    foreach (var product in products)
                    {
                        Console.WriteLine($"  Produit - Id: {product.Id}, Nom: {product.Name}, Prix: {product.Price}");
                    }
                }
            }
        }


        [HttpGet]
        public async Task<IActionResult> FilterProducts(string prd, decimal? minPrice, decimal? maxPrice, string selectedCategory)
        {
            try
            {
                IQueryable<Product> productsQuery = _context.Product;

                if (!string.IsNullOrEmpty(prd))
                {
                    productsQuery = productsQuery.Where(p => p.Name.Contains(prd));
                }

                if (minPrice.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
                }

                if (!string.IsNullOrEmpty(selectedCategory))
                {
                    int selectedCategoryId;
                    if (int.TryParse(selectedCategory, out selectedCategoryId))
                    {
                        productsQuery = productsQuery.Where(p => p.CategoryId == selectedCategoryId);
                    }
                }

                var filteredProducts = await productsQuery.ToListAsync();
                return Ok(filteredProducts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,CategoryId, imageUrl")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }



            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,CategoryId,imageUrl")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }


        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


    }
}
