using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrudProducts.Data;
using CrudProducts.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using CrudProducts.Controllers;

namespace CrudProducts.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ProductsController _productsController;

        public EditModel(IWebHostEnvironment hostingEnvironment, ProductsController productsController)
        {
            _hostingEnvironment = hostingEnvironment;
            _productsController = productsController;
        }

        [BindProperty]
        public Product Product { get; set; }
        [BindProperty]
        public IFormFile ImageFile { get; set; }

        public SelectList CategoryList { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productResult = await _productsController.GetProduct(id);

            if (productResult is ViewResult productViewResult)
            {
                Product = productViewResult.Model as Product;
            }
            else
            {
                return NotFound();
            }

            if (Product == null)
            {
                return NotFound();
            }

            var categoryListResult = await _productsController.GetCategories();

            if (categoryListResult is List<Category> categoryList)
            {
                CategoryList = new SelectList(categoryList, "Id", "Name", Product.CategoryId);
            }
            else
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                // Supprimez l'ancienne image si elle existe
                if (!string.IsNullOrEmpty(Product.imageUrl))
                {
                    var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", Product.imageUrl);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // Générez un nom de fichier unique en utilisant un timestamp
                var fileName = DateTime.Now.Ticks + Path.GetExtension(ImageFile.FileName);

                // Chemin où sauvegarder la nouvelle image
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Assurez-vous que le dossier d'uploads existe
                Directory.CreateDirectory(uploadsFolder);

                // Sauvegardez le fichier sur le serveur
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                // Mise à jour du chemin de l'image dans la base de données avec le nouveau chemin
                Product.imageUrl = "/uploads/" + fileName;
            }

            // Appel de la méthode Edit du controller pour gérer l'édition
            var editResult = await _productsController.Edit(Product.Id, Product);

            if (editResult is RedirectToActionResult)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                return NotFound();
            }
        }
    }

}
