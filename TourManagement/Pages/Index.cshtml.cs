using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TourManagement.Models;

namespace TourManagement.Pages
{
    public class IndexModel : PageModel
    {
        private readonly TourManagementContext _context;

        public IndexModel(TourManagementContext context) => _context = context;

        public List<FeaturedTour> FeaturedTours { get; set; } = new();

        public async Task OnGetAsync()
        {
            var tours = await _context.Tours
                .Include(t => t.Destination)
                .Include(t => t.TourGroups)
                .Where(t => t.TourGroups.Any())
                .OrderByDescending(t => t.CreatedAt)
                .Take(4)
                .ToListAsync();

            FeaturedTours = tours.Select(t => new FeaturedTour
            {
                TourId = t.TourId,
                Name = t.Name,
                Description = t.Description,
                DurationDays = t.DurationDays,
                BasePrice = t.BasePrice,
                ImageUrl = t.ImageUrl,
                City = t.Destination?.City ?? "",
                Country = t.Destination?.Country ?? "",
                MaxPax = t.TourGroups.OrderBy(g => g.DepartDate).First().MaxCapacity
            }).ToList();
        }

        public class FeaturedTour
        {
            public string TourId { get; set; } = "";
            public string Name { get; set; } = "";
            public string? Description { get; set; }
            public int DurationDays { get; set; }
            public decimal BasePrice { get; set; }
            public string? ImageUrl { get; set; }
            public string City { get; set; } = "";
            public string? Country { get; set; }
            public int MaxPax { get; set; }
        }
    }
}
