using Microsoft.EntityFrameworkCore;

namespace MatchSync.Models
{
    public class MatchSyncContext : DbContext
    {
        public MatchSyncContext(DbContextOptions<MatchSyncContext> options) : base(options)
        {
        }

        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<User> Users { get; set; }

        // ADD THIS LINE: This tells the code to connect to your Bookings table
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Inventory>().HasKey(i => i.ItemID);
            modelBuilder.Entity<User>().HasKey(u => u.UserID);

            // This ensures EF Core recognizes your Booking Primary Key
            modelBuilder.Entity<Booking>().HasKey(b => b.BookingID);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ensures ItemID is the Primary Key for scanning
            modelBuilder.Entity<Inventory>().HasKey(i => i.ItemID);

            // Ensures UserID is the Primary Key for Auth
            modelBuilder.Entity<User>().HasKey(u => u.UserID);

            // Ensures BookingID is the Primary Key for Reservations
            modelBuilder.Entity<Booking>().HasKey(b => b.BookingID);
        }
    }
}