namespace CalculatorApp.Models
{
    public class SaleItem
    {
        public long SaleItemID { get; set; }
        public long SaleID { get; set; }
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}
