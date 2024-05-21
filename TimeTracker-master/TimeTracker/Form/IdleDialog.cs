using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeTracker.Utilities;

namespace TimeTracker.Form
{
    public partial class IdleDialog : System.Windows.Forms.Form
    {
        private DateTimeOffset IdleTimeDetection;
        private readonly TimeTracker.Form.Application _application;
        private InternetManager InternetManager;

        public IdleDialog(TimeTracker.Form.Application application)
        {
            _application = application;
            InternetManager = new InternetManager(_application);
            InitializeComponent();
            this.IdleTimeDetection = DateTimeOffset.Now;
            this.TopMost = true;
            this.ControlBox = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void notWorking_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void workingBtn_Click(object sender, EventArgs e)
        {
            DBAccessContext dBAccessContext = new DBAccessContext();
            TimeSpan elapsedTime = DateTimeOffset.Now - IdleTimeDetection;
            var internetAvailable = await InternetManager.CheckInternetConnected();
            if (internetAvailable)
            {
                await dBAccessContext.AddUpdateTrackerInfo(default, elapsedTime);
            }
            else
            {
                await dBAccessContext.StoreTrackerDataToLocal(default, elapsedTime);
            }
            await _application.Start_Tracking();
        }
    }
}
