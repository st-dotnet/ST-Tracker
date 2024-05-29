using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeTracker.Utilities
{
    public class InternetManager
    {
        private Timer internetCheckTimer;
        private readonly TimeTracker.Form.Application _application;
        private readonly UserInformationManager offlineTrackerDataManager;
        public InternetManager(TimeTracker.Form.Application application)
        {
            _application = application;
            offlineTrackerDataManager = new UserInformationManager("tracker_info.xml");
        }

        public void InternetCheckInitialize()
        {
            internetCheckTimer = new Timer();
            internetCheckTimer.Interval = 5000; // Check internet connection every 5 seconds
            internetCheckTimer.Tick += Internet_Check_Timer_Tick;
            internetCheckTimer.Start();
        }

        private void Internet_Check_Timer_Tick(object sender, EventArgs e)
        {
            CheckInternetConnected();
        }
        private static bool IsInternetAvailable()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var result = ping.Send("8.8.8.8", 2000);
                    return result.Status == IPStatus.Success;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> CheckInternetConnected()
        {
            var internetAvailable = false;
            if (IsInternetAvailable())
            {
                await SaveLocalDataToDBIfExists();
                internetAvailable = true;
            }
            _application.InternetStatus(internetAvailable);
            return internetAvailable;
        }
        public async Task SaveLocalDataToDBIfExists()
        {
            var offlineDataToStore = offlineTrackerDataManager.RetrieveTrackerDataIfExists();
            if (offlineDataToStore != null)
            {
                DBAccessContext dBAccessContext = new DBAccessContext();
                await dBAccessContext.TryStoreOfflineDataToDb(offlineDataToStore);
            }
        }
    }
}
