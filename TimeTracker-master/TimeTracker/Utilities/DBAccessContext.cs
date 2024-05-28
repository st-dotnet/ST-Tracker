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
        private readonly UserInformationManager userManager;
        private readonly UserInformationManager offlineTrackerDataManager;

        public DBAccessContext()
        {
            userManager = new UserInformationManager("user_info.xml");
            offlineTrackerDataManager = new UserInformationManager("tracker_info.xml");
        }
        #region Save Tracker Data to Database
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

        public async Task TryStoreOfflineDataToDb(TrackerDataOffline offlineTrackerData)
        {
            var offlineEmpId = userManager.RetrieveUserInformation().EmployeeId;
            TrackerData offlineDataToStore = new TrackerData
            {
                TrackerId = offlineTrackerData.TrackerId,
                Date = offlineTrackerData.Date,
                TotalTime = offlineTrackerData.TotalTime,
                IdleTime = offlineTrackerData.IdleTime,
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
                    offlineDataToStore.IdleTime = trackerData.IdleTime + offlineDataToStore.IdleTime;
                    addUpdateQuery = @"UPDATE Tracker SET TotalTime = @TotalTime, IdleTime = @IdleTime  WHERE TrackerId = @TrackerId";
                }
                else
                {
                    addUpdateQuery = @"INSERT INTO Tracker (TrackerId, Date, TotalTime, EmployeeId, IdleTime)
                                    VALUES (@TrackerId, @Date, @TotalTime, @EmployeeId, @IdleTime)";
                }
                db.Execute(addUpdateQuery, offlineDataToStore);
                offlineTrackerDataManager.RemoveOfflineTrackerData();
            }

        }
        private async Task StoreTrackerDataToDB(TimeSpan statTotal)
        {
            try
            {
                UserInformation userInfo = userManager.RetrieveUserInformation();
                var dateTime = GetPreviousDate();
                var empId = userManager.RetrieveUserInformation().EmployeeId;
                TrackerData newData = new TrackerData
                {
                    TrackerId = Guid.NewGuid(),
                    Date = dateTime.Date,
                    TotalTime = statTotal,
                    IdleTime = TimeSpan.Zero,
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
                        newData.IdleTime = trackerData.IdleTime;
                        addUpdateQuery = @"UPDATE Tracker SET TotalTime = @TotalTime WHERE TrackerId = @TrackerId";
                    }
                    else
                    {
                        addUpdateQuery = @"INSERT INTO Tracker (TrackerId, Date, TotalTime, EmployeeId , IdleTime)
                                    VALUES (@TrackerId, @Date, @TotalTime, @EmployeeId, @IdleTime)";
                    }
                    db.Execute(addUpdateQuery, newData);
                }
            }
            catch (SqlException ex)
            {
                return;
            }
        }

        public async Task RemoveIdleTimeFromActual(TimeSpan idleTime, bool isYesWorking)
        {
            try
            {
                var offlineDataToStore = offlineTrackerDataManager.RetrieveTrackerDataIfExists();
                if (offlineDataToStore != null)
                {
                    await TryStoreOfflineDataToDb(offlineDataToStore);
                }
                UserInformation userInfo = userManager.RetrieveUserInformation();
                var dateTime = GetPreviousDate();
                var empId = userManager.RetrieveUserInformation().EmployeeId;
                using (IDbConnection db = new SqlConnection(ConnectionClass.ConVal()))
                {
                    var trackerData = await CheckTrackingExists(dateTime.Date, empId); // Check if Tracking exists for the same day
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
                var empId = userManager.RetrieveUserInformation().EmployeeId;
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

        #region Store Tracker Data Locally
        public async Task StoreTrackerDataToLocal(TimeSpan statTotal)
        {
            UserInformation userInfo = userManager.RetrieveUserInformation();
            var dateTime = GetPreviousDate();
            TrackerDataOffline newDataOffline = new TrackerDataOffline
            {
                TrackerId = Guid.NewGuid(),
                Date = dateTime.Date,
                TotalTime = statTotal,
                IdleTime = TimeSpan.Zero,
                EmployeeUsername = userInfo.Username,
            };
            offlineTrackerDataManager.SaveTrackerDataOffline(newDataOffline);
        }

        public async Task UpdateIdleData(TimeSpan idleTime, bool isYesWorking)
        {
            offlineTrackerDataManager.UpdateIldeTimeOffline(idleTime, isYesWorking);
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
                    username = userManager.RetrieveUserInformation().Username;
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
            var username = userManager.RetrieveUserInformation().Username;
            using (IDbConnection db = new SqlConnection(ConnectionClass.ConVal()))
            {
                return await db.QuerySingleOrDefaultAsync<EmployeeData>("SELECT EmployeeId, FirstName, LastName, ProfilePicture FROM Employees WHERE Email = @Username and IsActive = 1", new { Username = username });
            }
        }
        public async Task<TimeSpan> GetTotalTime(bool isInternet, TimeSpan oldtotalTimeOnline)
        {
            var dateTime = GetPreviousDate();
            var empId = userManager.RetrieveUserInformation().EmployeeId;
            if (isInternet)
            {
                var trackerData = await CheckTrackingExists(dateTime.Date, empId);
                if (trackerData != null)
                {
                    return trackerData.TotalTime + trackerData.IdleTime;
                }
            }
            else
            {
                var offlineDataToStore = offlineTrackerDataManager.RetrieveTrackerDataIfExists();
                if (offlineDataToStore != null)
                {
                    return oldtotalTimeOnline + offlineDataToStore.TotalTime + offlineDataToStore.IdleTime;
                }
                else
                {
                    return oldtotalTimeOnline;
                }
            }

            return TimeSpan.Zero;
        }
    }
}