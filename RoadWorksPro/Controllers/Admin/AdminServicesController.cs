using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadWorksPro.Data;
using RoadWorksPro.Models.Entities;
using RoadWorksPro.Models.ViewModels;

namespace RoadWorksPro.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Services")]
    public class AdminServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AdminServicesController> _logger;

        public AdminServicesController(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            ILogger<AdminServicesController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        // GET: Admin/Services
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var services = await _context.Services
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return View("~/Views/Admin/Services/Index.cshtml", services);
        }

        // GET: Admin/Services/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Services/Create.cshtml");
        }

        // POST: Admin/Services/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var service = new RoadService
                {
                    Name = model.Name,
                    Description = model.Description,
                    PriceInfo = model.PriceInfo,
                    IsActive = model.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                // Handle image upload
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "services");
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

                    service.ImagePath = "/images/services/" + uniqueFileName;
                }

                _context.Services.Add(service);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Послугу успішно додано!";
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Admin/Services/Create.cshtml", model);
        }

        // GET: Admin/Services/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            var model = new ServiceEditViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                PriceInfo = service.PriceInfo,
                IsActive = service.IsActive,
                CurrentImagePath = service.ImagePath
            };

            return View("~/Views/Admin/Services/Edit.cshtml", model);
        }

        // POST: Admin/Services/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var service = await _context.Services.FindAsync(id);
                    if (service == null)
                    {
                        return NotFound();
                    }

                    service.Name = model.Name;
                    service.Description = model.Description;
                    service.PriceInfo = model.PriceInfo;
                    service.IsActive = model.IsActive;
                    service.UpdatedAt = DateTime.UtcNow;

                    // Handle image upload
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(service.ImagePath))
                        {
                            var oldImagePath = Path.Combine(_environment.WebRootPath, service.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "services");
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

                        service.ImagePath = "/images/services/" + uniqueFileName;
                    }

                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Послугу успішно оновлено!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Services.AnyAsync(s => s.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View("~/Views/Admin/Services/Edit.cshtml", model);
        }

        // GET: Admin/Services/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Services/Delete.cshtml", service);
        }

        // POST: Admin/Services/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                // Delete image if exists
                if (!string.IsNullOrEmpty(service.ImagePath))
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, service.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Services.Remove(service);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Послугу успішно видалено!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}