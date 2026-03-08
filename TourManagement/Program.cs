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

            // ĐĂNG KÝ DbContext - Đây là phần thiếu!
            builder.Services.AddDbContext<TourManagement.Models.TourManagementContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")  // tên connection string trong appsettings.json
                ));

            // Nếu bạn dùng authentication cookie (như code login trước đó)
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    // ... các config khác
                });



            var app = builder.Build();

            // Middleware quan trọng - phải có thứ tự này
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();       // ← Nên có
            app.UseStaticFiles();            // ← BẮT BUỘC phải có dòng này để load wwwroot

            app.UseRouting();

            app.UseAuthentication();         // Nếu bạn có login
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
