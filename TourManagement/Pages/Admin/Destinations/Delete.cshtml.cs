using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TourManagement.Models;

namespace TourManagement.Pages.Admin.Destinations
{
    [Authorize(Roles = "admin")]
    public class DeleteModel : PageModel
    {
        private readonly TourManagementContext _context;

        public DeleteModel(TourManagementContext context) => _context = context;

        public Destination? Destination { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Destination = await _context.Destinations.FindAsync(id);
            if (Destination == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var d = await _context.Destinations.Include(x => x.Tours).FirstOrDefaultAsync(x => x.DestinationId == id);
            if (d == null) return NotFound();
            if (d.Tours.Any())
            {
                TempData["Error"] = "Không thể xóa vì đã có tour sử dụng điểm đến này.";
                return RedirectToPage("/Admin/Destinations/Index");
            }
            _context.Destinations.Remove(d);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa thành công.";
            return RedirectToPage("/Admin/Destinations/Index");
        }
    }
}
