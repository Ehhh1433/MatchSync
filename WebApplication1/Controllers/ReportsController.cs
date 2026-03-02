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

        public ReportsController(MatchSyncContext context)
        {
            _context = context;
        }

        [HttpGet("analytics")]
        public async Task<IActionResult> GetDetailedAnalytics(int month, int year)
        {
            var bookings = await _context.Bookings
                .Where(b => b.BookingDate.Month == month && b.BookingDate.Year == year && b.BookingStatus == "Completed")
                .ToListAsync();

            // Number 2: Grouping by Court type for the Admin's Pie Chart
            var courtBreakdown = bookings
                .GroupBy(b => b.CourtID <= 5 ? "Badminton" : "Volleyball")
                .Select(g => new {
                    Category = g.Key,
                    TotalRevenue = g.Sum(x => x.CourtID <= 5 ? 300 : 500) // Adjust based on your pricing
                });

            return Ok(new { Month = month, Year = year, CourtRevenue = courtBreakdown });
        }
    }
}