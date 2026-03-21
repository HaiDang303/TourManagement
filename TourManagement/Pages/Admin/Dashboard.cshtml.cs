using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TourManagement.Models;

namespace TourManagement.Pages.Admin
{
    [Authorize(Roles = "admin")]
    public class DashboardModel : PageModel
    {
        private readonly TourManagementContext _context;

        public DashboardModel(TourManagementContext context) => _context = context;

        public int UserCount { get; set; }
        public int TourCount { get; set; }
        public int PendingBookingCount { get; set; }
        public decimal TotalRevenue { get; set; }

        public async Task OnGetAsync()
        {
            UserCount = await _context.Users.CountAsync();
            TourCount = await _context.Tours.CountAsync();
            var pendingIds = await _context.Statuses.Where(s => s.StatusId == "PENDING").Select(s => s.StatusId).ToListAsync();
            PendingBookingCount = await _context.Bookings.CountAsync(b => pendingIds.Contains(b.StatusId));
            var paidIds = await _context.Statuses.Where(s =>
                new[] { "PAID", "SUCCESS", "COMPLETED", "CONFIRMED" }.Contains(s.StatusId)).Select(s => s.StatusId).ToListAsync();
            TotalRevenue = await _context.Payments.Where(p => paidIds.Contains(p.StatusId)).SumAsync(p => p.Amount);
        }
    }
}

