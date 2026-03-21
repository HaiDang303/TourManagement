using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourManagement.Models;

namespace TourManagement.Pages.Bookings
{
    [Authorize]
    public class HistoryModel : PageModel
    {
        private readonly TourManagementContext _context;

        public HistoryModel(TourManagementContext context)
        {
            _context = context;
        }

        public IList<Booking> Bookings { get; set; } = new List<Booking>();

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int userId = GetCurrentUserId();

            var query = _context.Bookings
                .Include(b => b.Group)
                    .ThenInclude(g => g.Tour)
                .Include(b => b.Payments)
                .Where(b => b.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(StatusFilter))
            {
                query = query.Where(b => b.StatusId == StatusFilter);
            }

            Bookings = await query
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return Page();
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
            {
                throw new Exception("Bạn chưa đăng nhập hoặc phiên đăng nhập không hợp lệ.");
            }

            return int.Parse(userIdClaim);
        }
    }
}