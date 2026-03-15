using BCrypt.Net;  // ← thêm dòng này sau khi install package
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using TourManagement.Data;      // sửa nếu DbContext tên khác
using TourManagement.Models;

namespace TourManagement.Pages.Account   // ← nếu dùng folder Account
// namespace TourManagement.Pages        // ← nếu không dùng folder Account
{
    public class LoginModel : PageModel
    {
        private readonly TourManagementContext _context;

        public LoginModel(TourManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập email")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu")]
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == Input.Email && u.IsActive);

            if (user == null || !BCrypt.Net.BCrypt.Verify(Input.Password, user.Password))
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
                return Page();
            }

            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name ?? user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12),
                IsPersistent = false
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return LocalRedirect(ReturnUrl);
        }
    }
}