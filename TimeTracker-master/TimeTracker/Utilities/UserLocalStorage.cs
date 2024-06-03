using DBModels.Model;
using Microsoft.Identity.Client;
using System;
using System.Data.SQLite;
using System.Runtime.Caching;
using TimeTracker.DapperUtility;

namespace TimeTracker.Utilities
{
    public class UserLocalStorage
    {
        private ObjectCache _cache = MemoryCache.Default;
        private CacheItemPolicy _policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(1) };

        public UserLocalStorage()
        {
            CreateDatabaseAndTable();
        }
        public void CreateDatabaseAndTable()
        {
            using (var connection = new SQLiteConnection(ConnectionClass.sqliteConnStr()))
            {
                connection.Open();

                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS User (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL,
                    EmployeeId TEXT NOT NULL,
                    IdToken TEXT,
                    AccessToken TEXT,
                    ExpiresOn TEXT
                );";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        public void SaveUserInformation(AuthenticationResult result, Guid? empId)
        {
            RemoveUserInformation();

            using (var connection = new SQLiteConnection(ConnectionClass.sqliteConnStr()))
            {
                connection.Open();

                string insertQuery = @"
                INSERT INTO User (Username, EmployeeId, IdToken, AccessToken, ExpiresOn)
                VALUES (@Username, @EmployeeId, @IdToken, @AccessToken, @ExpiresOn);";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", result.Account.Username);
                    command.Parameters.AddWithValue("@EmployeeId", empId.ToString());
                    command.Parameters.AddWithValue("@IdToken", result.IdToken);
                    command.Parameters.AddWithValue("@AccessToken", result.AccessToken);
                    command.Parameters.AddWithValue("@ExpiresOn", result.ExpiresOn.ToString());

                    command.ExecuteNonQuery();
                }
            }
            // Update cache
            var userInfo = new UserInformation
            {
                Username = result.Account.Username,
                EmployeeId = empId.GetValueOrDefault(),
                IdToken = result.IdToken,
                AccessToken = result.AccessToken,
                ExpiresOn = result.ExpiresOn
            };
            _cache.Set("UserInformation", userInfo, _policy);
        }
        public void RemoveUserInformation()
        {
            using (var connection = new SQLiteConnection(ConnectionClass.sqliteConnStr()))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM User;";
                using (var command = new SQLiteCommand(deleteQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            _cache.Remove("UserInformation");
        }
        public UserInformation RetrieveUserInformation()
        {
            // Check cache first
            if (_cache.Contains("UserInformation"))
            {
                return (UserInformation)_cache.Get("UserInformation");
            }

            using (var connection = new SQLiteConnection(ConnectionClass.sqliteConnStr()))
            {
                connection.Open();

                string selectQuery = "SELECT * FROM User LIMIT 1;";
                using (var command = new SQLiteCommand(selectQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var userInfo = new UserInformation
                        {
                            Username = reader["Username"].ToString(),
                            EmployeeId = Guid.Parse(reader["EmployeeId"].ToString()),
                            IdToken = reader["IdToken"].ToString(),
                            AccessToken = reader["AccessToken"].ToString(),
                            ExpiresOn = DateTimeOffset.Parse(reader["ExpiresOn"].ToString())
                        };

                        // Add to cache
                        _cache.Set("UserInformation", userInfo, _policy);
                        return userInfo;
                    }
                }
            }

            return null;
        }
    }
}