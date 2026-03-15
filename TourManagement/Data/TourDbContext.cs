using Microsoft.EntityFrameworkCore;
using TourManagement.Models;

namespace TourManagement.Data
{
    public class TourDbContext : DbContext
    {
        public TourDbContext(DbContextOptions<TourDbContext> options) : base(options)
        {
        }

        public DbSet<Tour> Tours { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<TourGroup> TourGroups { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingPassenger> BookingPassengers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<User> Users { get; set; }
    }
}