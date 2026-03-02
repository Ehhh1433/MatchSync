namespace MatchSync.Models
{
    public class Booking
    {
        public int BookingID { get; set; }

        // Use '?' because SQL generates this automatically (e.g., MS-1)
        public string? ReferenceCode { get; set; }

        public int UserID { get; set; }
        public int CourtID { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string BookingStatus { get; set; } = "Pending";
        public Guid QRCodeToken { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
