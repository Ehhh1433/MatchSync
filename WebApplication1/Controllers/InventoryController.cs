using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchSync.Models;

namespace MatchSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly MatchSyncContext _context;
        public InventoryController(MatchSyncContext context) => _context = context;

        [HttpGet("scan/{barcode}")]
        public async Task<IActionResult> GetByBarcode(string barcode)
        {
            var item = await _context.Inventory.FirstOrDefaultAsync(i => i.Barcode == barcode);
            return item == null ? NotFound("Item not found.") : Ok(item);
        }

        [HttpPut("update-stock/{id}")]
        public async Task<IActionResult> UpdateStock(int id, int quantityChange)
        {
            var item = await _context.Inventory.FindAsync(id);
            if (item == null) return NotFound();

            if (item.Category == "Retail" && item.StockQuantity + quantityChange < 0)
                return BadRequest("Insufficient stock.");

            item.StockQuantity += quantityChange;
            await _context.SaveChangesAsync();
            return Ok(new { Item = item.ItemName, NewQuantity = item.StockQuantity });
        }
    }
}