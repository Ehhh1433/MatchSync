namespace MatchSync.Models
{
    public class Inventory
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? Category { get; set; } // Retail, Rental, etc.
        public string? Barcode { get; set; } // Matches scanner input
        public decimal Price { get; set; }
        public int StockQty { get; set; }
        public int RedeemPoints { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}