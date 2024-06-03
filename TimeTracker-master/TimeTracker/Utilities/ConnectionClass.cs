using System.Configuration;

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
            return ConfigurationManager.ConnectionStrings["SQLiteConnectionString"].ConnectionString;
        }
    }
}
