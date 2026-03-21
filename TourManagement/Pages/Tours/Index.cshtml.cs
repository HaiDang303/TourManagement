using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TourManagement.Models;

namespace TourManagement.Pages.Tours
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly TourManagementContext _context;

        public IndexModel(TourManagementContext context)
        {
            _context = context;
        }

        public IList<Tour> Tours { get; set; } = new List<Tour>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Tours
                .Include(t => t.Destination)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                string keyword = SearchTerm.Trim();

                query = query.Where(t =>
                    t.Name.Contains(keyword) ||
                    (t.Category != null && t.Category.Contains(keyword)) ||
                    (t.Description != null && t.Description.Contains(keyword)) ||
                    (t.Destination != null && t.Destination.Name.Contains(keyword)) ||
                    (t.Destination != null && t.Destination.City.Contains(keyword)) ||
                    (t.Destination != null && t.Destination.Country != null && t.Destination.Country.Contains(keyword))
                );
            }

            Tours = await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}