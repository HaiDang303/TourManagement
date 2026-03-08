using Microsoft.EntityFrameworkCore;
using TourManagement.Models;    

namespace TourManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Them RazorPages Service
            builder.Services.AddRazorPages();

            // Them Service ket noi CSDL
            builder.Services.AddDbContext<TourManagementContext>(
                opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn"))
                );
            builder.Services.AddScoped(typeof(TourManagementContext));


            var app = builder.Build();

            // Thiet lap Route cho RazorPages
            app.MapRazorPages();
            //app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}
