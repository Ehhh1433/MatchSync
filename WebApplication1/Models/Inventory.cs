namespace MatchSync.Models
{
    public class Inventory
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Retail, Rental, or Service
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Barcode { get; set; } // Matches the unique constraint in SQL
    }
}

