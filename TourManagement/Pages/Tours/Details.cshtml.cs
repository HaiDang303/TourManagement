using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TourManagement.Data;           // thay bằng namespace DbContext của bạn
using TourManagement.Models;         // namespace chứa entity Tour, TourGroup,...

namespace TourManagement.Pages.Tours
{
    public class DetailsModel : PageModel
    {
        private readonly TourManagementContext _context;

        public DetailsModel(TourManagementContext context)
        {
            _context = context;
        }

        // Property BẮT BUỘC phải public và có getter
        public Tour? Tour { get; set; }     // ← đây là property bạn cần

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            Tour = await _context.Tours
                .Include(t => t.Destination)
                .Include(t => t.TourGroups)
                    .ThenInclude(g => g.Status)   // nếu có navigation Status
                .FirstOrDefaultAsync(t => t.TourId == id);

            if (Tour == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}