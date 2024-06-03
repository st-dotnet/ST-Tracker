using DBModels.Model;
using System;
using System.Configuration;
using System.Data.SQLite;
using System.Threading.Tasks;
using TimeTracker.DapperUtility;

namespace TimeTracker.Utilities
{
    public class TrackerLocalStorage
    {
        public TrackerLocalStorage()
        {
            CreateDatabaseAndTrackerTable();
        }
        public void CreateDatabaseAndTrackerTable()
        {
            using (var connection = new SQLiteConnection(ConnectionClass.sqliteConnStr()))
            {
                connection.Open();

                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS TrackerDataOffline (
                    TrackerId TEXT PRIMARY KEY,
                    Date TEXT NOT NULL,
                    TotalTime TEXT NOT NULL,
                    IdleTime TEXT NOT NULL,
                    EmployeeId TEXT NOT NULL
                );";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        public async Task<TrackerData> CheckOfflineTrackerDataExists(DateTime date, Guid? empId)
        {
            try
            {
                using (var connection = new SQLiteConnection(ConnectionClass.sqliteConnStr()))
                {
                    connection.Open();

                    string selectQuery = "SELECT * FROM TrackerDataOffline";
                    using (var command = new SQLiteCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Date", date.Date.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@empId", empId.ToString());

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new TrackerData
                                {
                                    TrackerId = Guid.Parse(reader["TrackerId"].ToString()),
                                    Date = DateTime.Parse(reader["Date"].ToString()),
                                    TotalTime = TimeSpan.Parse(reader["TotalTime"].ToString()),
                                    IdleTime = TimeSpan.Parse(reader["IdleTime"].ToString()),
                                    EmployeeId = Guid.Parse(reader["EmployeeId"].ToString())
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        public async Task SaveTrackerDataOffline(TrackerData data)
        {
            try
            {
                using (var connection = new SQLiteConnection(ConnectionClass.sqliteConnStr()))
                {
                    connection.Open();
                    string dynamicQuery = "";
                    var offlineTrackerData = await CheckOfflineTrackerDataExists(data.Date, data.EmployeeId);
                    if (offlineTrackerData != null)
                    {
                        data.TrackerId = offlineTrackerData.TrackerId;
                        data.EmployeeId = offlineTrackerData.EmployeeId;
                        data.TotalTime = offlineTrackerData.TotalTime + data.TotalTime;
                        data.IdleTime = offlineTrackerData.IdleTime;
                        dynamicQuery = @"UPDATE TrackerDataOffline SET TotalTime = @TotalTime, IdleTime = @IdleTime WHERE TrackerId = @TrackerId;";
                    }
                    else
                    {
                        dynamicQuery = @"
                    INSERT OR REPLACE INTO TrackerDataOffline (TrackerId, Date, TotalTime, IdleTime, EmployeeId)
                    VALUES (@TrackerId, @Date, @TotalTime, @IdleTime, @EmployeeId);";

                    }
                    using (var command = new SQLiteCommand(dynamicQuery, connection))
                    {
                        command.Parameters.AddWithValue("@TrackerId", data.TrackerId.ToString());
                        command.Parameters.AddWithValue("@Date", data.Date.Date.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@TotalTime", data.TotalTime.ToString());
                        command.Parameters.AddWithValue("@IdleTime", data.IdleTime.ToString());
                        command.Parameters.AddWithValue("@EmployeeId", data.EmployeeId.ToString());

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //public async Task RemoveIdleTimeFromActualOffline(TimeSpan idleTime, bool isYesWorking, DateTime dateTime, Guid? empId)
        //{
        //    using (var connection = new SQLiteConnection(ConnectionClass.sqliteConnStr()))
        //    {
        //        connection.Open();
        //        var offlineTrackerData = await CheckOfflineTrackerDataExists(dateTime, empId);
        //        if (offlineTrackerData != null)
        //        {
        //            string updateQuery = "";
        //            int timeIntervalMinutes;
        //            if (!int.TryParse(ConfigurationManager.AppSettings["TimeIntervalInMinutes"], out timeIntervalMinutes))
        //            {
        //                timeIntervalMinutes = 10; //Default Idle time set to 10 minutes
        //            }
        //            TimeSpan tenMinutes = new TimeSpan(0, timeIntervalMinutes, 0);//ChangeIdleTime
        //            TrackerData updatedData = new TrackerData
        //            {
        //                TrackerId = offlineTrackerData.TrackerId,
        //                TotalTime = offlineTrackerData.TotalTime > tenMinutes ? offlineTrackerData.TotalTime.Subtract(new TimeSpan(0, timeIntervalMinutes, 0)) : TimeSpan.Zero,//ChangeIdleTime
        //                IdleTime = isYesWorking ? offlineTrackerData.IdleTime.Add(new TimeSpan(0, timeIntervalMinutes, 0)) + idleTime : offlineTrackerData.IdleTime,//ChangeIdleTime
        //            };
        //            updateQuery = @"UPDATE TrackerDataOffline SET TotalTime = @TotalTime, IdleTime = @IdleTime WHERE TrackerId = @TrackerId;";
        //            using (var command = new SQLiteCommand(updateQuery, connection))
        //            {
        //                command.Parameters.AddWithValue("@TrackerId", updatedData.TrackerId.ToString());
        //                command.Parameters.AddWithValue("@TotalTime", updatedData.TotalTime.ToString());
        //                command.Parameters.AddWithValue("@IdleTime", updatedData.IdleTime.ToString());
        //                command.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //}
        public async Task DeletePreviousOfflineRecords()
        {
            using (var connection = new SQLiteConnection(ConnectionClass.sqliteConnStr()))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM TrackerDataOffline;";
                using (var command = new SQLiteCommand(deleteQuery, connection))
                {
                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }
    }
}
