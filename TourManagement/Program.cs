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

            // Add services...
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



            var app = builder.Build();

            // Middleware quan trọng - phải có thứ tự này
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection(); 
            app.UseStaticFiles();           

            app.UseRouting();

            app.UseAuthentication();         
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
