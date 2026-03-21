using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TourManagement.Models;

namespace TourManagement.Pages.Admin.Destinations
{
    [Authorize(Roles = "admin")]
    public class CreateModel : PageModel
    {
        private readonly TourManagementContext _context;

        public CreateModel(TourManagementContext context) => _context = context;

        [BindProperty] public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Bắt buộc")]
            public string Name { get; set; } = "";
            [Required(ErrorMessage = "Bắt buộc")]
            public string City { get; set; } = "";
            public string? Country { get; set; }
            public string? Description { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var id = $"DEST{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
            var d = new Destination { DestinationId = id, Name = Input.Name.Trim(), City = Input.City.Trim(), Country = Input.Country?.Trim(), Description = Input.Description?.Trim() };
            _context.Destinations.Add(d);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm điểm đến thành công.";
            return RedirectToPage("/Admin/Destinations/Index");
        }
    }
}
