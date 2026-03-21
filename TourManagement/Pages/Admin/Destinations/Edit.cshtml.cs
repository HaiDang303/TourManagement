using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TourManagement.Models;

namespace TourManagement.Pages.Admin.Destinations
{
    [Authorize(Roles = "admin")]
    public class EditModel : PageModel
    {
        private readonly TourManagementContext _context;

        public EditModel(TourManagementContext context) => _context = context;

        [BindProperty(SupportsGet = true)] public string Id { get; set; } = "";
        [BindProperty] public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required] public string Name { get; set; } = "";
            [Required] public string City { get; set; } = "";
            public string? Country { get; set; }
            public string? Description { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Id = id;
            var d = await _context.Destinations.FindAsync(id);
            if (d == null) return NotFound();
            Input = new InputModel { Name = d.Name, City = d.City, Country = d.Country, Description = d.Description };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? id)
        {
            id ??= Id;
            if (string.IsNullOrEmpty(id)) return NotFound();
            var d = await _context.Destinations.FindAsync(id);
            if (d == null) return NotFound();
            if (!ModelState.IsValid) return Page();
            d.Name = Input.Name.Trim();
            d.City = Input.City.Trim();
            d.Country = Input.Country?.Trim();
            d.Description = Input.Description?.Trim();
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật thành công.";
            return RedirectToPage("/Admin/Destinations/Index");
        }
    }
}
