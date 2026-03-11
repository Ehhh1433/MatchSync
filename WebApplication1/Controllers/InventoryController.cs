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
            // Matches the new Barcode column in SQL
            var item = await _context.Inventory.FirstOrDefaultAsync(i => i.Barcode == barcode);
            return item == null ? NotFound("Item not found.") : Ok(item);
        }

        [HttpPut("update-stock/{id}")]
        public async Task<IActionResult> UpdateStock(int id, int quantityChange)
        {
            var item = await _context.Inventory.FindAsync(id);
            if (item == null) return NotFound();

            // Updated to use 'StockQty' from new SQL
            if (item.StockQty + quantityChange < 0)
                return BadRequest("Insufficient stock.");

            item.StockQty += quantityChange;
            item.LastUpdated = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(new { Item = item.ItemName, NewQuantity = item.StockQty });
        }

        [HttpPut("change-price/{id}")]
        public async Task<IActionResult> ChangePrice(int id, [FromBody] decimal newPrice)
        {
            var item = await _context.Inventory.FindAsync(id);
            if (item == null) return NotFound();

            item.Price = newPrice;
            item.LastUpdated = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Price updated", Item = item.ItemName, NewPrice = item.Price });
        }
    }
}