using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourManagement.Models;

namespace TourManagement.Pages.Staff.Bookings
{
    [Authorize(Roles = "staff,admin")]
    public class IndexModel : PageModel
    {
        private readonly TourManagementContext _context;

        public IndexModel(TourManagementContext context)
        {
            _context = context;
        }

        public IList<Booking> Bookings { get; set; } = new List<Booking>();
        public IList<Tour> Tours { get; set; } = new List<Tour>();
        public string? SelectedTourId { get; set; }
        public IList<BookingPassenger> Passengers { get; set; } = new List<BookingPassenger>();

        public HashSet<string> PaidStatusSet { get; } = new HashSet<string> { "PAID", "SUCCESS", "COMPLETED", "CONFIRMED" };

        public async Task OnGetAsync(string? tourId = null)
        {
            Tours = await _context.Tours.Include(t => t.Destination).OrderBy(t => t.Name).ToListAsync();
            SelectedTourId = tourId;

            var query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Group).ThenInclude(g => g.Tour)
                .Include(b => b.Payments)
                .OrderByDescending(b => b.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrEmpty(tourId))
            {
                var groupIds = await _context.TourGroups.Where(g => g.TourId == tourId).Select(g => g.GroupId).ToListAsync();
                query = query.Where(b => groupIds.Contains(b.GroupId));
            }

            Bookings = await query.ToListAsync();

            // Flow 4: Load passengers khi chọn tour
            if (!string.IsNullOrEmpty(tourId))
            {
                var groupIds = await _context.TourGroups.Where(g => g.TourId == tourId).Select(g => g.GroupId).ToListAsync();
                var bookingIds = await _context.Bookings.Where(b => groupIds.Contains(b.GroupId)).Select(b => b.BookingId).ToListAsync();
                Passengers = await _context.BookingPassengers
                    .Include(p => p.Booking).ThenInclude(b => b!.User)
                    .Include(p => p.Category)
                    .Where(p => bookingIds.Contains(p.BookingId))
                    .OrderBy(p => p.FullName)
                    .ToListAsync();
            }
        }

        // Flow 2: Xác nhận booking (đã thanh toán)
        public async Task<IActionResult> OnPostConfirmAsync(string bookingId)
        {
            var booking = await _context.Bookings.Include(b => b.Payments).FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
            {
                TempData["Error"] = "Không tìm thấy booking.";
                return RedirectToPage();
            }
            var isPaid = booking.Payments.Any(p => PaidStatusSet.Contains((p.StatusId ?? "").ToUpperInvariant()));
            if (!isPaid)
            {
                TempData["Error"] = "Booking chưa thanh toán, giữ nguyên Pending.";
                return RedirectToPage();
            }
            var confirmedId = await ResolveStatusIdAsync("CONFIRMED", "APPROVED", "BOOKED");
            if (confirmedId == null) { TempData["Error"] = "Không tìm thấy status CONFIRMED."; return RedirectToPage(); }
            booking.StatusId = confirmedId;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xác nhận booking thành công.";
            return RedirectToPage();
        }

        // Flow 3: Xác nhận thanh toán - Update payment = PAID, gửi thông báo
        public async Task<IActionResult> OnPostConfirmPaymentAsync(string paymentId)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking).ThenInclude(b => b!.User)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
            if (payment == null)
            {
                TempData["Error"] = "Không tìm thấy payment.";
                return RedirectToPage();
            }
            var paidId = await ResolveStatusIdAsync("PAID", "SUCCESS", "COMPLETED", "CONFIRMED");
            if (paidId == null) { TempData["Error"] = "Không tìm thấy status PAID trong Statuses."; return RedirectToPage(); }
            payment.StatusId = paidId;
            await _context.SaveChangesAsync();
            var customerName = payment.Booking?.User?.Name ?? "Khách hàng";
            TempData["Success"] = $"Đã xác nhận thanh toán. Đã gửi thông báo cho {customerName} (email: {payment.Booking?.User?.Email}).";
            return RedirectToPage();
        }

        // Flow 5: Hoàn thành tour - Update booking status = Completed
        public async Task<IActionResult> OnPostCompleteAsync(string bookingId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null) { TempData["Error"] = "Không tìm thấy booking."; return RedirectToPage(); }
            var completedId = await ResolveStatusIdAsync("COMPLETED", "DONE", "FINISHED");
            if (completedId == null) { TempData["Error"] = "Không tìm thấy status COMPLETED."; return RedirectToPage(); }
            booking.StatusId = completedId;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã đánh dấu booking hoàn thành.";
            return RedirectToPage();
        }

        // Flow 6: Xử lý hủy - Update status = Cancelled
        public async Task<IActionResult> OnPostCancelAsync(string bookingId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null) { TempData["Error"] = "Không tìm thấy booking."; return RedirectToPage(); }
            var cancelledId = await ResolveStatusIdAsync("CANCELLED", "CANCELED", "CANCEL");
            if (cancelledId == null) { TempData["Error"] = "Không tìm thấy status CANCELLED."; return RedirectToPage(); }
            booking.StatusId = cancelledId;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xác nhận hủy booking.";
            return RedirectToPage();
        }

        private async Task<string?> ResolveStatusIdAsync(params string[] preferred)
        {
            var ids = await _context.Statuses.Select(s => s.StatusId).ToListAsync();
            var map = ids.ToDictionary(x => x.ToUpperInvariant(), x => x);
            foreach (var p in preferred)
                if (map.TryGetValue(p.ToUpperInvariant(), out var found)) return found;
            return null;
        }
    }
}
