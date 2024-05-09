using System;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using DBModels.Model;
using TimeTracker.DapperUtility;

namespace TimeTracker.Utilities
{
    public class DBAccessContext
    {
        private readonly UserInformationManager userManager;
        private readonly UserInformationManager offlineTrackerDataManager;

        public DBAccessContext()
        {
            userManager = new UserInformationManager("user_info.xml");
            offlineTrackerDataManager = new UserInformationManager("tracker_info.xml");
        }

        public async Task AddUpdateTrackerInfo(TimeSpan statTotal)
        {
            try
            {
                var offlineDataToStore = offlineTrackerDataManager.RetrieveTrackerDataIfExists();
                if (offlineDataToStore != null)
                {
                    await TryStoreOfflineDataToDb(offlineDataToStore);
                }
                await StoreTrackerDataToDB(statTotal);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private async Task<TrackerData> CheckTrackingExists(DateTime date, Guid? empId)
        {
            using (IDbConnection db = new SqlConnection(ConnectionClass.ConVal()))
            {
                return await db.QuerySingleOrDefaultAsync<TrackerData>("SELECT * FROM [dbo].[Tracker] WHERE [Date] = @date AND EmployeeId = @empId", new { date = date, empId = empId });
            }
        }

        private async Task<Guid?> GetEmployeeId(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                username = userManager.RetrieveUserInformation().Username;
            }
            using (IDbConnection db = new SqlConnection(ConnectionClass.ConVal()))
            {
                return await db.QuerySingleOrDefaultAsync<Guid?>("SELECT EmployeeId FROM Employees WHERE Email = @Username", new { Username = username });
            }
        }

        public async Task SaveScreenshot(byte[] screenshotBytes, int? keyStrokes, bool isCameraCapture)
        {
            var dateTime = GetPreviousDate();
            var empId = await GetEmployeeId("");
            var trackerData = await CheckTrackingExists(dateTime.Date, empId);
            TrackerScreenshotData newData = new TrackerScreenshotData
            {
                TrackerId = trackerData.TrackerId,
                Screenshots = screenshotBytes,
                Keystrokes = keyStrokes,
                CreatedDateTime = DateTime.Now,
                IsCameraCapture = isCameraCapture,
            };
            using (IDbConnection db = new SqlConnection(ConnectionClass.ConVal()))
            {
                string query = @"INSERT INTO TrackerScreenshots (TrackerId, Screenshots, Keystrokes, createdDateTime, IsCameraCapture)
                                    VALUES (@TrackerId, @Screenshots, @Keystrokes, @CreatedDateTime, @IsCameraCapture)";
                db.Execute(query, newData);
            }
        }

        public async Task TryStoreOfflineDataToDb(TrackerDataOffline offlineTrackerData)
        {
            var offlineEmpId = await GetEmployeeId(offlineTrackerData.EmployeeUsername);
            TrackerData offlineDataToStore = new TrackerData
            {
                TrackerId = offlineTrackerData.TrackerId,
                Date = offlineTrackerData.Date,
                TotalTime = offlineTrackerData.TotalTime,
                EmployeeId = offlineEmpId,
            };
            using (IDbConnection db = new SqlConnection(ConnectionClass.ConVal()))
            {
                string addUpdateQuery = "";
                var trackerData = await CheckTrackingExists(offlineTrackerData.Date, offlineDataToStore.EmployeeId);
                if (trackerData != null)
                {
                    offlineDataToStore.TrackerId = trackerData.TrackerId;
                    offlineDataToStore.TotalTime = trackerData.TotalTime + offlineDataToStore.TotalTime;
                    addUpdateQuery = @"UPDATE Tracker SET TotalTime = @TotalTime WHERE TrackerId = @TrackerId";
                }
                else
                {
                    addUpdateQuery = @"INSERT INTO Tracker (TrackerId, Date, TotalTime, EmployeeId)
                                    VALUES (@TrackerId, @Date, @TotalTime, @EmployeeId)";
                }
                db.Execute(addUpdateQuery, offlineDataToStore);
                offlineTrackerDataManager.RemoveOfflineTrackerData();
            }

        }

        public async Task StoreTrackerDataToDB(TimeSpan statTotal)
        {
            UserInformation userInfo = userManager.RetrieveUserInformation();
            var dateTime = GetPreviousDate();
            try
            {
                var empId = await GetEmployeeId(userInfo.Username);
                TrackerData newData = new TrackerData
                {
                    TrackerId = Guid.NewGuid(),
                    Date = dateTime.Date,
                    TotalTime = statTotal,
                    EmployeeId = empId,
                };

                using (IDbConnection db = new SqlConnection(ConnectionClass.ConVal()))
                {
                    string addUpdateQuery = "";
                    var trackerData = await CheckTrackingExists(dateTime.Date, empId); // Check if Tracking exists for the same day
                    if (trackerData != null)
                    {
                        newData.TrackerId = trackerData.TrackerId;
                        newData.EmployeeId = trackerData.EmployeeId;
                        newData.TotalTime = trackerData.TotalTime + statTotal;
                        addUpdateQuery = @"UPDATE Tracker SET TotalTime = @TotalTime WHERE TrackerId = @TrackerId";
                    }
                    else
                    {
                        addUpdateQuery = @"INSERT INTO Tracker (TrackerId, Date, TotalTime, EmployeeId)
                                    VALUES (@TrackerId, @Date, @TotalTime, @EmployeeId)";
                    }
                    db.Execute(addUpdateQuery, newData);
                }
            }
            catch (SqlException ex)
            {
                TrackerDataOffline newDataOffline = new TrackerDataOffline
                {
                    TrackerId = Guid.NewGuid(),
                    Date = dateTime.Date,
                    TotalTime = statTotal,
                    EmployeeUsername = userInfo.Username,
                };
                offlineTrackerDataManager.SaveTrackerDataOffline(newDataOffline);
            }
        }
        private DateTime GetPreviousDate()
        {
            var dateTime = DateTime.Now;
            if (dateTime.TimeOfDay >= TimeSpan.Zero && dateTime.TimeOfDay < TimeSpan.FromHours(3))
            {
                dateTime = dateTime.AddDays(-1);
            }
            return dateTime;
        }

        public async Task<EmployeeData> GetEmployeeDetail()
        {
            var username = userManager.RetrieveUserInformation().Username;
            using (IDbConnection db = new SqlConnection(ConnectionClass.ConVal()))
            {
                return await db.QuerySingleOrDefaultAsync<EmployeeData>("SELECT EmployeeId, FirstName, LastName, ProfilePicture FROM Employees WHERE Email = @Username", new { Username = username });
            }
        }
    }
}