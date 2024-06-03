using System;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using DBModels.Model;
using TimeTracker.DapperUtility;
using System.Configuration;

namespace TimeTracker.Utilities
{
    public class DBAccessContext
    {
        private readonly UserLocalStorage storage;
        private readonly TrackerLocalStorage _trackerStorage;
        private readonly InternetManager _internetManager;
        private TimeSpan totalTimeToShow;

        public DBAccessContext(TrackerLocalStorage trackerStorage, InternetManager internetManager)
        {
            _trackerStorage = trackerStorage;
            storage = new UserLocalStorage();
            _internetManager = internetManager;
        }
        #region Save Tracker Data to Database
        public async Task AddUpdateTrackerInfo(TimeSpan statTotal)
        {
            try
            {
                var internetAvailavble = await _internetManager.CheckInternetConnected();
                var dateTime = GetPreviousDate();
                var empId = storage.RetrieveUserInformation().EmployeeId;
                TrackerData newTrackerData = new TrackerData
                {
                    TrackerId = Guid.NewGuid(),
                    Date = dateTime.Date,
                    TotalTime = statTotal,
                    IdleTime = TimeSpan.Zero,
                    EmployeeId = empId,
                };
                if (internetAvailavble)
                {
                    await StoreTrackerDataToDB(newTrackerData);
                }
                else
                {
                    await _trackerStorage.SaveTrackerDataOffline(newTrackerData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private async Task StoreTrackerDataToDB(TrackerData newTrackerData)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(ConnectionClass.ConVal()))
                {
                    string addUpdateQuery = "";
                    var trackerData = await CheckTrackingExists(newTrackerData.Date, newTrackerData.EmployeeId); // Check if Tracking exists for the same day
                    if (trackerData != null)
                    {
                        newTrackerData.TrackerId = trackerData.TrackerId;
                        newTrackerData.EmployeeId = trackerData.EmployeeId;
                        newTrackerData.TotalTime = trackerData.TotalTime + newTrackerData.TotalTime;
                        newTrackerData.IdleTime = trackerData.IdleTime;
                        addUpdateQuery = @"UPDATE Tracker SET TotalTime = @TotalTime WHERE TrackerId = @TrackerId";
                    }
                    else
                    {
                        addUpdateQuery = @"INSERT INTO Tracker (TrackerId, Date, TotalTime, EmployeeId , IdleTime)
                                    VALUES (@TrackerId, @Date, @TotalTime, @EmployeeId, @IdleTime)";
                    }
                    db.Execute(addUpdateQuery, newTrackerData);
                }
            }
            catch (SqlException ex)
            {
                return;
            }
        }

        public async Task RemoveIdleTimeFromActual(TimeSpan idleTime, bool isYesWorking)
        {
            var dateTime = GetPreviousDate();
            var empId = storage.RetrieveUserInformation().EmployeeId;
            await RemoveIdleTimeFromActualDB(idleTime, isYesWorking, dateTime, empId);
        }

        public async Task RemoveIdleTimeFromActualDB(TimeSpan idleTime, bool isYesWorking, DateTime dateTime, Guid? empId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(ConnectionClass.ConVal()))
                {
                    var trackerData = await CheckTrackingExists(dateTime.Date, empId);
                    if (trackerData != null)
                    {
                        string updateQuery = "";
                        int timeIntervalMinutes;
                        if (!int.TryParse(ConfigurationManager.AppSettings["TimeIntervalInMinutes"], out timeIntervalMinutes))
                        {
                            timeIntervalMinutes = 10; //Default Idle time set to 10 minutes
                        }
                        TimeSpan tenMinutes = new TimeSpan(0, timeIntervalMinutes, 0);//ChangeIdleTime
                        TrackerData updatedData = new TrackerData
                        {
                            TrackerId = trackerData.TrackerId,
                            TotalTime = trackerData.TotalTime > tenMinutes ? trackerData.TotalTime.Subtract(new TimeSpan(0, timeIntervalMinutes, 0)) : TimeSpan.Zero,//ChangeIdleTime
                            IdleTime = isYesWorking ? trackerData.IdleTime.Add(new TimeSpan(0, timeIntervalMinutes, 0)) + idleTime : trackerData.IdleTime,//ChangeIdleTime
                        };
                        updateQuery = @"UPDATE Tracker SET TotalTime = @TotalTime, IdleTime = @IdleTime  WHERE TrackerId = @TrackerId";
                        db.Execute(updateQuery, updatedData);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        #endregion

        #region Save ScreenShots
        public async Task SaveScreenshot(byte[] screenshotBytes, int? keyStrokes, bool isCameraCapture)
        {
            try
            {
                var dateTime = GetPreviousDate();
                var empId = storage.RetrieveUserInformation().EmployeeId;
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
            catch (SqlException ex)
            {
                return;
            }
        }
        #endregion

        private async Task<TrackerData> CheckTrackingExists(DateTime date, Guid? empId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(ConnectionClass.ConVal()))
                {
                    return await db.QuerySingleOrDefaultAsync<TrackerData>("SELECT * FROM [dbo].[Tracker] WHERE [Date] = @date AND EmployeeId = @empId", new { date = date, empId = empId });
                }
            }
            catch (SqlException ex)
            {
                return null;
            }
        }

        public async Task<Guid?> GetEmployeeId(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    username = storage.RetrieveUserInformation().Username;
                }
                using (IDbConnection db = new SqlConnection(ConnectionClass.ConVal()))
                {
                    return await db.QuerySingleOrDefaultAsync<Guid?>("SELECT EmployeeId FROM Employees WHERE Email = @Username and IsActive = 1", new { Username = username });
                }
            }
            catch (SqlException ex)
            {
                return null;
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
            var username = storage.RetrieveUserInformation().Username;
            using (IDbConnection db = new SqlConnection(ConnectionClass.ConVal()))
            {
                return await db.QuerySingleOrDefaultAsync<EmployeeData>("SELECT EmployeeId, FirstName, LastName, ProfilePicture FROM Employees WHERE Email = @Username and IsActive = 1", new { Username = username });
            }
        }
        public async Task<TimeSpan> GetTotalTime()
        {
            var internetAvailavble = await _internetManager.CheckInternetConnected();

            var dateTime = GetPreviousDate();
            var empId = storage.RetrieveUserInformation().EmployeeId;
            if (internetAvailavble)
            {
                var trackerData = await CheckTrackingExists(dateTime.Date, empId);
                if (trackerData != null)
                {
                    totalTimeToShow = trackerData.TotalTime + trackerData.IdleTime;
                    return totalTimeToShow;
                }
            }
            else
            {
                var localTrackerData = await _trackerStorage.CheckOfflineTrackerDataExists(dateTime, empId);
                if (localTrackerData != null)
                {
                    return localTrackerData.TotalTime + localTrackerData.IdleTime + totalTimeToShow;
                }
                else
                {
                    return totalTimeToShow;
                }
            }
            return TimeSpan.Zero;
        }
        public async Task SyncLocalDataToServer()
        {
            var dateTime = GetPreviousDate();
            var empId = storage.RetrieveUserInformation().EmployeeId;
            var localTrackerData = await _trackerStorage.CheckOfflineTrackerDataExists(dateTime, empId);
            if (localTrackerData != null)
            {
                await StoreTrackerDataToDB(localTrackerData);
            }
            await _trackerStorage.DeletePreviousOfflineRecords();
        }
    }
}