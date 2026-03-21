using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TourManagement.Models;

namespace TourManagement.Pages.Admin.Tours
{
    [Authorize(Roles = "admin")]
    public class EditModel : PageModel
    {
        private readonly TourManagementContext _context;
        private readonly IWebHostEnvironment _env;

        public EditModel(TourManagementContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public string TourId { get; set; } = string.Empty;

        public IList<Destination> Destinations { get; set; } = new List<Destination>();
        public string? CurrentImageUrl { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập tên tour")]
            [Display(Name = "Tên tour")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng chọn điểm đến")]
            [Display(Name = "Điểm đến")]
            public string DestinationId { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập số ngày")]
            [Range(1, 365, ErrorMessage = "Số ngày phải từ 1 đến 365")]
            [Display(Name = "Thời lượng (ngày)")]
            public int DurationDays { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập giá")]
            [Range(0, double.MaxValue, ErrorMessage = "Giá phải >= 0")]
            [Display(Name = "Giá (VNĐ)")]
            public decimal Price { get; set; }

            [Display(Name = "Danh mục")]
            public string? Category { get; set; }

            [Display(Name = "Số người tối đa")]
            [Range(1, 10000, ErrorMessage = "Số người tối đa phải >= 1")]
            public int? MaxParticipants { get; set; }

            [Display(Name = "Mô tả")]
            public string? Description { get; set; }

            [Display(Name = "Ảnh tour")]
            public IFormFile? ImageFile { get; set; }

        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            TourId = id;

            Destinations = await _context.Destinations
                .OrderBy(d => d.Name)
                .ToListAsync();

            var tour = await _context.Tours
                .FirstOrDefaultAsync(t => t.TourId == id);
            if (tour == null)
            {
                return NotFound();
            }

            CurrentImageUrl = tour.ImageUrl;
            Input = new InputModel
            {
                Name = tour.Name,
                DestinationId = tour.DestinationId,
                DurationDays = tour.DurationDays,
                Price = tour.BasePrice,
                Category = tour.Category,
                MaxParticipants = tour.MaxParticipants,
                Description = tour.Description
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? id)
        {
            id ??= TourId;
            if (string.IsNullOrEmpty(id)) return NotFound();
            TourId = id;

            Destinations = await _context.Destinations
                .OrderBy(d => d.Name)
                .ToListAsync();

            if (!ModelState.IsValid)
            {
                var t = await _context.Tours.AsNoTracking().FirstOrDefaultAsync(x => x.TourId == id);
                CurrentImageUrl = t?.ImageUrl;
                return Page();
            }


            var tour = await _context.Tours
                .FirstOrDefaultAsync(t => t.TourId == id);
            if (tour == null)
            {
                return NotFound();
            }

            tour.Name = Input.Name;
            tour.DestinationId = Input.DestinationId;
            tour.DurationDays = Input.DurationDays;
            tour.BasePrice = Input.Price;
            tour.Category = Input.Category;
            tour.MaxParticipants = Input.MaxParticipants;
            tour.Description = Input.Description;
            var newImageUrl = await SaveImageAsync(Input.ImageFile);
            if (newImageUrl != null)
                tour.ImageUrl = newImageUrl;
            tour.UpdatedAt = DateTime.Now;


            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật tour thành công.";
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

