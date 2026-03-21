using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TourManagement.Models;

namespace TourManagement.Pages.Admin.Tours
{
    [Authorize(Roles = "admin")]
    public class DeleteModel : PageModel
    {
        private readonly TourManagementContext _context;

        public DeleteModel(TourManagementContext context)
        {
            _context = context;
        }

        public Tour? Tour { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Tour = await _context.Tours
                .Include(t => t.Destination)
                .FirstOrDefaultAsync(t => t.TourId == id);

            if (Tour == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var tour = await _context.Tours
                .Include(t => t.TourGroups)
                .FirstOrDefaultAsync(t => t.TourId == id);

            if (tour == null)
            {
                return NotFound();
            }

            // Không cho xóa nếu đã có booking liên quan bất kỳ group nào
            var groupIds = tour.TourGroups.Select(g => g.GroupId).ToList();
            var hasBookings = await _context.Bookings.AnyAsync(b => groupIds.Contains(b.GroupId));
            if (hasBookings)
            {
                TempData["Error"] = "Không thể xóa tour vì đã có booking liên quan.";
                return RedirectToPage("/Admin/Tours/Index");
            }

            // Nếu chỉ có group rỗng (chưa booking), xóa group trước rồi xóa tour
            if (tour.TourGroups.Any())
            {
                _context.TourGroups.RemoveRange(tour.TourGroups);
            }

            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa tour thành công.";
            return RedirectToPage("/Admin/Tours/Index");
        }
    }
}

