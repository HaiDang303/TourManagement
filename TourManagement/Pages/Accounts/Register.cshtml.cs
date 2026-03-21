using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TourManagement.Models;

namespace TourManagement.Pages.Accounts
{
    public class RegisterModel : PageModel
    {
        private readonly TourManagementContext _context;

        public RegisterModel(TourManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập họ tên")]
            [Display(Name = "Họ tên")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập email")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            [DataType(DataType.Password)]
            [MinLength(6, ErrorMessage = "Mật khẩu phải ít nhất 6 ký tự")]
            [Display(Name = "Mật khẩu")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
            [Display(Name = "Xác nhận mật khẩu")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Kiểm tra email đã tồn tại chưa
            var exists = await _context.Users.AnyAsync(u => u.Email == Input.Email);
            if (exists)
            {
                ModelState.AddModelError(string.Empty, "Email đã được sử dụng.");
                return Page();
            }

            // Tạo user mới (mật khẩu để plain text cho đơn giản)
            var user = new User
            {
                Name = Input.Name,
                Email = Input.Email,
                Password = Input.Password,
                IsActive = true,
                CreatedAt = DateTime.Now,
                RoleId = 1

                // tạm cho role mặc định là 1
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Sau khi đăng ký thành công, chuyển sang trang Login
            return RedirectToPage("/Accounts/Login");
        }
    }
}

