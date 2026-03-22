using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using TourManagement.Models;

namespace TourManagement.Pages.Admin.Tours
{
    [Authorize(Roles = "admin")]
    public class CreateModel : PageModel
    {
        private readonly TourManagementContext _context;
        private readonly IWebHostEnvironment _env;

        public CreateModel(TourManagementContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public IList<Destination> Destinations { get; set; } = new List<Destination>();

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập tên tour")]
            [Display(Name = "Tên tour")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng chọn điểm đến")]
            [Display(Name = "Điểm đến")]
            public string DestinationId { get; set; } = string.Empty;


            [Display(Name = "Mô tả")]
            public string? Description { get; set; }

            [Display(Name = "Ảnh tour")]
            public IFormFile? ImageFile { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập giá")]
            [Range(0, double.MaxValue, ErrorMessage = "Giá phải >= 0")]
            [Display(Name = "Giá (VNĐ)")]
            public decimal Price { get; set; }

            [Display(Name = "Danh mục")]
            public string? Category { get; set; }


        }

        public async Task OnGetAsync()
        {
            Destinations = await _context.Destinations
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Destinations = await _context.Destinations
                .OrderBy(d => d.Name)
                .ToListAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }


            var destinationExists = await _context.Destinations.AnyAsync(d => d.DestinationId == Input.DestinationId);
            if (!destinationExists)
            {
                ModelState.AddModelError(string.Empty, "Điểm đến không hợp lệ.");
                return Page();
            }

            var creatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int? createdBy = null;
            if (int.TryParse(creatorId, out var parsed))
            {
                createdBy = parsed;
            }

            var tourId = $"TOUR{DateTime.Now:yyyyMMdd}{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

            var tour = new Tour
            {
                TourId = tourId,
                Name = Input.Name,
                DestinationId = Input.DestinationId,
                DurationDays = 1, // Default value as it's required in DB but removed from form
                BasePrice = Input.Price,
                Category = Input.Category,
                MaxParticipants = null,
                Description = Input.Description,
                ImageUrl = await SaveImageAsync(Input.ImageFile),
                CreatedBy = createdBy,
                CreatedAt = DateTime.Now
            };

            _context.Tours.Add(tour);


            await _context.SaveChangesAsync();

            TempData["Success"] = "Tạo tour thành công.";
            return RedirectToPage("/Admin/Tours/Index");
        }

        private async Task<string?> SaveImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".gif" && ext != ".webp") return null;
            var dir = Path.Combine(_env.WebRootPath, "uploads", "tours");
            Directory.CreateDirectory(dir);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var path = Path.Combine(dir, fileName);
            using (var stream = new FileStream(path, FileMode.Create))
                await file.CopyToAsync(stream);
            return $"/uploads/tours/{fileName}";
        }
    }
}

