using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TourManagement.Data;
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

        public TourGroup? Group { get; set; }
        public Tour? Tour { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public string GroupId { get; set; } = string.Empty;
            public int Adults { get; set; } = 1;
            public int Children { get; set; }
            public int Infants { get; set; }
            public string? Notes { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string groupId)
        {
            if (string.IsNullOrWhiteSpace(groupId))
                return NotFound();

            Group = await _context.TourGroups
                .Include(g => g.Tour)
                    .ThenInclude(t => t.Destination)
                .FirstOrDefaultAsync(g => g.GroupId == groupId && g.StatusId == "OPEN");

            if (Group == null)
                return NotFound();

            Tour = Group.Tour;
            Input.GroupId = groupId;
            Input.Adults = 1;
            Input.Children = 0;
            Input.Infants = 0;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadGroupAndTourAsync(Input.GroupId);

            if (Group == null || Tour == null)
                return NotFound();

            int totalPax = Input.Adults + Input.Children + Input.Infants;
            int remain = Group.MaxCapacity - Group.CurrentBookings;

            if (Input.Adults < 1)
                ModelState.AddModelError("Input.Adults", "Phải có ít nhất 1 người lớn.");

            if (Input.Children < 0)
                ModelState.AddModelError("Input.Children", "Số trẻ em không hợp lệ.");

            if (Input.Infants < 0)
                ModelState.AddModelError("Input.Infants", "Số em bé không hợp lệ.");

            if (totalPax <= 0)
                ModelState.AddModelError(string.Empty, "Tổng số hành khách phải lớn hơn 0.");

            if (totalPax > remain)
                ModelState.AddModelError(string.Empty, $"Chỉ còn {remain} chỗ trống cho đợt khởi hành này.");

            if (!ModelState.IsValid)
                return Page();

            decimal adultPrice = Tour.BasePrice;
            decimal childPrice = Tour.BasePrice * 0.8m;
            decimal infantPrice = Tour.BasePrice * 0.3m;

            decimal totalPrice =
                adultPrice * Input.Adults +
                childPrice * Input.Children +
                infantPrice * Input.Infants;

            int userId = GetCurrentUserId();

            var booking = new Booking
            {
                BookingId = GenerateBookingId(),
                UserId = userId,
                GroupId = Input.GroupId,
                BookingDate = DateTime.Now,
                Adults = Input.Adults,
                Children = Input.Children,
                Infants = Input.Infants,
                TotalPrice = totalPrice,
                StatusId = "PENDING",
                Notes = Input.Notes,
                CreatedAt = DateTime.Now
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Bookings/Passengers", new { bookingId = booking.BookingId });
        }

        private async Task LoadGroupAndTourAsync(string groupId)
        {
            Group = await _context.TourGroups
                .Include(g => g.Tour)
                    .ThenInclude(t => t.Destination)
                .FirstOrDefaultAsync(g => g.GroupId == groupId && g.StatusId == "OPEN");

            Tour = Group?.Tour;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
            {
                throw new Exception("Không lấy được UserId từ phiên đăng nhập.");
            }

            return int.Parse(userIdClaim);
        }

        private string GenerateBookingId()
        {
            return "BK" + DateTime.Now.ToString("yyMMddHHmmss");
        }
    }
}