using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using TourManagement.Data;           // thay bằng namespace thực tế của DbContext nếu khác
=======
>>>>>>> b001e8fd745f45e1cf0ea34fa229315904f90c76
using TourManagement.Models;

namespace TourManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ────────────────────────────────────────────────
            // 1. Thêm các dịch vụ (Services)
            // ────────────────────────────────────────────────

            // Razor Pages - bắt buộc cho dự án Razor Pages
            builder.Services.AddRazorPages();

<<<<<<< HEAD
            // DbContext với SQL Server + retry logic khi kết nối lỗi tạm thời
            builder.Services.AddDbContext<TourManagementContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null
                    )
                ));

            // Authentication với Cookie
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    options.SlidingExpiration = true;               // gia hạn tự động nếu còn hoạt động
                    options.Cookie.HttpOnly = true;                 // chống XSS
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // chỉ gửi qua HTTPS
                    options.Cookie.SameSite = SameSiteMode.Lax;     // chống CSRF
                    options.Cookie.Name = "TourManagementAuth";     // tên cookie rõ ràng (tùy chọn)
                });
=======
            // DbContext cho dữ liệu domain (Users, Roles tự thiết kế)
            builder.Services.AddDbContext<TourManagement.Models.TourManagementContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("MyCnn")
                ));


            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Accounts/Login";          // Phải match trang login của bạn
        options.AccessDeniedPath = "/Accounts/AccessDenied"; // Optional
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.SlidingExpiration = true;              // Tự renew khi còn 50% thời gian
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Nếu dùng HTTPS
    });
>>>>>>> b001e8fd745f45e1cf0ea34fa229315904f90c76

            // Session (nếu bạn dùng Session trong ứng dụng)
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;              // cần thiết cho GDPR
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            // (Tùy chọn) Nếu sau này cần thêm policy-based authorization
            // builder.Services.AddAuthorization(options => { ... });

            // ────────────────────────────────────────────────
            // 2. Build ứng dụng
            // ────────────────────────────────────────────────
            var app = builder.Build();

            // ────────────────────────────────────────────────
            // 3. Pipeline middleware - thứ tự rất quan trọng
            // ────────────────────────────────────────────────

            // Xử lý lỗi ở môi trường Production
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // HSTS chỉ nên bật ở Production (không bật ở localhost)
                app.UseHsts();
            }

<<<<<<< HEAD
            // Chuyển hướng HTTP → HTTPS
            app.UseHttpsRedirection();
=======
            app.UseHttpsRedirection(); 
            app.UseStaticFiles();           
>>>>>>> b001e8fd745f45e1cf0ea34fa229315904f90c76

            // Phục vụ file tĩnh (css, js, images, favicon...)
            app.UseStaticFiles();

            // Routing
            app.UseRouting();

<<<<<<< HEAD
            // Session phải nằm trước Authentication
            app.UseSession();

            // Authentication → Authorization
            app.UseAuthentication();
=======
            app.UseAuthentication();         
>>>>>>> b001e8fd745f45e1cf0ea34fa229315904f90c76
            app.UseAuthorization();

            // Map tất cả Razor Pages
            app.MapRazorPages();

            // (Tùy chọn) Nếu sau này thêm API controllers
            // app.MapControllers();

            app.Run();
        }
    }
}