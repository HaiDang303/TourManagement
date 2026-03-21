using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourManagement.Models;

namespace TourManagement.Pages.Admin.Tours
{
    [Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        private readonly TourManagementContext _context;

        public IndexModel(TourManagementContext context)
        {
            _context = context;
        }

        public IList<TourRow> Rows { get; set; } = new List<TourRow>();

        public class TourRow
        {
            public int Stt { get; set; }
            public string TourId { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string? ImageUrl { get; set; }
            public string? Description { get; set; }
            public decimal Price { get; set; }
            public string? Category { get; set; }
            public string? City { get; set; }
        }

        public async Task OnGetAsync()
        {
            var tours = await _context.Tours
                .Include(t => t.Destination)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            Rows = tours.Select((t, idx) =>
            {

                return new TourRow
                {
                    Stt = idx + 1,
                    TourId = t.TourId,
                    Name = t.Name,
                    ImageUrl = t.ImageUrl,
                    Description = t.Description,
                    Price = t.BasePrice,
                    Category = t.Category,
                    City = t.Destination?.City
                };
            }).ToList();
        }
    }
}

