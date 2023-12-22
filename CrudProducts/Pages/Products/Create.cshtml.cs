using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CrudProducts.Data;
using CrudProducts.Model;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using CrudProducts.Controllers;

namespace CrudProducts.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ProductsController _productsController;

        public CreateModel( IWebHostEnvironment hostingEnvironment, ProductsController productsController)
        {
            _hostingEnvironment = hostingEnvironment;
            _productsController = productsController;

        }



        [BindProperty]
        public Product Product { get; set; }
        // Property to represent the uploaded image file
        [BindProperty]
        public IFormFile ImageFile { get; set; }


        [BindProperty]
        public int Category { get; set; }
        public SelectList CategoryList { get; set; }

        public IActionResult OnGet()
        {
            // Appelez la méthode GetCategories du controller
            var categoriesResult = _productsController.GetCategories();

            if (categoriesResult != null && categoriesResult.Result is List<Category> categories)
            {
                CategoryList = new SelectList(categories, "Id", "Name");
            }
            else
            {
                Console.WriteLine("Failed to get categories.");
            }

            return Page();
        }



        [HttpPost]
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // Generate a unique filename using a timestamp
                var fileName = DateTime.Now.Ticks + Path.GetExtension(ImageFile.FileName);

                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Ensure the uploads folder exists
                Directory.CreateDirectory(uploadsFolder);

                // Save the file to the server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                // Save the file path in your database
                Product.imageUrl = "/uploads/" + fileName;
                // Update the path as per your project structure

            }
            var createResult = await _productsController.Create(Product);

            if (createResult is RedirectToActionResult redirectToActionResult)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                return Page();
            }
        }
    }
}
