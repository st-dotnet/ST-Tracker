using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeTracker.Utilities
{
    public class InternetManager
    {
        private Timer internetCheckTimer;
        private readonly TimeTracker.Form.Application _application;
        private bool isSaveToDB = false;

        public InternetManager(TimeTracker.Form.Application application)
        {
            _application = application;
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
        private async static Task<bool> IsInternetAvailable()
        {
            using (var ping = new Ping())
            {
                try
                {
                    var reply = await ping.SendPingAsync("8.8.8.8", 2000);
                    return reply.Status == IPStatus.Success;
                }
                catch (PingException)
                {
                    // Handle ping exceptions if needed
                    return false;
                }
            }
        }
        public async Task<bool> CheckInternetConnected()
        {
            var internetAvailable = false;
            if (await IsInternetAvailable())
            {
                if (isSaveToDB == true)
                {
                    isSaveToDB = false;
                    await Task.Run(() => _application.SyncLocalDataToServer());
                }
                internetAvailable = true;
            }
            else
            {
                isSaveToDB = true;
            }
            _application.InternetStatus(internetAvailable);
            return internetAvailable;
        }
    }
}
