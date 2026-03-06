using MatchSync.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MatchSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly MatchSyncContext _context;
        public BookingController(MatchSyncContext context) => _context = context;

        [HttpGet("calculate-badminton")]
        public IActionResult GetBadmintonRate(int hours) => Ok(new { TotalPrice = hours * 250.00m, Currency = "PHP" });

        [HttpGet("calculate-volleyball")]
        public IActionResult GetVolleyballRate(DateTime startTime)
        {
            decimal rate = (startTime.Hour >= 8 && startTime.Hour < 14) ? 500.00m : 650.00m;
            return Ok(new { Rate = rate, Currency = "PHP" });
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> CreateBooking(Booking newBooking)
        {
            if (newBooking.StartTime.Hours < 7 || newBooking.EndTime.Hours > 23)
                return BadRequest("Match Point is open 7 AM - 11 PM.");

            if (newBooking.BookingDate < DateTime.Today)
                return BadRequest("Cannot book past dates.");

            // Overlap/Conflict Logic: Checks if any part of the new time hits an existing booking
            bool isConflict = await _context.Bookings.AnyAsync(b =>
                b.CourtID == newBooking.CourtID && b.BookingDate.Date == newBooking.BookingDate.Date &&
                b.BookingStatus != "Cancelled" &&
                ((newBooking.StartTime >= b.StartTime && newBooking.StartTime < b.EndTime) ||
                 (newBooking.EndTime > b.StartTime && newBooking.EndTime <= b.EndTime) ||
                 (newBooking.StartTime <= b.StartTime && newBooking.EndTime >= b.EndTime)));

            if (isConflict) return BadRequest("Time-Conflict: One or more slots are already Reserved or Sold.");

            newBooking.BookingStatus = "Pending";
            newBooking.CreatedAt = DateTime.Now;
            _context.Bookings.Add(newBooking);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Reservation saved!", Reference = newBooking.ReferenceCode });
        }

        [HttpGet("daily-court-status")]
        public async Task<IActionResult> GetDailyStatus(DateTime date)
        {
            // Auto-Cancel: Frees up courts if Pending > 30 mins
            var expired = await _context.Bookings
                .Where(b => b.BookingStatus == "Pending" && b.CreatedAt < DateTime.Now.AddMinutes(-30))
                .ToListAsync();
            expired.ForEach(b => b.BookingStatus = "Cancelled");
            await _context.SaveChangesAsync();

            var dailyBookings = await _context.Bookings
                .Where(b => b.BookingDate.Date == date.Date && b.BookingStatus != "Cancelled").ToListAsync();

            var timeSlots = new List<object>();
            for (int hour = 7; hour < 23; hour++)
            {
                var slotTime = new TimeSpan(hour, 0, 0);
                var courtData = Enumerable.Range(1, 6).Select(id => {
                    var b = dailyBookings.FirstOrDefault(x => x.CourtID == id && slotTime >= x.StartTime && slotTime < x.EndTime);
                    return new
                    {
                        CourtID = id,
                        Label = b == null ? "Available" : (b.BookingStatus == "Completed" ? "Sold" : "Reserved"),
                        IsClickable = b == null
                    };
                });
                timeSlots.Add(new { Time = slotTime.ToString(@"hh\:mm"), Courts = courtData });
            }
            return Ok(timeSlots);
        }

        [HttpPost("walk-in-sale")]
        public async Task<IActionResult> CreateWalkIn(Booking walkIn)
        {
            walkIn.BookingStatus = "Completed"; // Logic for 'Sold' status
            walkIn.CreatedAt = DateTime.Now;
            _context.Bookings.Add(walkIn);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Walk-in sale completed.", Reference = walkIn.ReferenceCode });
        }

        [HttpPost("confirm-payment/{bookingId}")]
        public async Task<IActionResult> ConfirmPayment(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return NotFound();
            booking.BookingStatus = "Confirmed";
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Payment Confirmed!", Status = booking.BookingStatus });
        }
    }
}