using System;
using System.Collections.Generic;
using System.Data.SQLite;
using CalculatorApp.Models;

namespace CalculatorApp.Data
{
    public class ProductRepository
    {
        public List<Product> GetAll(string search = null)
        {
            var result = new List<Product>();
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var sql = @"SELECT ProductID, ProductName, Category, Barcode, Price, StockQuantity, MinimumStockLevel
                            FROM Products
                            WHERE @search IS NULL OR ProductName LIKE @term OR Barcode LIKE @term
                            ORDER BY ProductName;";

                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@search", string.IsNullOrWhiteSpace(search) ? (object)DBNull.Value : search);
                    cmd.Parameters.AddWithValue("@term", "%" + (search ?? string.Empty) + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(Map(reader));
                        }
                    }
                }
            }

            return result;
        }

        public Product GetByBarcode(string barcode)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            using (var cmd = new SQLiteCommand(@"SELECT ProductID, ProductName, Category, Barcode, Price, StockQuantity, MinimumStockLevel FROM Products WHERE Barcode = @barcode;", connection))
            {
                cmd.Parameters.AddWithValue("@barcode", barcode);
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() ? Map(reader) : null;
                }
            }
        }

        public void Add(Product product)
        {
            ValidateProduct(product);
            using (var connection = DbConnectionFactory.CreateConnection())
            using (var cmd = new SQLiteCommand(@"INSERT INTO Products (ProductName, Category, Barcode, Price, StockQuantity, MinimumStockLevel)
                                                VALUES (@name, @category, @barcode, @price, @stock, @minStock);", connection))
            {
                cmd.Parameters.AddWithValue("@name", product.ProductName.Trim());
                cmd.Parameters.AddWithValue("@category", product.Category ?? string.Empty);
                cmd.Parameters.AddWithValue("@barcode", string.IsNullOrWhiteSpace(product.Barcode) ? (object)DBNull.Value : product.Barcode.Trim());
                cmd.Parameters.AddWithValue("@price", product.Price);
                cmd.Parameters.AddWithValue("@stock", product.StockQuantity);
                cmd.Parameters.AddWithValue("@minStock", product.MinimumStockLevel);
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(Product product)
        {
            ValidateProduct(product);
            using (var connection = DbConnectionFactory.CreateConnection())
            using (var cmd = new SQLiteCommand(@"UPDATE Products
                                                SET ProductName = @name,
                                                    Category = @category,
                                                    Barcode = @barcode,
                                                    Price = @price,
                                                    StockQuantity = @stock,
                                                    MinimumStockLevel = @minStock
                                                WHERE ProductID = @id;", connection))
            {
                cmd.Parameters.AddWithValue("@id", product.ProductID);
                cmd.Parameters.AddWithValue("@name", product.ProductName.Trim());
                cmd.Parameters.AddWithValue("@category", product.Category ?? string.Empty);
                cmd.Parameters.AddWithValue("@barcode", string.IsNullOrWhiteSpace(product.Barcode) ? (object)DBNull.Value : product.Barcode.Trim());
                cmd.Parameters.AddWithValue("@price", product.Price);
                cmd.Parameters.AddWithValue("@stock", product.StockQuantity);
                cmd.Parameters.AddWithValue("@minStock", product.MinimumStockLevel);
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(long productId)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            using (var cmd = new SQLiteCommand("DELETE FROM Products WHERE ProductID = @id;", connection))
            {
                cmd.Parameters.AddWithValue("@id", productId);
                cmd.ExecuteNonQuery();
            }
        }

        private static Product Map(SQLiteDataReader reader)
        {
            return new Product
            {
                ProductID = Convert.ToInt64(reader["ProductID"]),
                ProductName = Convert.ToString(reader["ProductName"]),
                Category = Convert.ToString(reader["Category"]),
                Barcode = Convert.ToString(reader["Barcode"]),
                Price = Convert.ToDecimal(reader["Price"]),
                StockQuantity = Convert.ToInt32(reader["StockQuantity"]),
                MinimumStockLevel = Convert.ToInt32(reader["MinimumStockLevel"])
            };
        }

        private static void ValidateProduct(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (string.IsNullOrWhiteSpace(product.ProductName)) throw new InvalidOperationException("Product name is required.");
            if (product.Price < 0) throw new InvalidOperationException("Price cannot be negative.");
            if (product.StockQuantity < 0) throw new InvalidOperationException("Stock cannot be negative.");
            if (product.MinimumStockLevel < 0) throw new InvalidOperationException("Minimum stock cannot be negative.");
        }
    }
}
