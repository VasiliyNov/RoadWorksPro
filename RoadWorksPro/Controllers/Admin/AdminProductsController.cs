using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadWorksPro.Data;
using RoadWorksPro.Models.Entities;
using RoadWorksPro.Models.ViewModels;

namespace RoadWorksPro.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Products")]
    public class AdminProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AdminProductsController> _logger;

        public AdminProductsController(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            ILogger<AdminProductsController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        // GET: Admin/Products
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View("~/Views/Admin/Products/Index.cshtml", products);
        }

        // GET: Admin/Products/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Products/Create.cshtml");
        }

        // POST: Admin/Products/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = new RoadProduct
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Dimensions = model.Dimensions,
                    Material = model.Material,
                    Standard = model.Standard,
                    IsActive = model.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                // Handle image upload
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    product.ImagePath = "/images/products/" + uniqueFileName;
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Товар успішно додано!";
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Admin/Products/Create.cshtml", model);
        }

        // GET: Admin/Products/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Dimensions = product.Dimensions,
                Material = product.Material,
                Standard = product.Standard,
                IsActive = product.IsActive,
                CurrentImagePath = product.ImagePath
            };

            return View("~/Views/Admin/Products/Edit.cshtml", model);
        }

        // POST: Admin/Products/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _context.Products.FindAsync(id);
                    if (product == null)
                    {
                        return NotFound();
                    }

                    product.Name = model.Name;
                    product.Description = model.Description;
                    product.Price = model.Price;
                    product.Dimensions = model.Dimensions;
                    product.Material = model.Material;
                    product.Standard = model.Standard;
                    product.IsActive = model.IsActive;
                    product.UpdatedAt = DateTime.UtcNow;

                    // Handle image upload
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(product.ImagePath))
                        {
                            var oldImagePath = Path.Combine(_environment.WebRootPath, product.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(fileStream);
                        }

                        product.ImagePath = "/images/products/" + uniqueFileName;
                    }

                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Товар успішно оновлено!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Products.AnyAsync(p => p.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View("~/Views/Admin/Products/Edit.cshtml", model);
        }

        // GET: Admin/Products/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Products/Delete.cshtml", product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // Delete image if exists
                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, product.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Товар успішно видалено!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}