using System;
using System.Configuration;
using System.IO;

namespace TimeTracker.DapperUtility
{
    public static class ConnectionClass
    {
        public static string ConVal()
        {
            return ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }
        public static string sqliteConnStr()
        {
            string dbFilePath = GetDatabasePath();
            string connectionStringTemplate = ConfigurationManager.ConnectionStrings["SQLiteConnectionString"].ConnectionString;
            string connectionString = string.Format(connectionStringTemplate, dbFilePath);
            return connectionString;
        }
        public static string GetDatabasePath()
        {
            string localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(localAppDataFolder, "STTracker");

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }
            string dbFilePath = Path.Combine(appFolder, "userInformation.db");
            return dbFilePath;
        }
    }
}
