using Dapper;
using Microsoft.Data.Sqlite;
namespace TinyUrlApp.Utility
{
    // Data/DbInitializer.cs
   

    public static class DbInitializer
    {
        public static void Initialize(string dbPath)
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            // ✅ Check and create each table if not exists
            CreateLinksTable(connection);
            CreateUsersTable(connection);

            Console.WriteLine("✅ Database initialized successfully.");
        }

        private static void CreateLinksTable(SqliteConnection connection)
        {
            // ✅ Check if table exists
            var exists = connection.ExecuteScalar<int>(@"
            SELECT COUNT(*) FROM sqlite_master 
            WHERE type='table' AND name='Links'");

            if (exists == 0)
            {
                connection.Execute(@"
                CREATE TABLE Links (
                    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
                    ShortCode TEXT    NOT NULL UNIQUE,
                    FullUrl   TEXT    NOT NULL,
                    CreatedAt TEXT    NOT NULL DEFAULT (datetime('now'))
                )");

                Console.WriteLine("✅ Table 'Links' created.");
            }
            else
            {
                Console.WriteLine("✅ Table 'Links' already exists — skipped.");
            }
        }

        private static void CreateUsersTable(SqliteConnection connection)
        {
            var exists = connection.ExecuteScalar<int>(@"
            SELECT COUNT(*) FROM sqlite_master 
            WHERE type='table' AND name='Users'");

            if (exists == 0)
            {
                connection.Execute(@"
                CREATE TABLE Users (
                    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name      TEXT    NOT NULL,
                    Email     TEXT    NOT NULL UNIQUE,
                    CreatedAt TEXT    NOT NULL DEFAULT (datetime('now'))
                )");

                Console.WriteLine("✅ Table 'Users' created.");
            }
            else
            {
                Console.WriteLine("✅ Table 'Users' already exists — skipped.");
            }
        }
    }
}
