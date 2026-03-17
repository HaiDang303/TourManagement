using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TourManagement.Models;

namespace TourManagement.Pages.Payments
{
    public class CreateModel : PageModel
    {
        private readonly TourManagementContext _context;

        public CreateModel(TourManagementContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string BookingId { get; set; } = string.Empty;

        public Booking? Booking { get; set; }

        [BindProperty]
        public Payment Payment { get; set; } = new();

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

            // Kiểm tra đã đủ hành khách chưa
            if ((Booking.BookingPassengers?.Count ?? 0) < (Booking.Adults + Booking.Children + Booking.Infants))
            {
                TempData["Error"] = "Vui lòng thêm đủ hành khách trước khi thanh toán.";
                return RedirectToPage("/Passengers/Add", new { bookingId });
            }

            // Gán giá trị mặc định cho Payment
            Payment.Amount = Booking.TotalPrice;
            Payment.PaymentDate = DateTime.Now;
            Payment.StatusId = "PENDING";
            Payment.CreatedAt = DateTime.Now;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(BookingId);
                return Page();
            }

            var booking = await _context.Bookings.FindAsync(BookingId);
            if (booking == null)
            {
                return NotFound();
            }

            // Generate payment_id (ví dụ: PAY + yyyyMMdd + random)
            string paymentId = $"PAY{DateTime.Now:yyyyMMdd}{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";

            Payment.PaymentId = paymentId;
            Payment.BookingId = BookingId;
            Payment.Amount = booking.TotalPrice; // đảm bảo khớp

            _context.Payments.Add(Payment);

            // (Tùy chọn) Cập nhật trạng thái booking thành "AWAITING_PAYMENT" hoặc tương tự
            // booking.StatusId = "AWAITING_PAYMENT";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Yêu cầu thanh toán đã được tạo. Vui lòng thực hiện thanh toán theo phương thức đã chọn.";

            // Trong thực tế: redirect đến trang thanh toán cổng (VNPay, Momo,...) hoặc hiển thị thông tin chuyển khoản
            return RedirectToPage("/Bookings/Details", new { id = BookingId });
        }
    }
}