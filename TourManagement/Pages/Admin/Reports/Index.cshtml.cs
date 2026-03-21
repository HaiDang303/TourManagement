using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TourManagement.Models;

namespace TourManagement.Pages.Admin.Reports
{
    [Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        private readonly TourManagementContext _context;

        public IndexModel(TourManagementContext context) => _context = context;

        [BindProperty(SupportsGet = true)] public DateTime? FromDate { get; set; }
        [BindProperty(SupportsGet = true)] public DateTime? ToDate { get; set; }

        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public List<RevenueByTourVm> RevenueByTour { get; set; } = new();
        public List<RevenueByMonthVm> RevenueByMonth { get; set; } = new();
        public List<TourAlmostFullVm> ToursAlmostFull { get; set; } = new();
        public List<CompletedTourVm> CompletedTours { get; set; } = new();

        private static readonly HashSet<string> ConfirmedStatuses = new(StringComparer.OrdinalIgnoreCase)
            { "CONFIRMED", "APPROVED", "BOOKED", "COMPLETED", "DONE", "FINISHED" };
        private static readonly HashSet<string> PaidStatuses = new(StringComparer.OrdinalIgnoreCase)
            { "PAID", "SUCCESS", "COMPLETED", "CONFIRMED" };

        public async Task OnGetAsync()
        {
            var from = FromDate ?? DateTime.Today.AddMonths(-3);
            var to = ToDate ?? DateTime.Today.AddDays(1);
            if (to < from) to = from.AddDays(1);

            var paidIds = await _context.Statuses
                .Where(s => PaidStatuses.Contains(s.StatusId))
                .Select(s => s.StatusId)
                .ToListAsync();
            var confirmedIds = await _context.Statuses
                .Where(s => ConfirmedStatuses.Contains(s.StatusId))
                .Select(s => s.StatusId)
                .ToListAsync();

            var paymentsInRange = await _context.Payments
                .Include(p => p.Booking).ThenInclude(b => b!.Group).ThenInclude(g => g!.Tour)
                .Where(p => paidIds.Contains(p.StatusId)
                    && p.PaymentDate >= from && p.PaymentDate < to)
                .ToListAsync();

            var bookingsInRange = await _context.Bookings
                .Include(b => b.Group).ThenInclude(g => g!.Tour)
                .Where(b => confirmedIds.Contains(b.StatusId)
                    && b.BookingDate >= from && b.BookingDate < to)
                .ToListAsync();

            TotalRevenue = paymentsInRange.Sum(p => p.Amount);
            TotalBookings = bookingsInRange.Count;

            RevenueByTour = paymentsInRange
                .GroupBy(p => p.Booking?.Group?.Tour?.Name ?? "N/A")
                .Select(g => new RevenueByTourVm { TourName = g.Key, Revenue = g.Sum(x => x.Amount), Bookings = g.Count() })
                .OrderByDescending(x => x.Revenue)
                .Take(20)
                .ToList();

            RevenueByMonth = paymentsInRange
                .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                .Select(g => new RevenueByMonthVm
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(x => x.Amount),
                    Bookings = g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            var groups = await _context.TourGroups
                .Include(g => g.Tour)
                .Where(g => g.MaxCapacity > 0)
                .ToListAsync();
            ToursAlmostFull = groups
                .Where(g => g.MaxCapacity > 0 && (double)g.CurrentBookings / g.MaxCapacity >= 0.8)
                .Select(g => new TourAlmostFullVm
                {
                    TourName = g.Tour?.Name ?? "-",
                    GroupId = g.GroupId,
                    DepartDate = g.DepartDate,
                    CurrentBookings = g.CurrentBookings,
                    MaxCapacity = g.MaxCapacity,
                    PercentFull = g.MaxCapacity > 0 ? (int)Math.Round(100.0 * g.CurrentBookings / g.MaxCapacity) : 0
                })
                .OrderByDescending(x => x.PercentFull)
                .Take(15)
                .ToList();

            var completedIds = await _context.Statuses
                .Where(s => new[] { "COMPLETED", "DONE", "FINISHED" }.Contains(s.StatusId))
                .Select(s => s.StatusId)
                .ToListAsync();
            var completedBookings = await _context.Bookings
                .Include(b => b.Group).ThenInclude(g => g!.Tour)
                .Where(b => completedIds.Contains(b.StatusId)
                    && b.BookingDate >= from && b.BookingDate < to)
                .ToListAsync();
            CompletedTours = completedBookings
                .GroupBy(b => b.Group?.Tour?.Name ?? "N/A")
                .Select(g => new CompletedTourVm
                {
                    TourName = g.Key,
                    CompletedCount = g.Count(),
                    Revenue = g.Sum(b => b.TotalPrice)
                })
                .OrderByDescending(x => x.CompletedCount)
                .Take(15)
                .ToList();
        }
    }

    public class RevenueByTourVm
    {
        public string TourName { get; set; } = "";
        public decimal Revenue { get; set; }
        public int Bookings { get; set; }
    }

    public class RevenueByMonthVm
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Revenue { get; set; }
        public int Bookings { get; set; }
        public string Label => $"{Month}/{Year}";
    }

    public class TourAlmostFullVm
    {
        public string TourName { get; set; } = "";
        public string GroupId { get; set; } = "";
        public DateOnly DepartDate { get; set; }
        public int CurrentBookings { get; set; }
        public int MaxCapacity { get; set; }
        public int PercentFull { get; set; }
    }

    public class CompletedTourVm
    {
        public string TourName { get; set; } = "";
        public int CompletedCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
