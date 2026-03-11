using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchSync.Models;

namespace MatchSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MatchSyncContext _context;
        public UserController(MatchSyncContext context) => _context = context;

        // Requirement: Dashboard - View Points and Announcements
        [HttpGet("dashboard/{userId}")]
        public async Task<IActionResult> GetDashboard(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var announcements = await _context.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .ToListAsync();

            return Ok(new
            {
                FullName = user.FullName,
                Points = user.LoyaltyPoints,
                Announcements = announcements
            });
        }

        // Requirement: See items and their redeem points
        [HttpGet("rewards")]
        public async Task<IActionResult> GetRedeemableItems()
        {
            var items = await _context.Inventory
                .Where(i => i.RedeemPoints > 0)
                .Select(i => new { i.ItemName, i.RedeemPoints, i.StockQty })
                .ToListAsync();
            return Ok(items);
        }

        // Requirement: Update profile/password
        [HttpPut("profile/update")]
        public async Task<IActionResult> UpdateProfile([FromBody] User updatedUser)
        {
            var user = await _context.Users.FindAsync(updatedUser.UserID);
            if (user == null) return NotFound();

            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;
            if (!string.IsNullOrEmpty(updatedUser.PasswordHash))
                user.PasswordHash = updatedUser.PasswordHash;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Profile updated successfully." });
        }
    }
}