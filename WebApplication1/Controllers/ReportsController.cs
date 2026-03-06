using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchSync.Models;

namespace MatchSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly MatchSyncContext _context;
        public ReportsController(MatchSyncContext context) => _context = context;

        [HttpGet("analytics")]
        public async Task<IActionResult> GetAnalytics(int month, int year)
        {
            var bookings = await _context.Bookings
                .Where(b => b.BookingDate.Month == month && b.BookingDate.Year == year &&
                       (b.BookingStatus == "Completed" || b.BookingStatus == "Confirmed"))
                .ToListAsync();

            var stats = bookings.GroupBy(b => b.CourtID <= 5 ? "Badminton" : "Volleyball")
                .Select(g => new {
                    Sport = g.Key,
                    Revenue = g.Count() * (g.Key == "Badminton" ? 250 : 500)
                });

            return Ok(new { Month = month, Year = year, Data = stats });
        }

        [HttpGet("user-proven-status/{userId}")]
        public async Task<IActionResult> GetProvenStatus(int userId)
        {
            var sessions = await _context.Bookings
                .Where(b => b.UserID == userId && b.BookingStatus == "Completed")
                .ToListAsync();

            int qualifying = sessions.Count(s => (s.EndTime - s.StartTime).TotalHours >= 2);
            bool isProven = qualifying >= 5; // The 5-booking hurdle logic

            return Ok(new { TotalQualifying = qualifying, IsProven = isProven });
        }
    }
}