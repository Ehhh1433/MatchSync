using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchSync.Models; // Ensure this matches your project namespace

namespace MatchSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly MatchSyncContext _context;

        public InventoryController(MatchSyncContext context)
        {
            _context = context;
        }

        // GET: api/Inventory
        // This is what the Web Portal calls to show the stringing services
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventory()
        {
            // Fetches everything: Yonex packages, labor fees, etc.
            return await _context.Inventory.ToListAsync();
        }
        // GET: api/Inventory/scan/123456789
        [HttpGet("scan/{barcode}")]
        public async Task<IActionResult> GetItemByBarcode(string barcode)
        {
            // This searches your updated Inventory model for a matching barcode
            var item = await _context.Inventory
                .FirstOrDefaultAsync(i => i.Barcode == barcode);

            if (item == null)
            {
                return NotFound("Item not found. Please check the barcode or add it to the system.");
            }

            // Returns the item including its Category (Retail, Rental, or Service)
            return Ok(item);
        }
    }
}