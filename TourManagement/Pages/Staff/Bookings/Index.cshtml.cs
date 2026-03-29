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
        public string? StatusFilter { get; set; } // "pending" | "confirmed" | null = all
        public IList<BookingPassenger> Passengers { get; set; } = new List<BookingPassenger>();

        public HashSet<string> PaidStatusSet { get; } = new HashSet<string> { "PAID", "SUCCESS", "COMPLETED", "CONFIRMED" };

        public async Task OnGetAsync(string? tourId = null, string? status = null)
        {
            Tours = await _context.Tours.Include(t => t.Destination).OrderBy(t => t.Name).ToListAsync();
            SelectedTourId = tourId;
            StatusFilter = status;

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
            if (status == "pending")
                query = query.Where(b => b.StatusId == "PENDING");
            else if (status == "confirmed")
                query = query.Where(b => new[] { "CONFIRMED", "APPROVED", "BOOKED" }.Contains(b.StatusId));

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
            var booking = await _context.Bookings
                .Include(b => b.Group)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
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
            var confirmedId = await ResolveBookingStatusIdAsync("CONFIRMED", "APPROVED", "BOOKED");
            if (confirmedId == null) { TempData["Error"] = "Không tìm thấy status CONFIRMED."; return RedirectToPage(); }
            
            if (booking.StatusId != confirmedId)
            {
                int totalPax = booking.Adults + booking.Children + booking.Infants;
                int remain = booking.Group.MaxCapacity - booking.Group.CurrentBookings;
                
                if (totalPax > remain)
                {
                    TempData["Error"] = $"Không đủ chỗ trống để duyệt! (Còn: {remain}, Yêu cầu: {totalPax})";
                    return RedirectToPage();
                }

                booking.StatusId = confirmedId;
                booking.Group.CurrentBookings += totalPax;
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Đã duyệt và xác nhận booking thành công.";
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
            var paidId = await ResolvePaymentStatusIdAsync("PAID", "SUCCESS", "COMPLETED", "CONFIRMED");
            if (paidId == null) { TempData["Error"] = "Không tìm thấy status PAID trong PaymentStatuses."; return RedirectToPage(); }
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
            var completedId = await ResolveBookingStatusIdAsync("COMPLETED", "DONE", "FINISHED");
            if (completedId == null) { TempData["Error"] = "Không tìm thấy status COMPLETED."; return RedirectToPage(); }
            booking.StatusId = completedId;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã đánh dấu booking hoàn thành.";
            return RedirectToPage();
        }

        // Flow 6: Xử lý hủy - Update status = Cancelled (đã thanh toán → chuyển Admin)
        public async Task<IActionResult> OnPostCancelAsync(string bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Group)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null) { TempData["Error"] = "Không tìm thấy booking."; return RedirectToPage(); }

            var isPaid = booking.Payments.Any(p => PaidStatusSet.Contains((p.StatusId ?? "").ToUpperInvariant()));
            if (isPaid && booking.Group?.StatusId != "OPEN")
            {
                TempData["Error"] = "Booking đã thanh toán và Tour không còn ở trạng thái Mở. Vui lòng chuyển Admin để phê duyệt hoàn tiền.";
                return RedirectToPage();
            }

            var cancelledId = await ResolveBookingStatusIdAsync("CANCELLED", "CANCELED", "CANCEL");
            if (cancelledId == null) { TempData["Error"] = "Không tìm thấy status CANCELLED."; return RedirectToPage(); }

            if (booking.StatusId != cancelledId)
            {
                var activeStatusIds = new HashSet<string> { "CONFIRMED", "APPROVED", "BOOKED", "COMPLETED", "DONE", "FINISHED" };
                bool wasTakingSeats = booking.StatusId != null && activeStatusIds.Contains(booking.StatusId.ToUpperInvariant());
                booking.StatusId = cancelledId;
                if (wasTakingSeats)
                {
                    int totalPax = booking.Adults + booking.Children + booking.Infants;
                    booking.Group.CurrentBookings -= totalPax;
                    if (booking.Group.CurrentBookings < 0) booking.Group.CurrentBookings = 0;
                }
                await _context.SaveChangesAsync();
            }
            TempData["Success"] = "Đã xác nhận hủy booking.";
            return RedirectToPage();
        }

        // Từ chối booking (hết chỗ hoặc lý do khác) - gửi lý do qua Notes
        public async Task<IActionResult> OnPostRejectAsync(string bookingId, string rejectReason)
        {
            var booking = await _context.Bookings
                .Include(b => b.Group)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null) { TempData["Error"] = "Không tìm thấy booking."; return RedirectToPage(); }
            var rejectedId = await ResolveBookingStatusIdAsync("REJECTED", "DENIED", "CANCELLED", "CANCELED");
            if (rejectedId == null) rejectedId = await ResolveBookingStatusIdAsync("CANCELLED", "CANCELED");
            if (rejectedId == null) { TempData["Error"] = "Không tìm thấy status từ chối."; return RedirectToPage(); }

            if (booking.StatusId != rejectedId)
            {
                var activeStatusIds = new HashSet<string> { "CONFIRMED", "APPROVED", "BOOKED", "COMPLETED", "DONE", "FINISHED" };
                bool wasTakingSeats = booking.StatusId != null && activeStatusIds.Contains(booking.StatusId.ToUpperInvariant());

                booking.StatusId = rejectedId;
                booking.Notes = (booking.Notes ?? "") + (string.IsNullOrWhiteSpace(rejectReason) ? "" : " [Từ chối: " + rejectReason.Trim() + "]");

                if (wasTakingSeats && booking.Group != null)
                {
                    int totalPax = booking.Adults + booking.Children + booking.Infants;
                    booking.Group.CurrentBookings -= totalPax;
                    if (booking.Group.CurrentBookings < 0) booking.Group.CurrentBookings = 0;
                }
                await _context.SaveChangesAsync();
            }
            TempData["Success"] = "Đã từ chối booking. Lý do đã được lưu và sẽ gửi đến khách hàng.";
            return RedirectToPage();
        }

        private async Task<string?> ResolveBookingStatusIdAsync(params string[] preferred)
        {
            var ids = await _context.BookingStatuses.Select(s => s.StatusId).ToListAsync();
            var map = ids.ToDictionary(x => x.ToUpperInvariant(), x => x);
            foreach (var p in preferred)
                if (map.TryGetValue(p.ToUpperInvariant(), out var found)) return found;
            return null;
        }

        private async Task<string?> ResolvePaymentStatusIdAsync(params string[] preferred)
        {
            var ids = await _context.PaymentStatuses.Select(s => s.StatusId).ToListAsync();
            var map = ids.ToDictionary(x => x.ToUpperInvariant(), x => x);
            foreach (var p in preferred)
                if (map.TryGetValue(p.ToUpperInvariant(), out var found)) return found;
            return null;
        }
    }
}
