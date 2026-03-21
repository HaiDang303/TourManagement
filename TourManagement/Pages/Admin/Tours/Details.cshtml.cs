using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TourManagement.Models;

namespace TourManagement.Pages.Admin.Tours
{
    [Authorize(Roles = "admin")]
    public class DetailsModel : PageModel
    {
        private readonly TourManagementContext _context;

        public DetailsModel(TourManagementContext context) => _context = context;

        public Tour? Tour { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            Tour = await _context.Tours
                .Include(t => t.Destination)
                .Include(t => t.TourGroups)
                .FirstOrDefaultAsync(t => t.TourId == id);
            if (Tour == null) return NotFound();
            return Page();
        }
    }
}
