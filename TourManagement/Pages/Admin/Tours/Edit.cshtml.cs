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

        [BindProperty(SupportsGet = true)]
        public string TourId { get; set; } = string.Empty;

        public IList<Destination> Destinations { get; set; } = new List<Destination>();
        public TourGroup? PrimaryGroup { get; set; }
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

            [Required(ErrorMessage = "Vui lòng nhập ngày khởi hành")]
            [Display(Name = "START DATE")]
            [DataType(DataType.Date)]
            public DateOnly StartDate { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập ngày kết thúc")]
            [Display(Name = "END DATE")]
            [DataType(DataType.Date)]
            public DateOnly EndDate { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập số chỗ")]
            [Range(1, 10000, ErrorMessage = "Số chỗ phải >= 1")]
            [Display(Name = "AVAIABLESEATS")]
            public int AvailableSeats { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            TourId = id;

            Destinations = await _context.Destinations
                .OrderBy(d => d.Name)
                .ToListAsync();

            var tour = await _context.Tours
                .Include(t => t.TourGroups)
                .FirstOrDefaultAsync(t => t.TourId == id);
            if (tour == null)
            {
                return NotFound();
            }

            PrimaryGroup = tour.TourGroups
                .OrderBy(g => g.DepartDate)
                .FirstOrDefault();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var start = PrimaryGroup?.DepartDate ?? today.AddDays(7);
            var end = PrimaryGroup?.ReturnDate ?? today.AddDays(7 + Math.Max(1, tour.DurationDays));
            var seats = PrimaryGroup?.MaxCapacity ?? (tour.MaxParticipants ?? 50);

            CurrentImageUrl = tour.ImageUrl;
            Input = new InputModel
            {
                Name = tour.Name,
                DestinationId = tour.DestinationId,
                DurationDays = tour.DurationDays,
                Price = tour.BasePrice,
                Category = tour.Category,
                MaxParticipants = tour.MaxParticipants,
                Description = tour.Description,
                StartDate = start,
                EndDate = end,
                AvailableSeats = seats
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

            if (Input.EndDate < Input.StartDate)
            {
                var t0 = await _context.Tours.AsNoTracking().FirstOrDefaultAsync(x => x.TourId == id);
                CurrentImageUrl = t0?.ImageUrl;
                ModelState.AddModelError(string.Empty, "END DATE phải lớn hơn hoặc bằng START DATE.");
                return Page();
            }

            var tour = await _context.Tours
                .Include(t => t.TourGroups)
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

            var group = tour.TourGroups
                .OrderBy(g => g.DepartDate)
                .FirstOrDefault();

            if (group == null)
            {
                var groupId = $"GR{DateTime.Now:yyyyMMdd}{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
                group = new TourGroup
                {
                    GroupId = groupId,
                    TourId = tour.TourId,
                    DepartDate = Input.StartDate,
                    ReturnDate = Input.EndDate,
                    MaxCapacity = Input.AvailableSeats,
                    CurrentBookings = 0,
                    StatusId = "OPEN",
                    CreatedAt = DateTime.Now
                };
                _context.TourGroups.Add(group);
            }
            else
            {
                // Không cho giảm số chỗ < số booking hiện tại
                if (Input.AvailableSeats < group.CurrentBookings)
                {
                    CurrentImageUrl = tour.ImageUrl;
                    ModelState.AddModelError(string.Empty, $"AVAIABLESEATS không thể nhỏ hơn số booking hiện tại ({group.CurrentBookings}).");
                    return Page();
                }

                group.DepartDate = Input.StartDate;
                group.ReturnDate = Input.EndDate;
                group.MaxCapacity = Input.AvailableSeats;
            }

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

