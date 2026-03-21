using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TourManagement.Models;

namespace TourManagement.Pages.Staff.Bookings
{
    [Authorize(Roles = "staff,admin")]
    public class EditModel : PageModel
    {
        private readonly TourManagementContext _context;

        public EditModel(TourManagementContext context) => _context = context;

        public Booking? Booking { get; set; }
        [BindProperty] public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public int Adults { get; set; }
            public int Children { get; set; }
            public int Infants { get; set; }
            public string? Notes { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Booking = await _context.Bookings
                .Include(b => b.Group).ThenInclude(g => g!.Tour)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BookingId == id);
            if (Booking == null) return NotFound();
            Input = new InputModel { Adults = Booking.Adults, Children = Booking.Children, Infants = Booking.Infants, Notes = Booking.Notes };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromRoute] string id)
        {
            var booking = await _context.Bookings.Include(b => b.Group).ThenInclude(g => g!.Tour).FirstOrDefaultAsync(b => b.BookingId == id);
            if (booking == null) return NotFound();
            var confirmedIds = new[] { "CONFIRMED", "APPROVED", "BOOKED", "COMPLETED", "DONE", "FINISHED" };
            var currentConfirmed = confirmedIds.Contains(booking.StatusId ?? "", StringComparer.OrdinalIgnoreCase);
            int oldPax = booking.Adults + booking.Children + booking.Infants;
            int newPax = Input.Adults + Input.Children + Input.Infants;
            if (Input.Adults < 1) { ModelState.AddModelError("Input.Adults", "Phải có ít nhất 1 người lớn."); }
            if (Input.Children < 0 || Input.Infants < 0) { ModelState.AddModelError("", "Số trẻ em / em bé không hợp lệ."); }
            int remain = booking.Group.MaxCapacity - booking.Group.CurrentBookings;
            if (currentConfirmed)
                remain += oldPax;
            if (newPax > remain)
            {
                ModelState.AddModelError("", $"Không đủ chỗ. Có thể đặt tối đa {remain} người.");
            }
            if (!ModelState.IsValid)
            {
                Booking = await _context.Bookings.Include(b => b.Group).ThenInclude(g => g!.Tour).Include(b => b.User).FirstOrDefaultAsync(b => b.BookingId == id);
                return Page();
            }
            if (currentConfirmed)
                booking.Group.CurrentBookings -= oldPax;
            booking.Adults = Input.Adults;
            booking.Children = Input.Children;
            booking.Infants = Input.Infants;
            booking.Notes = Input.Notes;

            if (booking.Group?.Tour != null)
            {
                decimal adultPrice = booking.Group.Tour.BasePrice;
                decimal childPrice = booking.Group.Tour.BasePrice * 0.8m;
                decimal infantPrice = booking.Group.Tour.BasePrice * 0.3m;
                booking.TotalPrice = adultPrice * Input.Adults + childPrice * Input.Children + infantPrice * Input.Infants;
            }

            if (currentConfirmed)
                booking.Group.CurrentBookings += newPax;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật thông tin booking.";
            return RedirectToPage("/Staff/Bookings/Index");
        }
    }
}
