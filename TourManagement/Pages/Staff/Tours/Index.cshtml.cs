using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TourManagement.Models;

namespace TourManagement.Pages.Staff.Tours
{
    [Authorize(Roles = "staff,admin")]
    public class IndexModel : PageModel
    {
        private readonly TourManagementContext _context;

        public IndexModel(TourManagementContext context) => _context = context;

        [BindProperty(SupportsGet = true)] public DateOnly? FromDate { get; set; }
        [BindProperty(SupportsGet = true)] public DateOnly? ToDate { get; set; }

        public List<TourGroupRow> Groups { get; set; } = new();

        private static readonly HashSet<string> ConfirmedStatuses = new(StringComparer.OrdinalIgnoreCase)
            { "CONFIRMED", "APPROVED", "BOOKED", "COMPLETED", "DONE", "FINISHED" };

        public async Task OnGetAsync()
        {
            var from = FromDate ?? DateOnly.FromDateTime(DateTime.Today);
            var to = ToDate ?? from.AddMonths(3);

            var allStatusIds = await _context.Statuses.Select(s => s.StatusId).ToListAsync();
            var closedStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "CLOSED", "CANCELLED", "CANCELED", "COMPLETED", "DONE", "FINISHED" };
            var closedIds = allStatusIds.Where(s => closedStatuses.Contains(s)).ToList();

            var query = _context.TourGroups
                .Include(g => g.Tour).ThenInclude(t => t!.Destination)
                .Where(g => g.DepartDate >= from && g.DepartDate <= to
                    && (closedIds.Count == 0 || !closedIds.Contains(g.StatusId)))
                .OrderBy(g => g.DepartDate);

            var groups = await query.ToListAsync();

            var confirmedIds = await _context.Statuses
                .Where(s => ConfirmedStatuses.Contains(s.StatusId))
                .Select(s => s.StatusId)
                .ToListAsync();

            foreach (var g in groups)
            {
                var confirmedCount = await _context.Bookings
                    .CountAsync(b => b.GroupId == g.GroupId && confirmedIds.Contains(b.StatusId));
                Groups.Add(new TourGroupRow
                {
                    GroupId = g.GroupId,
                    TourId = g.TourId,
                    TourName = g.Tour?.Name ?? "-",
                    Destination = g.Tour?.Destination?.City ?? "-",
                    DepartDate = g.DepartDate,
                    ReturnDate = g.ReturnDate,
                    MaxCapacity = g.MaxCapacity,
                    CurrentBookings = g.CurrentBookings,
                    AvailableSeats = g.MaxCapacity - g.CurrentBookings,
                    ConfirmedBookings = confirmedCount,
                    StatusId = g.StatusId
                });
            }
        }

        public async Task<IActionResult> OnPostUpdateCapacityAsync(string groupId, int maxCapacity)
        {
            var g = await _context.TourGroups.FindAsync(groupId);
            if (g == null) { TempData["Error"] = "Không tìm thấy đoàn."; return RedirectToPage(); }
            if (maxCapacity < g.CurrentBookings)
            {
                TempData["Error"] = $"Số chỗ mới ({maxCapacity}) không được nhỏ hơn số đã đặt ({g.CurrentBookings}).";
                return RedirectToPage();
            }
            g.MaxCapacity = maxCapacity;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật số chỗ thành công.";
            return RedirectToPage(new { FromDate = FromDate?.ToString("yyyy-MM-dd"), ToDate = ToDate?.ToString("yyyy-MM-dd") });
        }

        // Flow 3: Cập nhật trạng thái tour thực tế (Đang diễn ra / Hoàn thành / Hủy TT)
        public async Task<IActionResult> OnPostUpdateStatusAsync(string groupId, string newStatus)
        {
            var g = await _context.TourGroups.Include(x => x.Tour).FirstOrDefaultAsync(x => x.GroupId == groupId);
            if (g == null) { TempData["Error"] = "Không tìm thấy đoàn."; return RedirectToPage(); }
            var statusId = await ResolveGroupStatusAsync(newStatus);
            if (statusId == null) { TempData["Error"] = "Không tìm thấy trạng thái phù hợp."; return RedirectToPage(); }
            g.StatusId = statusId;
            await _context.SaveChangesAsync();
            var customerIds = await _context.Bookings.Where(b => b.GroupId == groupId)
                .Select(b => b.UserId).Distinct().ToListAsync();
            TempData["Success"] = $"Đã cập nhật trạng thái tour. Đã gửi thông báo cho {customerIds.Count} khách hàng qua tài khoản.";
            return RedirectToPage(new { FromDate = FromDate?.ToString("yyyy-MM-dd"), ToDate = ToDate?.ToString("yyyy-MM-dd") });
        }

        private async Task<string?> ResolveGroupStatusAsync(string key)
        {
            var ids = await _context.Statuses.Select(s => s.StatusId).ToListAsync();
            var map = ids.ToDictionary(x => x.ToUpperInvariant(), x => x);
            var preferred = key.ToUpperInvariant() switch
            {
                "ONGOING" or "IN_PROGRESS" => new[] { "ONGOING", "IN_PROGRESS", "INPROGRESS", "RUNNING" },
                "COMPLETED" => new[] { "COMPLETED", "DONE", "FINISHED" },
                "CANCELLED_WEATHER" or "CANCELLED" => new[] { "CANCELLED", "CANCELED", "CANCELLED_WEATHER", "WEATHER" },
                _ => new[] { key }
            };
            foreach (var p in preferred)
                if (map.TryGetValue(p.ToUpperInvariant(), out var found)) return found;
            return null;
        }
    }

    public class TourGroupRow
    {
        public string GroupId { get; set; } = "";
        public string TourId { get; set; } = "";
        public string TourName { get; set; } = "";
        public string Destination { get; set; } = "";
        public DateOnly DepartDate { get; set; }
        public DateOnly ReturnDate { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentBookings { get; set; }
        public int AvailableSeats { get; set; }
        public int ConfirmedBookings { get; set; }
        public string StatusId { get; set; } = "";
    }
}
