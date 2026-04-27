using System;
using System.Collections.Generic;
using System.Data.SQLite;
using CalculatorApp.Models;

namespace CalculatorApp.Data
{
    public class SalesRepository
    {
        public long CompleteSale(List<SaleItem> items, decimal total, decimal paid, decimal change)
        {
            if (items == null || items.Count == 0) throw new InvalidOperationException("Cart is empty.");
            if (total < 0 || paid < 0 || change < 0) throw new InvalidOperationException("Amounts cannot be negative.");

            using (var connection = DbConnectionFactory.CreateConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var saleId = InsertSale(connection, transaction, total, paid, change);

                    foreach (var item in items)
                    {
                        if (item.Quantity <= 0) throw new InvalidOperationException("Quantity must be greater than zero.");
                        InsertSaleItem(connection, transaction, saleId, item);
                        UpdateStock(connection, transaction, item.ProductID, item.Quantity);
                    }

                    transaction.Commit();
                    return saleId;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public List<Sale> GetSalesHistory()
        {
            var result = new List<Sale>();
            using (var connection = DbConnectionFactory.CreateConnection())
            using (var cmd = new SQLiteCommand(@"SELECT SaleID, SaleDate, TotalAmount, PaidAmount, ChangeAmount
                                                FROM Sales ORDER BY SaleID DESC;", connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new Sale
                    {
                        SaleID = Convert.ToInt64(reader["SaleID"]),
                        SaleDate = DateTime.Parse(Convert.ToString(reader["SaleDate"])),
                        TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                        PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                        ChangeAmount = Convert.ToDecimal(reader["ChangeAmount"])
                    });
                }
            }

            return result;
        }

        public List<SaleItem> GetSaleItems(long saleId)
        {
            var result = new List<SaleItem>();
            using (var connection = DbConnectionFactory.CreateConnection())
            using (var cmd = new SQLiteCommand(@"SELECT si.SaleItemID, si.SaleID, si.ProductID, p.ProductName, si.Quantity, si.UnitPrice, si.Subtotal
                                                FROM SaleItems si
                                                INNER JOIN Products p ON p.ProductID = si.ProductID
                                                WHERE si.SaleID = @saleId;", connection))
            {
                cmd.Parameters.AddWithValue("@saleId", saleId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new SaleItem
                        {
                            SaleItemID = Convert.ToInt64(reader["SaleItemID"]),
                            SaleID = Convert.ToInt64(reader["SaleID"]),
                            ProductID = Convert.ToInt64(reader["ProductID"]),
                            ProductName = Convert.ToString(reader["ProductName"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                            Subtotal = Convert.ToDecimal(reader["Subtotal"])
                        });
                    }
                }
            }

            return result;
        }

        private static long InsertSale(SQLiteConnection connection, SQLiteTransaction transaction, decimal total, decimal paid, decimal change)
        {
            using (var cmd = new SQLiteCommand(@"INSERT INTO Sales (SaleDate, TotalAmount, PaidAmount, ChangeAmount)
                                                VALUES (@date, @total, @paid, @change);
                                                SELECT last_insert_rowid();", connection, transaction))
            {
                cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@total", total);
                cmd.Parameters.AddWithValue("@paid", paid);
                cmd.Parameters.AddWithValue("@change", change);
                return (long)cmd.ExecuteScalar();
            }
        }

        private static void InsertSaleItem(SQLiteConnection connection, SQLiteTransaction transaction, long saleId, SaleItem item)
        {
            using (var cmd = new SQLiteCommand(@"INSERT INTO SaleItems (SaleID, ProductID, Quantity, UnitPrice, Subtotal)
                                                VALUES (@saleId, @productId, @quantity, @unitPrice, @subtotal);", connection, transaction))
            {
                cmd.Parameters.AddWithValue("@saleId", saleId);
                cmd.Parameters.AddWithValue("@productId", item.ProductID);
                cmd.Parameters.AddWithValue("@quantity", item.Quantity);
                cmd.Parameters.AddWithValue("@unitPrice", item.UnitPrice);
                cmd.Parameters.AddWithValue("@subtotal", item.Subtotal);
                cmd.ExecuteNonQuery();
            }
        }

        private static void UpdateStock(SQLiteConnection connection, SQLiteTransaction transaction, long productId, int quantityToDeduct)
        {
            using (var cmd = new SQLiteCommand(@"UPDATE Products
                                                SET StockQuantity = StockQuantity - @quantity
                                                WHERE ProductID = @productId AND StockQuantity >= @quantity;", connection, transaction))
            {
                cmd.Parameters.AddWithValue("@quantity", quantityToDeduct);
                cmd.Parameters.AddWithValue("@productId", productId);
                var rows = cmd.ExecuteNonQuery();
                if (rows == 0) throw new InvalidOperationException("Insufficient stock or product missing.");
            }
        }
    }
}
