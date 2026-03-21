using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TourManagement.Data;
using TourManagement.Models;

namespace TourManagement.Pages.Payments
{
    public class CheckoutModel : PageModel
    {
        private readonly TourManagementContext _context;

        public CheckoutModel(TourManagementContext context)
        {
            _context = context;
        }

        public Booking? Booking { get; set; }

        [BindProperty]
        public string Method { get; set; } = "Cash";

        public async Task<IActionResult> OnGetAsync(string bookingId)
        {
            if (string.IsNullOrWhiteSpace(bookingId))
                return NotFound();

            Booking = await _context.Bookings
                .Include(b => b.Group)
                    .ThenInclude(g => g.Tour)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (Booking == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string bookingId)
        {
            if (string.IsNullOrWhiteSpace(bookingId))
                return NotFound();

            Booking = await _context.Bookings
                .Include(b => b.Group)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (Booking == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(Method))
            {
                ModelState.AddModelError(nameof(Method), "Vui lòng chọn phương thức thanh toán.");
                return Page();
            }

            int totalPax = Booking.Adults + Booking.Children + Booking.Infants;
            int remain = Booking.Group.MaxCapacity - Booking.Group.CurrentBookings;

            if (totalPax > remain)
            {
                ModelState.AddModelError(string.Empty, "Số chỗ còn lại không đủ để xác nhận booking này.");
                return Page();
            }

            bool alreadyPaid = Booking.Payments.Any(p => p.StatusId == "PAID");
            if (alreadyPaid)
            {
                return RedirectToPage("/Bookings/Details", new { id = bookingId });
            }

            var payment = new Payment
            {
                PaymentId = GeneratePaymentId(),
                BookingId = bookingId,
                Amount = Booking.TotalPrice,
                PaymentDate = DateTime.Now,
                Method = Method,
                TransactionRef = GenerateTransactionRef(),
                StatusId = "PAID",
                CreatedAt = DateTime.Now
            };

            _context.Payments.Add(payment);

            Booking.StatusId = "CONFIRMED";
            Booking.Group.CurrentBookings += totalPax;

            await _context.SaveChangesAsync();

            return RedirectToPage("/Bookings/Details", new { id = bookingId });
        }

        private string GeneratePaymentId()
        {
            return "PM" + DateTime.Now.ToString("yyMMddHHmmss");
        }

        private string GenerateTransactionRef()
        {
            return "TRX" + Guid.NewGuid().ToString("N")[..10].ToUpper();
        }
    }
}