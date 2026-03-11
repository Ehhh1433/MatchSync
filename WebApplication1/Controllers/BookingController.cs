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

        [HttpPost("reserve")]
        public async Task<IActionResult> CreateBooking(Booking newBooking)
        {
            // Validation using the 7 AM - 11 PM range (7 to 23)
            if (newBooking.TimeSlot < 7 || newBooking.TimeSlot > 23)
                return BadRequest("Time slot must be between 7 (7 AM) and 23 (11 PM).");

            if (newBooking.BookingDate < DateTime.Today)
                return BadRequest("Cannot book past dates.");

            // Conflict Logic using specific TimeSlot
            bool isConflict = await _context.Bookings.AnyAsync(b =>
                b.CourtID == newBooking.CourtID &&
                b.BookingDate.Date == newBooking.BookingDate.Date &&
                b.TimeSlot == newBooking.TimeSlot &&
                b.Status != "Cancelled");

            if (isConflict) return BadRequest("This slot is already Reserved or Sold.");

            newBooking.Status = "Reserved";
            newBooking.CreatedAt = DateTime.Now;
            _context.Bookings.Add(newBooking);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Reservation saved!", Slot = newBooking.TimeSlot });
        }

        [HttpGet("daily-court-status")]
        public async Task<IActionResult> GetDailyStatus(DateTime date)
        {
            var dailyBookings = await _context.Bookings
                .Where(b => b.BookingDate.Date == date.Date && b.Status != "Cancelled").ToListAsync();

            var timeSlots = new List<object>();
            // Loop from 7 AM to 11 PM
            for (int hour = 7; hour <= 23; hour++)
            {
                var courtData = Enumerable.Range(1, 6).Select(id => {
                    var b = dailyBookings.FirstOrDefault(x => x.CourtID == id && x.TimeSlot == hour);
                    return new
                    {
                        CourtID = id,
                        // Legend: Available, Reserved, Sold, or Maintenance
                        Label = b == null ? "Available" : b.Status,
                        IsClickable = b == null
                    };
                });
                timeSlots.Add(new { Hour = hour, DisplayTime = $"{hour}:00", Courts = courtData });
            }
            return Ok(timeSlots);
        }
        [HttpGet("daily-status")]
        public async Task<IActionResult> GetStaffCalendar(DateTime date, string sportType)
        {
            var courts = await _context.Courts.Where(c => c.SportType == sportType).ToListAsync();
            var bookings = await _context.Bookings.Where(b => b.BookingDate.Date == date.Date).ToListAsync();

            var timeTable = new List<object>();

            // 7 AM to 11 PM logic
            for (int hour = 7; hour <= 23; hour++)
            {
                var row = new
                {
                    TimeLabel = $"{hour}:00",
                    Courts = courts.Select(c => {
                        var b = bookings.FirstOrDefault(x => x.CourtID == c.CourtID && x.TimeSlot == hour);

                        string legend = "Available";
                        if (!c.IsActive) legend = "Maintenance";
                        else if (b != null)
                        {
                            // Match the specific legends requested
                            legend = b.Status switch
                            {
                                "Completed" => "Sold",
                                "Reserved" => "Reserved",
                                "Selected" => "Selected",
                                _ => "Reserved"
                            };
                        }

                        return new
                        {
                            CourtName = c.CourtName,
                            Status = legend,
                            CanBook = legend == "Available"
                        };
                    })
                };
                timeTable.Add(row);
            }
            return Ok(timeTable);
        }
    }
}