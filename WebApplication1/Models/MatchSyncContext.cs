using Microsoft.EntityFrameworkCore;

namespace MatchSync.Models
{
    public class MatchSyncContext : DbContext
    {
        public MatchSyncContext(DbContextOptions<MatchSyncContext> options) : base(options)
        {
        }

        // The tables your API can "talk" to
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        // MERGED: This single method tells SQL which columns are the "Keys"
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Primary Key for Inventory (Retail, Rental, Service)
            modelBuilder.Entity<Inventory>().HasKey(i => i.ItemID);

            // Primary Key for Users (Admin, Staff, Customer)
            modelBuilder.Entity<User>().HasKey(u => u.UserID);

            // Primary Key for Reservations (Match Point bookings)
            modelBuilder.Entity<Booking>().HasKey(b => b.BookingID);
        }
    }
}