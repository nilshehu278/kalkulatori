using System;

namespace CalculatorApp.Models
{
    public class Sale
    {
        public long SaleID { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal ChangeAmount { get; set; }
    }
}
