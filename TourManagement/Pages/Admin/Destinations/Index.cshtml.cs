using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TourManagement.Models;

namespace TourManagement.Pages.Admin.Destinations
{
    [Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        private readonly TourManagementContext _context;

        public IndexModel(TourManagementContext context) => _context = context;

        public IList<Destination> Destinations { get; set; } = new List<Destination>();

        public async Task OnGetAsync() =>
            Destinations = await _context.Destinations.OrderBy(d => d.Name).ToListAsync();
    }
}
