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

        // Requirement: Analytics with Type and Date Range filtering (e.g., Mar 1 - Mar 7)
        [HttpGet("analytics")]
        public async Task<IActionResult> GetAnalytics(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? type) // 'Badminton', 'Volleyball', 'Rental', 'Retail'
        {
            var query = _context.Transactions.AsQueryable();

            if (startDate.HasValue) query = query.Where(t => t.TransDate >= startDate.Value);
            if (endDate.HasValue) query = query.Where(t => t.TransDate <= endDate.Value);
            if (!string.IsNullOrEmpty(type)) query = query.Where(t => t.TransType == type);

            var data = await query.ToListAsync();

            // Formatted for Visual Graphs (Grouped by Date)
            var graphData = data.GroupBy(t => t.TransDate.Date)
                .Select(g => new {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    TotalSales = g.Sum(x => x.TotalAmount)
                }).OrderBy(x => x.Date);

            return Ok(new
            {
                Filters = new { startDate, endDate, type },
                TotalRevenue = data.Sum(x => x.TotalAmount),
                ChartData = graphData
            });
        }

        // Requirement: View All Staff & Users + Soft Delete
        [HttpGet("accounts")]
        public async Task<IActionResult> GetAccounts(int roleId) // 1 for Staff, 2 for User
        {
            var accounts = await _context.Users
                .Where(u => u.RoleID == roleId && u.AccountStatus == "Active")
                .Select(u => new {
                    u.UserID,
                    u.FullName,
                    u.LoyaltyPoints,
                    u.Username
                }).ToListAsync();
            return Ok(accounts);
        }

        [HttpDelete("accounts/{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.AccountStatus = "Deleted"; // Soft delete to keep historical logs
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Account deactivated." });
        }

        // Requirement: User History (Reserved courts and redeemed items)
        [HttpGet("user-history/{userId}")]
        public async Task<IActionResult> GetUserHistory(int userId)
        {
            var bookings = await _context.Bookings.Where(b => b.UserID == userId).ToListAsync();
            var redemptions = await _context.Transactions
                .Where(t => t.UserID == userId && t.TransType == "Redeemed").ToListAsync();

            return Ok(new { Bookings = bookings, Redemptions = redemptions });
        }
    }
}