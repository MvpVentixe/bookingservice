using BookingService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<BookingEntity> bookings { get; set; } = null!;
    }
}
