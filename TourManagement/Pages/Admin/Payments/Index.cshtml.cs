using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TourManagement.Models;

namespace TourManagement.Pages.Admin.Payments
{
    [Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        private readonly TourManagementContext _context;

        public IndexModel(TourManagementContext context) => _context = context;

        public List<PaymentVm> PendingPayments { get; set; } = new();
        public List<PaymentVm> AllPayments { get; set; } = new();
        public string Tab { get; set; } = "pending";

        public static readonly HashSet<string> PaidStatusSet = new(StringComparer.OrdinalIgnoreCase)
            { "PAID", "SUCCESS", "COMPLETED", "CONFIRMED" };

        public async Task OnGetAsync(string? tab = null)
        {
            Tab = tab ?? "pending";
            var all = await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b!.Group)
                    .ThenInclude(g => g!.Tour)
                .Include(p => p.Booking)
                    .ThenInclude(b => b!.User)
                .Include(p => p.Status)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var pending = all.Where(p => !PaidStatusSet.Contains(p.StatusId ?? "")).ToList();
            PendingPayments = pending.Select(Map).ToList();
            AllPayments = all.Select(Map).ToList();
        }

        public async Task<IActionResult> OnPostConfirmAsync(string paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) { TempData["Error"] = "Không tìm thấy thanh toán."; return RedirectToPage(); }
            var paidId = await _context.Statuses
                .Where(s => PaidStatusSet.Contains(s.StatusId))
                .Select(s => s.StatusId)
                .FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(paidId))
                paidId = (await _context.Statuses.FirstOrDefaultAsync(s => s.StatusId == "PAID"))?.StatusId ?? "PAID";
            payment.StatusId = paidId;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xác nhận thanh toán.";
            return RedirectToPage(new { tab = "pending" });
        }

        private static PaymentVm Map(Payment p)
        {
            var b = p.Booking;
            var g = b?.Group;
            var t = g?.Tour;
            return new PaymentVm
            {
                PaymentId = p.PaymentId,
                BookingId = p.BookingId,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                Method = p.Method,
                TransactionRef = p.TransactionRef,
                StatusId = p.StatusId,
                StatusName = p.Status?.StatusName ?? p.StatusId,
                IsPaid = PaidStatusSet.Contains(p.StatusId ?? ""),
                CustomerName = b?.User?.Name ?? "-",
                TourName = t?.Name ?? "-",
                DepartDate = g?.DepartDate
            };
        }
    }

    public class PaymentVm
    {
        public string PaymentId { get; set; } = "";
        public string BookingId { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Method { get; set; } = "";
        public string? TransactionRef { get; set; }
        public string StatusId { get; set; } = "";
        public string StatusName { get; set; } = "";
        public bool IsPaid { get; set; }
        public string CustomerName { get; set; } = "";
        public string TourName { get; set; } = "";
        public DateOnly? DepartDate { get; set; }
    }
}
