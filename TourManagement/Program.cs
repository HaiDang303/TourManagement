using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

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


            app.UseHttpsRedirection(); 
            app.UseStaticFiles();           


            // Phục vụ file tĩnh (css, js, images, favicon...)
            app.UseStaticFiles();

            // Routing
            app.UseRouting();


            app.UseAuthentication();         

            app.UseAuthorization();

            // Map tất cả Razor Pages
            app.MapRazorPages();

            // (Tùy chọn) Nếu sau này thêm API controllers
            // app.MapControllers();

            app.Run();
        }
    }
}