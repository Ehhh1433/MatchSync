using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchSync.Models;

namespace MatchSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly MatchSyncContext _context;
        public StaffController(MatchSyncContext context) => _context = context;

        // POS System: Process a sale/rental via Barcode
        [HttpPost("pos/transaction")]
        public async Task<IActionResult> ProcessTransaction([FromBody] POSRequest request)
        {
            if (string.IsNullOrEmpty(request.Barcode)) return BadRequest("Barcode is required.");

            var item = await _context.Inventory.FirstOrDefaultAsync(i => i.Barcode == request.Barcode);

            // Fix for CS8602: Ensure 'item' is not null before accessing Category/Price
            if (item == null) return NotFound("Item not found in inventory.");

            if (item.StockQty < request.Quantity) return BadRequest("Insufficient stock.");

            item.StockQty -= request.Quantity;

            var trans = new Transaction
            {
                UserID = request.UserID,
                TransType = item.Category ?? "Retail", // Use null-coalescing
                TotalAmount = item.Price * request.Quantity,
                TransDate = DateTime.Now
            };

            _context.Transactions.Add(trans);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Transaction Successful", ReceiptTotal = trans.TotalAmount });
        }

        // Post Announcements (Promos/Tournaments)
        [HttpPost("announcements")]
        public async Task<IActionResult> PostAnnouncement([FromBody] Announcement post)
        {
            post.CreatedAt = DateTime.Now;
            _context.Announcements.Add(post);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Announcement published." });
        }

        // Search User Accounts
        [HttpGet("users/search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string? query)
        {
            // Fix for CS8602: Check if query is null
            if (string.IsNullOrWhiteSpace(query)) return BadRequest("Search query cannot be empty.");

            var results = await _context.Users
                .Where(u => u.RoleID == 2 &&
                           (u.FullName.Contains(query) || (u.PhoneNumber != null && u.PhoneNumber.Contains(query))))
                .Select(u => new { u.UserID, u.FullName, u.LoyaltyPoints, u.AccountStatus })
                .ToListAsync();

            return Ok(results);
        }
    }

    public class POSRequest
    {
        public string Barcode { get; set; } = string.Empty;
        public int UserID { get; set; }
        public int Quantity { get; set; }
    }
}