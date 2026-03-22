using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TourManagement.Models;

namespace TourManagement.Pages.Staff.Tours
{
    [Authorize(Roles = "staff,admin")]
    public class CreateModel : PageModel
    {
        private readonly TourManagementContext _context;

        public CreateModel(TourManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public IList<Tour> Tours { get; set; } = new List<Tour>();

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng chọn Tour")]
            [Display(Name = "Tour du lịch")]
            public string TourId { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập ngày khởi hành")]
            [Display(Name = "Ngày khởi hành")]
            [DataType(DataType.Date)]
            public DateOnly DepartDate { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập ngày kết thúc")]
            [Display(Name = "Ngày kết thúc")]
            [DataType(DataType.Date)]
            public DateOnly ReturnDate { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập số chỗ")]
            [Range(1, 10000, ErrorMessage = "Số chỗ phải >= 1")]
            [Display(Name = "Số chỗ tối đa")]
            public int MaxCapacity { get; set; } = 50;
        }

        public async Task OnGetAsync()
        {
            Tours = await _context.Tours
                .Include(t => t.Destination)
                .OrderBy(t => t.Name)
                .ToListAsync();

            var today = DateOnly.FromDateTime(DateTime.Today);
            Input.DepartDate = today.AddDays(7);
            Input.ReturnDate = today.AddDays(10);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Tours = await _context.Tours
                .Include(t => t.Destination)
                .OrderBy(t => t.Name)
                .ToListAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.ReturnDate < Input.DepartDate)
            {
                ModelState.AddModelError(string.Empty, "Ngày kết thúc phải lớn hơn hoặc bằng ngày khởi hành.");
                return Page();
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            if (Input.DepartDate < today)
            {
                ModelState.AddModelError(string.Empty, "Ngày khởi hành không được ở quá khứ.");
                return Page();
            }

            var tourExists = await _context.Tours.AnyAsync(t => t.TourId == Input.TourId);
            if (!tourExists)
            {
                ModelState.AddModelError(string.Empty, "Tour không hợp lệ.");
                return Page();
            }

            var groupId = $"GR{DateTime.Now:yyyyMMdd}{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
            var group = new TourGroup
            {
                GroupId = groupId,
                TourId = Input.TourId,
                DepartDate = Input.DepartDate,
                ReturnDate = Input.ReturnDate,
                MaxCapacity = Input.MaxCapacity,
                CurrentBookings = 0,
                StatusId = "OPEN",
                CreatedAt = DateTime.Now
            };

            _context.TourGroups.Add(group);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tạo lịch khởi hành mới thành công.";
            return RedirectToPage("/Staff/Tours/Index");
        }
    }
}
