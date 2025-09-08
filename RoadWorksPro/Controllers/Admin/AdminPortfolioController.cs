using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadWorksPro.Data;
using RoadWorksPro.Models.Entities;
using RoadWorksPro.Models.ViewModels;

namespace RoadWorksPro.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Portfolio")]
    public class AdminPortfolioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AdminPortfolioController> _logger;

        public AdminPortfolioController(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            ILogger<AdminPortfolioController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var items = await _context.PortfolioItems
                .Include(p => p.AdditionalImages)
                .OrderBy(p => p.DisplayOrder)
                .ThenByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View("~/Views/Admin/Portfolio/Index.cshtml", items);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Portfolio/Create.cshtml");
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PortfolioCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var item = new PortfolioItem
                {
                    Title = model.Title,
                    Location = model.Location,
                    Description = model.Description,
                    Category = model.Category,
                    WorkVolume = model.WorkVolume,
                    Materials = model.Materials,
                    CompletedDate = model.CompletedDate,
                    DisplaySize = model.DisplaySize,
                    DisplayOrder = model.DisplayOrder,
                    IsFeatured = model.IsFeatured,
                    IsActive = model.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                // Handle main image upload
                if (model.MainImageFile != null && model.MainImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "portfolio");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.MainImageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.MainImageFile.CopyToAsync(fileStream);
                    }

                    item.MainImagePath = "/images/portfolio/" + uniqueFileName;
                }

                _context.PortfolioItems.Add(item);
                await _context.SaveChangesAsync();

                // Handle additional images
                if (model.AdditionalImageFiles != null && model.AdditionalImageFiles.Any())
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "portfolio");
                    int order = 0;

                    foreach (var imageFile in model.AdditionalImageFiles)
                    {
                        if (imageFile.Length > 0)
                        {
                            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(fileStream);
                            }

                            var portfolioImage = new PortfolioImage
                            {
                                PortfolioItemId = item.Id,
                                ImagePath = "/images/portfolio/" + uniqueFileName,
                                DisplayOrder = order++
                            };

                            _context.PortfolioImages.Add(portfolioImage);
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Роботу успішно додано!";
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Admin/Portfolio/Create.cshtml", model);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.PortfolioItems
                .Include(p => p.AdditionalImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var model = new PortfolioEditViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Location = item.Location,
                Description = item.Description,
                Category = item.Category,
                WorkVolume = item.WorkVolume,
                Materials = item.Materials,
                CompletedDate = item.CompletedDate,
                DisplaySize = item.DisplaySize,
                DisplayOrder = item.DisplayOrder,
                IsFeatured = item.IsFeatured,
                IsActive = item.IsActive,
                CurrentMainImagePath = item.MainImagePath,
                CurrentAdditionalImages = item.AdditionalImages.ToList()
            };

            return View("~/Views/Admin/Portfolio/Edit.cshtml", model);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PortfolioEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var item = await _context.PortfolioItems
                        .Include(p => p.AdditionalImages)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (item == null)
                    {
                        return NotFound();
                    }

                    item.Title = model.Title;
                    item.Location = model.Location;
                    item.Description = model.Description;
                    item.Category = model.Category;
                    item.WorkVolume = model.WorkVolume;
                    item.Materials = model.Materials;
                    item.CompletedDate = model.CompletedDate;
                    item.DisplaySize = model.DisplaySize;
                    item.DisplayOrder = model.DisplayOrder;
                    item.IsFeatured = model.IsFeatured;
                    item.IsActive = model.IsActive;

                    // Handle main image update
                    if (model.MainImageFile != null && model.MainImageFile.Length > 0)
                    {
                        // Delete old image
                        if (!string.IsNullOrEmpty(item.MainImagePath))
                        {
                            var oldImagePath = Path.Combine(_environment.WebRootPath, item.MainImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "portfolio");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.MainImageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.MainImageFile.CopyToAsync(fileStream);
                        }

                        item.MainImagePath = "/images/portfolio/" + uniqueFileName;
                    }

                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Роботу успішно оновлено!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.PortfolioItems.AnyAsync(p => p.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            return View("~/Views/Admin/Portfolio/Edit.cshtml", model);
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.PortfolioItems
                .Include(p => p.AdditionalImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Portfolio/Delete.cshtml", item);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.PortfolioItems
                .Include(p => p.AdditionalImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (item != null)
            {
                // Delete main image
                if (!string.IsNullOrEmpty(item.MainImagePath))
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, item.MainImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // Delete additional images
                foreach (var img in item.AdditionalImages)
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, img.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.PortfolioItems.Remove(item);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Роботу успішно видалено!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}