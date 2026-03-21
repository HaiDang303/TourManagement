using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TourManagement.Data;
using TourManagement.Models;

namespace TourManagement.Pages.Tours
{
    public class IndexModel : PageModel
    {
        private readonly TourManagementContext _context;

        public IndexModel(TourManagementContext context)
        {
            _context = context;
        }

        public IList<Tour> Tours { get; set; } = new List<Tour>();

        public async Task OnGetAsync()
        {
            Tours = await _context.Tours
                .Include(t => t.Destination)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}