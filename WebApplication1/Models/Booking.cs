namespace MatchSync.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public int UserID { get; set; }
        public int CourtID { get; set; }
        public DateTime BookingDate { get; set; }
        public int TimeSlot { get; set; } // 7-23 (7am to 11pm)
        public string Status { get; set; } = "Reserved"; // Reserved, Sold, etc.
        public string PaymentStatus { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}