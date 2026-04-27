using System;
using System.Data.SQLite;
using System.IO;

namespace CalculatorApp.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            var isFirstRun = !File.Exists(DbConnectionFactory.DatabasePath);
            if (isFirstRun)
            {
                SQLiteConnection.CreateFile(DbConnectionFactory.DatabasePath);
            }

            using (var connection = DbConnectionFactory.CreateConnection())
            {
                CreateTables(connection);
                if (isFirstRun)
                {
                    SeedSampleProducts(connection);
                }
            }
        }

        private static void CreateTables(SQLiteConnection connection)
        {
            var createProducts = @"
CREATE TABLE IF NOT EXISTS Products (
    ProductID INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductName TEXT NOT NULL,
    Category TEXT,
    Barcode TEXT UNIQUE,
    Price DECIMAL NOT NULL CHECK (Price >= 0),
    StockQuantity INTEGER NOT NULL CHECK (StockQuantity >= 0),
    MinimumStockLevel INTEGER NOT NULL CHECK (MinimumStockLevel >= 0)
);";

            var createSales = @"
CREATE TABLE IF NOT EXISTS Sales (
    SaleID INTEGER PRIMARY KEY AUTOINCREMENT,
    SaleDate TEXT NOT NULL,
    TotalAmount DECIMAL NOT NULL CHECK (TotalAmount >= 0),
    PaidAmount DECIMAL NOT NULL CHECK (PaidAmount >= 0),
    ChangeAmount DECIMAL NOT NULL CHECK (ChangeAmount >= 0)
);";

            var createSaleItems = @"
CREATE TABLE IF NOT EXISTS SaleItems (
    SaleItemID INTEGER PRIMARY KEY AUTOINCREMENT,
    SaleID INTEGER NOT NULL,
    ProductID INTEGER NOT NULL,
    Quantity INTEGER NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL NOT NULL CHECK (UnitPrice >= 0),
    Subtotal DECIMAL NOT NULL CHECK (Subtotal >= 0),
    FOREIGN KEY (SaleID) REFERENCES Sales(SaleID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);";

            using (var cmd = new SQLiteCommand(createProducts, connection)) cmd.ExecuteNonQuery();
            using (var cmd = new SQLiteCommand(createSales, connection)) cmd.ExecuteNonQuery();
            using (var cmd = new SQLiteCommand(createSaleItems, connection)) cmd.ExecuteNonQuery();
        }

        private static void SeedSampleProducts(SQLiteConnection connection)
        {
            var sql = @"INSERT INTO Products (ProductName, Category, Barcode, Price, StockQuantity, MinimumStockLevel)
                        VALUES (@name, @category, @barcode, @price, @stock, @minStock);";

            InsertProduct(connection, sql, "Coca Cola", "Drinks", "100000001", 1.50m, 20, 5);
            InsertProduct(connection, sql, "Water", "Drinks", "100000002", 0.80m, 30, 10);
            InsertProduct(connection, sql, "Chips", "Snacks", "100000003", 1.20m, 25, 7);
            InsertProduct(connection, sql, "Milk", "Dairy", "100000004", 1.10m, 18, 5);
            InsertProduct(connection, sql, "Bread", "Bakery", "100000005", 1.00m, 15, 4);
        }

        private static void InsertProduct(SQLiteConnection connection, string sql, string name, string category, string barcode, decimal price, int stock, int minStock)
        {
            using (var cmd = new SQLiteCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@category", category);
                cmd.Parameters.AddWithValue("@barcode", barcode);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@stock", stock);
                cmd.Parameters.AddWithValue("@minStock", minStock);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
