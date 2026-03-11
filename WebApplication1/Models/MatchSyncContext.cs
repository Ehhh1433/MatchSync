using Microsoft.EntityFrameworkCore;

namespace MatchSync.Models
{
    public class MatchSyncContext : DbContext
    {
        public MatchSyncContext(DbContextOptions<MatchSyncContext> options) : base(options) { }

        // The tables your API can "talk" to
        public DbSet<User> Users { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Announcement> Announcements { get; set; } // This line caused the error
        public DbSet<Message> Messages { get; set; }
        public DbSet<Court> Courts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.UserID);
            modelBuilder.Entity<Inventory>().HasKey(i => i.ItemID);
            modelBuilder.Entity<Booking>().HasKey(b => b.BookingID);
            modelBuilder.Entity<Transaction>().HasKey(t => t.TransID);
            modelBuilder.Entity<Announcement>().HasKey(a => a.AnnounceID);
            modelBuilder.Entity<Message>().HasKey(m => m.MessageID);
            modelBuilder.Entity<Court>().HasKey(c => c.CourtID);

            // Admin Logic: Mapping relationships for History Tracking
            modelBuilder.Entity<Booking>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(b => b.UserID);

            modelBuilder.Entity<Transaction>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.UserID);
        }
    }

    // --- ADDING THE MISSING CLASSES BELOW ---

    public class Announcement
    {
        public int AnnounceID { get; set; }
        public int PostedBy { get; set; } // UserID of the Staff/Admin
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string PostType { get; set; } = "General"; // Promo, Tournament, etc.
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class Court
    {
        public int CourtID { get; set; }
        public string CourtName { get; set; } = string.Empty;
        public string SportType { get; set; } = string.Empty; // Badminton or Volleyball
        public decimal HourlyRate { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class Transaction
    {
        public int TransID { get; set; }
        public int UserID { get; set; }
        public string? TransType { get; set; } // Badminton, Volleyball, Rental, Retail
        public decimal TotalAmount { get; set; }
        public int PointsUsed { get; set; }
        public int PointsEarned { get; set; }
        public DateTime TransDate { get; set; } = DateTime.Now;
    }

    public class Message
    {
        public int MessageID { get; set; }
        public int SenderID { get; set; }
        public int ReceiverID { get; set; }
        public string MessageContent { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}