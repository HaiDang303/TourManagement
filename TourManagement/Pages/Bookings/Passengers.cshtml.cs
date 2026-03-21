using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TourManagement.Data;
using TourManagement.Models;

namespace TourManagement.Pages.Bookings
{
    public class PassengersModel : PageModel
    {
        private readonly TourManagementContext _context;

        public PassengersModel(TourManagementContext context)
        {
            _context = context;
        }

        public Booking? Booking { get; set; }

        [BindProperty]
        public List<PassengerInput> Passengers { get; set; } = new();

        public class PassengerInput
        {
            public string FullName { get; set; } = string.Empty;
            public string? GenderId { get; set; }
            public DateTime? Dob { get; set; }
            public string CategoryId { get; set; } = string.Empty;
            public string? PassportNo { get; set; }
            public string? Notes { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string bookingId)
        {
            if (string.IsNullOrWhiteSpace(bookingId))
                return NotFound();

            Booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (Booking == null)
                return NotFound();

            BuildPassengerInputsFromBooking(Booking);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string bookingId)
        {
            if (string.IsNullOrWhiteSpace(bookingId))
                return NotFound();

            Booking = await _context.Bookings
                .Include(b => b.BookingPassengers)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (Booking == null)
                return NotFound();

            int expectedCount = Booking.Adults + Booking.Children + Booking.Infants;

            if (Passengers == null || Passengers.Count != expectedCount)
            {
                ModelState.AddModelError(string.Empty, "Số lượng hành khách không khớp với booking.");
            }

            for (int i = 0; i < Passengers.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(Passengers[i].FullName))
                {
                    ModelState.AddModelError($"Passengers[{i}].FullName", "Họ tên không được để trống.");
                }

                if (string.IsNullOrWhiteSpace(Passengers[i].CategoryId))
                {
                    ModelState.AddModelError($"Passengers[{i}].CategoryId", "Loại hành khách không hợp lệ.");
                }
            }

            if (!ModelState.IsValid)
                return Page();

            if (Booking.BookingPassengers.Any())
            {
                _context.BookingPassengers.RemoveRange(Booking.BookingPassengers);
            }

            foreach (var item in Passengers)
            {
                var passenger = new BookingPassenger
                {
                    PassengerId = GeneratePassengerId(),
                    BookingId = bookingId,
                    FullName = item.FullName.Trim(),
                    GenderId = string.IsNullOrWhiteSpace(item.GenderId) ? null : item.GenderId,
                    Dob = item.Dob.HasValue ? DateOnly.FromDateTime(item.Dob.Value) : null,
                    CategoryId = item.CategoryId,
                    PassportNo = string.IsNullOrWhiteSpace(item.PassportNo) ? null : item.PassportNo.Trim(),
                    Notes = string.IsNullOrWhiteSpace(item.Notes) ? null : item.Notes.Trim()
                };

                _context.BookingPassengers.Add(passenger);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Payments/Checkout", new { bookingId });
        }

        private void BuildPassengerInputsFromBooking(Booking booking)
        {
            Passengers = new List<PassengerInput>();

            for (int i = 0; i < booking.Adults; i++)
            {
                Passengers.Add(new PassengerInput
                {
                    CategoryId = "ADULT"
                });
            }

            for (int i = 0; i < booking.Children; i++)
            {
                Passengers.Add(new PassengerInput
                {
                    CategoryId = "CHILD"
                });
            }

            for (int i = 0; i < booking.Infants; i++)
            {
                Passengers.Add(new PassengerInput
                {
                    CategoryId = "INFANT"
                });
            }
        }

        private string GeneratePassengerId()
        {
            return "PS" + Guid.NewGuid().ToString("N")[..10].ToUpper();
        }
    }
}

