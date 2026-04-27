namespace CalculatorApp.Models
{
    public class Product
    {
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int MinimumStockLevel { get; set; }
    }
}
