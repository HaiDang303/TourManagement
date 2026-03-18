using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TourManagement.Models;

namespace TourManagement.Pages.Admin.Users
{
    [Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        private readonly TourManagementContext _context;

        public IndexModel(TourManagementContext context)
        {
            _context = context;
        }

        public IList<User> Users { get; set; } = new List<User>();
        public IList<Role> Roles { get; set; } = new List<Role>();

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        // Support querystring "?page=" used by the view links
        [FromQuery(Name = "page")]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string SearchKey { get; set; } = string.Empty;

        public int PageSize { get; } = 10;
        public int TotalPages { get; private set; }
        public int TotalUsers { get; private set; }

        public async Task OnGetAsync()
        {
            var query = _context.Users
                .Include(u => u.Role)
                .OrderBy(u => u.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchKey))
            {
                var searchTerm = SearchKey.ToLower().Trim();
                query = query.Where(u =>
                    u.Name.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    (u.Phone != null && u.Phone.ToLower().Contains(searchTerm)));
            }

            // Normalize page from querystring (?page=) into CurrentPage
            CurrentPage = PageNumber > 0 ? PageNumber : CurrentPage;

            TotalUsers = await query.CountAsync();
            TotalPages = (int)Math.Ceiling((double)TotalUsers / PageSize);
            CurrentPage = Math.Max(1, Math.Min(CurrentPage, TotalPages > 0 ? TotalPages : 1));

            Users = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            Roles = await _context.Roles
                .OrderBy(r => r.RoleName)
                .ToListAsync();
        }

        // ... phần đầu giữ nguyên ...

        public async Task<IActionResult> OnPostToggleActiveAsync(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(currentUserId, out var me) && me == id)
            {
                TempData["Error"] = "Bạn không thể tự khóa tài khoản đang đăng nhập.";
                return RedirectToPage("./Index", new { page = CurrentPage, searchKey = SearchKey });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật trạng thái thành công.";
            return RedirectToPage("./Index", new { page = CurrentPage, searchKey = SearchKey });
        }

        public async Task<IActionResult> OnPostChangeRoleAsync(int id, int roleId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(currentUserId, out var me) && me == id)
            {
                TempData["Error"] = "Bạn không thể tự đổi vai trò của tài khoản đang đăng nhập.";
                return RedirectToPage("./Index", new { page = CurrentPage, searchKey = SearchKey });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var role = await _context.Roles.FindAsync(roleId);
            if (role == null) return NotFound();

            user.RoleId = role.RoleId;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đổi vai trò thành công.";
            return RedirectToPage("./Index", new { page = CurrentPage, searchKey = SearchKey });
        }

        public async Task<IActionResult> OnPostCreateUserAsync(string name, string email, string phone, string password, int roleId)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin bắt buộc.";
                return RedirectToPage("./Index", new { page = CurrentPage, searchKey = SearchKey });
            }

            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                TempData["Error"] = "Email này đã được sử dụng.";
                return RedirectToPage("./Index", new { page = CurrentPage, searchKey = SearchKey });
            }

            var newUser = new User
            {
                Name = name.Trim(),
                Email = email.Trim(),
                Phone = phone?.Trim(),
                Password = password, // NÊN HASH Ở ĐÂY (ví dụ: BCrypt.Net.BCrypt.HashPassword(password))
                RoleId = roleId,
                IsActive = true
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm người dùng thành công.";
            return RedirectToPage("./Index", new { page = 1, searchKey = "" });
        }
    }
}