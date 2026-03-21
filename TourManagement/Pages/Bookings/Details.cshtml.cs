using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TourManagement.Data;
using TourManagement.Models;

namespace TourManagement.Pages.Bookings
{
    public class DetailsModel : PageModel
    {
        private readonly TourManagementContext _context;

        public DetailsModel(TourManagementContext context)
        {
            _context = context;
        }

        public Booking? Booking { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            Booking = await _context.Bookings
                .Include(b => b.Group)
                    .ThenInclude(g => g.Tour)
                .Include(b => b.BookingPassengers)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (Booking == null)
                return NotFound();

            return Page();
        }
    }
}