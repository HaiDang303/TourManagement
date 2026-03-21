using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourManagement.Models;

namespace TourManagement.Pages.Staff.Customers
{
    [Authorize(Roles = "staff,admin")]
    public class DetailsModel : PageModel
    {
        private readonly TourManagementContext _context;

        public DetailsModel(TourManagementContext context)
        {
            _context = context;
        }

        public User Customer { get; set; } = null!;
        public List<BookingVm> Bookings { get; set; } = new();

        public class BookingVm
        {
            public string BookingId { get; set; } = string.Empty;
            public string TourName { get; set; } = string.Empty;
            public string DepartDate { get; set; } = string.Empty;
            public string BookingDate { get; set; } = string.Empty;
            public int PaxCount { get; set; }
            public decimal TotalPrice { get; set; }
            public string StatusId { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.Group)
                        .ThenInclude(g => g!.Tour)
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.Status)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            Customer = user;

            Bookings = user.Bookings.Select(b => new BookingVm
            {
                BookingId = b.BookingId,
                TourName = b.Group?.Tour?.Name ?? "-",
                DepartDate = b.Group?.DepartDate.ToString("dd/MM/yyyy") ?? "-",
                BookingDate = b.BookingDate.ToString("dd/MM/yyyy HH:mm"),
                PaxCount = b.Adults + b.Children + b.Infants,
                TotalPrice = b.TotalPrice,
                StatusId = b.StatusId ?? "UNKNOWN"
            })
            .OrderByDescending(b => b.BookingDate)
            .ToList();

            return Page();
        }
    }
}
