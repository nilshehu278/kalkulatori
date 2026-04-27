using System.Data.SQLite;
using System.IO;

namespace CalculatorApp.Data
{
    public static class DbConnectionFactory
    {
        public const string DatabaseFileName = "apedin_market.db";

        public static string DatabasePath
        {
            get
            {
                var baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                return Path.Combine(baseDirectory, DatabaseFileName);
            }
        }

        public static SQLiteConnection CreateConnection()
        {
            var connection = new SQLiteConnection($"Data Source={DatabasePath};Version=3;");
            connection.Open();
            using (var pragma = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
            {
                pragma.ExecuteNonQuery();
            }

            return connection;
        }
    }
}
