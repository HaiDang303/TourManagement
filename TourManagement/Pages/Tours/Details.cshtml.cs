using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TourManagement.Models;

namespace TourManagement.Pages.Tours
{
    public class DetailsModel : PageModel
    {
        private readonly TourManagementContext _context;

        public DetailsModel(TourManagementContext context)
        {
            _context = context;
        }

        public Tour? Tour { get; set; }
        public List<TourGroup> OpenGroups { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            Tour = await _context.Tours
                .Include(t => t.Destination)
                .FirstOrDefaultAsync(t => t.TourId == id);

            if (Tour == null)
                return NotFound();

            OpenGroups = await _context.TourGroups
                .Where(g => g.TourId == id && g.StatusId == "OPEN")
                .OrderBy(g => g.DepartDate)
                .ToListAsync();

            return Page();
        }
    }
}