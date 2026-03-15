using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourManagement.Models;

namespace TourManagement.Pages.Bookings
{
    public class CreateModel : PageModel
    {
        private readonly TourManagementContext _context;

        public CreateModel(TourManagementContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string GroupId { get; set; } = string.Empty;

        public TourGroup? TourGroup { get; set; }

        public int RemainingCapacity { get; set; }

        [BindProperty]
        public int Adults { get; set; } = 1;

        [BindProperty]
        public int Children { get; set; }

        [BindProperty]
        public int Infants { get; set; }

        public async Task<IActionResult> OnGetAsync(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                return NotFound();
            }

            GroupId = groupId;
            TourGroup = await _context.TourGroups
                .Include(g => g.Tour)
                .Include(g => g.Tour.Destination)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);

            if (TourGroup == null)
            {
                return NotFound();
            }

            // Kiểm tra còn chỗ không
            RemainingCapacity = TourGroup.MaxCapacity - TourGroup.CurrentBookings;
            if (RemainingCapacity <= 0)
            {
                TempData["Error"] = "Tour nhóm này đã hết chỗ.";
                return RedirectToPage("/Tours/Details", new { id = TourGroup.TourId });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Account/Login", new { returnUrl = $"/Bookings/Create?groupId={GroupId}" });
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var tourGroup = await _context.TourGroups
                .Include(g => g.Tour)
                .FirstOrDefaultAsync(g => g.GroupId == GroupId);

            if (tourGroup == null)
            {
                return NotFound();
            }

            // Kiểm tra lại chỗ
            int totalPax = Adults + Children + Infants;
            if (tourGroup.MaxCapacity - tourGroup.CurrentBookings < totalPax)
            {
                ModelState.AddModelError("", $"Chỉ còn {tourGroup.MaxCapacity - tourGroup.CurrentBookings} chỗ.");
                return Page();
            }

            // Lấy user hiện tại
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToPage("/Account/Login");
            }

            // Tạo booking_id (ví dụ: BK + yyyyMMdd + random 6 ký tự)
            string bookingId = $"BK{DateTime.Now:yyyyMMdd}{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";

            var booking = new Booking
            {
                BookingId = bookingId,
                UserId = userId,
                GroupId = GroupId,
                BookingDate = DateTime.Now,
                Adults = Adults,
                Children = Children,
                Infants = Infants,
                TotalPrice = CalculateTotalPrice(tourGroup.Tour.BasePrice),
                StatusId = "PENDING",
                CreatedAt = DateTime.Now
            };

            _context.Bookings.Add(booking);

            // Cập nhật số booking hiện tại
            tourGroup.CurrentBookings += totalPax;

            await _context.SaveChangesAsync();

            // Chuyển sang trang thêm hành khách
            return RedirectToPage("/Passengers/Add", new { bookingId = booking.BookingId });
        }

        private decimal CalculateTotalPrice(decimal basePrice)
        {
            return (Adults * basePrice) +
                   (Children * basePrice * 0.75m) +
                   (Infants * basePrice * 0.10m);
        }
    }
}