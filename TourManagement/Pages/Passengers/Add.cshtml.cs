using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourManagement.Models;

namespace TourManagement.Pages.Passengers
{
    public class AddModel : PageModel
    {
        private readonly TourManagementContext _context;

        public AddModel(TourManagementContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string BookingId { get; set; } = string.Empty;

        public Booking? Booking { get; set; }

        [BindProperty]
        public BookingPassenger Passenger { get; set; } = new();

        public List<SelectListItem> Genders { get; set; } = new();
        public List<PassengerCategory> PassengerCategories { get; set; } = new();

        public int CurrentPassengerCount { get; set; }
        public int TotalRequiredPassengers { get; set; }

        public async Task<IActionResult> OnGetAsync(string bookingId)
        {
            if (string.IsNullOrEmpty(bookingId))
            {
                return NotFound();
            }

            BookingId = bookingId;

            Booking = await _context.Bookings
                .Include(b => b.Group)
                .ThenInclude(g => g.Tour)
                .Include(b => b.BookingPassengers)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (Booking == null)
            {
                return NotFound();
            }

            // Load danh sách giới tính
            var genders = await _context.Genders.ToListAsync();
            Genders = genders.Select(g => new SelectListItem
            {
                Value = g.GenderId,
                Text = g.GenderName
            }).ToList();

            // Load danh sách loại hành khách
            PassengerCategories = await _context.PassengerCategories.ToListAsync();

            // Đếm số hành khách đã thêm
            CurrentPassengerCount = Booking.BookingPassengers?.Count ?? 0;

            // Tổng số hành khách cần thêm (theo booking)
            TotalRequiredPassengers = Booking.Adults + Booking.Children + Booking.Infants;

            // Gợi ý category mặc định dựa trên số lượng còn lại
            if (CurrentPassengerCount < Booking.Adults)
                Passenger.CategoryId = "ADULT";     // giả sử có category_id = "ADULT"
            else if (CurrentPassengerCount < Booking.Adults + Booking.Children)
                Passenger.CategoryId = "CHILD";
            else
                Passenger.CategoryId = "INFANT";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload dữ liệu nếu validation fail
                await OnGetAsync(BookingId);
                return Page();
            }

            var booking = await _context.Bookings
                .Include(b => b.BookingPassengers)
                .FirstOrDefaultAsync(b => b.BookingId == BookingId);

            if (booking == null)
            {
                return NotFound();
            }

            // Kiểm tra đã đủ hành khách chưa
            int currentCount = booking.BookingPassengers?.Count ?? 0;
            if (currentCount >= (booking.Adults + booking.Children + booking.Infants))
            {
                TempData["Warning"] = "Đã đủ số lượng hành khách theo đặt chỗ.";
                return RedirectToPage("/Bookings/Details", new { id = BookingId });
            }

            // Generate passenger_id (ví dụ: PAX + bookingId + số thứ tự 3 chữ số)
            int nextSeq = currentCount + 1;
            string passengerId = $"PAX{BookingId}{nextSeq:D3}";

            Passenger.PassengerId = passengerId;
            Passenger.BookingId = BookingId;

            // Đảm bảo category hợp lệ
            if (string.IsNullOrEmpty(Passenger.CategoryId))
            {
                ModelState.AddModelError("Passenger.CategoryId", "Vui lòng chọn loại hành khách.");
                await OnGetAsync(BookingId);
                return Page();
            }

            _context.BookingPassengers.Add(Passenger);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã thêm hành khách: {Passenger.FullName}";

            // Quay lại trang thêm tiếp hoặc xem chi tiết nếu đủ
            if (currentCount + 1 >= (booking.Adults + booking.Children + booking.Infants))
            {
                return RedirectToPage("/Bookings/Details", new { id = BookingId });
            }

            return RedirectToPage(new { bookingId = BookingId });
        }
    }
}