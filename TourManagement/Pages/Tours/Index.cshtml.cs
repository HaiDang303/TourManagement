using Microsoft.AspNetCore.Mvc;
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
        public string? SearchString { get; set; }
        public string? DestinationId { get; set; }
        public IList<Destination> Destinations { get; set; } = new List<Destination>();

        public async Task OnGetAsync(string search, string destinationId)
        {
            SearchString = search;
            DestinationId = destinationId;

            var query = _context.Tours
                .Include(t => t.Destination)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.Name.Contains(search) || t.TourId.Contains(search));
            }

            if (!string.IsNullOrEmpty(destinationId))
            {
                query = query.Where(t => t.DestinationId == destinationId);
            }

            Tours = await query
                .OrderBy(t => t.Name)
                .ToListAsync();

            Destinations = await _context.Destinations
                .OrderBy(d => d.Name)
                .ToListAsync();
        }
    }
}