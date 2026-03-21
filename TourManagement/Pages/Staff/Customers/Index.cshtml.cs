using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourManagement.Models;

namespace TourManagement.Pages.Staff.Customers
{
    [Authorize(Roles = "staff,admin")]
    public class IndexModel : PageModel
    {
        private readonly TourManagementContext _context;

        public IndexModel(TourManagementContext context)
        {
            _context = context;
        }

        public List<CustomerVm> Customers { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public class CustomerVm
        {
            public int UserId { get; set; }
            public string Name { get; set; } = "";
            public string Email { get; set; } = "";
            public string? Phone { get; set; }
            public int TotalBookings { get; set; }
            public decimal TotalSpent { get; set; }
            public DateTime? LastBookingDate { get; set; }
        }

        public async Task OnGetAsync()
        {
            var query = _context.Users
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.Payments)
                .Where(u => u.Bookings.Any()); // Chỉ lấy những khách đã từng đặt tour

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var normSearch = SearchTerm.Trim().ToLower();
                query = query.Where(u => 
                    u.Name.ToLower().Contains(normSearch) || 
                    u.Email.ToLower().Contains(normSearch) || 
                    (u.Phone != null && u.Phone.Contains(normSearch))
                );
            }

            var validStatuses = new[] { "CONFIRMED", "PAID", "COMPLETED", "APPROVED", "BOOKED" };

            var dbUsers = await query.ToListAsync();

            Customers = dbUsers.Select(u => new CustomerVm
            {
                UserId = u.Id,
                Name = u.Name,
                Email = u.Email,
                Phone = u.Phone,
                TotalBookings = u.Bookings.Count,
                TotalSpent = u.Bookings
                    .Where(b => validStatuses.Contains((b.StatusId ?? "").ToUpperInvariant()) || 
                                (b.Payments != null && b.Payments.Any(p => validStatuses.Contains((p.StatusId ?? "").ToUpperInvariant()))))
                    .Sum(b => b.TotalPrice),
                LastBookingDate = u.Bookings.Any() ? u.Bookings.Max(b => b.BookingDate) : null
            })
            .OrderByDescending(c => c.LastBookingDate)
            .ToList();
        }
    }
}
