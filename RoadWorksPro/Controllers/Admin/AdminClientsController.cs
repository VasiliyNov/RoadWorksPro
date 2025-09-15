using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadWorksPro.Data;
using RoadWorksPro.Models.Entities;

namespace RoadWorksPro.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Clients")]
    public class AdminClientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminClientsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var clients = await _context.Clients
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();
            return View("~/Views/Admin/Clients/Index.cshtml", clients);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Clients/Create.cshtml");
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoadClient client, IFormFile? logo)
        {
            if (ModelState.IsValid)
            {
                if (logo != null && logo.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "clients");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + logo.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await logo.CopyToAsync(fileStream);
                    }

                    client.LogoPath = "/images/clients/" + uniqueFileName;
                }

                client.CreatedAt = DateTime.UtcNow;
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Клієнт успішно доданий!";
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Admin/Clients/Create.cshtml", client);
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            return View("~/Views/Admin/Clients/Delete.cshtml", client);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                if (!string.IsNullOrEmpty(client.LogoPath))
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, client.LogoPath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Клієнт видалений!";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            return View("~/Views/Admin/Clients/Edit.cshtml", client);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoadClient client, IFormFile? logo)
        {
            if (id != client.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingClient = await _context.Clients.FindAsync(id);
                    if (existingClient == null) return NotFound();

                    existingClient.Name = client.Name;
                    existingClient.WebsiteUrl = client.WebsiteUrl;
                    existingClient.DisplayOrder = client.DisplayOrder;
                    existingClient.IsActive = client.IsActive;

                    if (logo != null && logo.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(existingClient.LogoPath))
                        {
                            var oldImagePath = Path.Combine(_environment.WebRootPath, existingClient.LogoPath.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "clients");
                        Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + logo.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await logo.CopyToAsync(fileStream);
                        }

                        existingClient.LogoPath = "/images/clients/" + uniqueFileName;
                    }

                    _context.Update(existingClient);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Клієнт успішно оновлений!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Clients.AnyAsync(c => c.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            return View("~/Views/Admin/Clients/Edit.cshtml", client);
        }
    }
}