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

            // Không cho xóa nếu tour đã có đoàn
            if (tour.TourGroups.Any())
            {
                TempData["Error"] = "Không thể xóa tour vì đã có đoàn (TourGroup) liên quan.";
                return RedirectToPage("/Admin/Tours/Index");
            }

            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa tour thành công.";
            return RedirectToPage("/Admin/Tours/Index");
        }
    }
}

