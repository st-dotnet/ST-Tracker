using System;
using System.IO;
using System.Windows.Forms;
using TimeTracker.Utilities;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace TimeTracker.Form
{
    public partial class Application : System.Windows.Forms.Form
    {
        private TrackingService TrackingService;
        private DBAccessContext _dbAccessContext;
        public TrackerLocalStorage _trackerStorage;
        public InternetManager InternetManager;
        private Timer RefreshTimer;
        private const int SW_RESTORE = 9;
        private DateTimeOffset IdleTimeDetection;

        //To show application on Front
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        public Application()
        {
            _trackerStorage = new TrackerLocalStorage();
            InternetManager = new InternetManager(this);
            _dbAccessContext = new DBAccessContext(_trackerStorage, InternetManager);
            TrackingService = new TrackingService(this, _dbAccessContext, InternetManager);

            AssignEmployeeValues();
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            RefreshTimer = new Timer
            {
                Interval = 100
            };
            RefreshTimer.Tick += new System.EventHandler(RefreshTrackingInfo);

            InitializeComponent();
            TrackingService.InitializeHooks();
            InternetManager.InternetCheckInitialize();
            this.toolStripMain.Items.RemoveByKey("addToolStripButton");
            this.toolStripMain.Items.RemoveByKey("copyToolStripButton");
            this.Refresh();
            RefreshTitle();
            RefreshTrackingButtons();
            SetTotalTime();
            this.TopMost = true;
        }

        private void RefreshTitle()
        {
            var text = ProductName;
            this.Text = text;
            notifyIcon.Text = text;
        }

        #region Set Employee Data
        private async void AssignEmployeeValues()
        {
            var employeeData = await _dbAccessContext.GetEmployeeDetail();
            if (employeeData.ProfilePicture != null && employeeData.ProfilePicture.Length > 0)
            {
                System.Drawing.Image profileImage;
                using (MemoryStream ms = new MemoryStream(employeeData.ProfilePicture))
                {
                    profileImage = System.Drawing.Image.FromStream(ms);
                }
                profilePictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                profilePictureBox.Image = profileImage;
            }
            this.EmployeeName.Text = employeeData.FirstName + " " + employeeData.LastName;
        }
        #endregion

        public async void SetTotalTime()
        {
            var totalTime = await _dbAccessContext.GetTotalTime();
            this.statsTotalText.Text = String.Format(Properties.Resources.Application_statsTotal_Text, totalTime.Format());
        }

        #region Start / Pause / PunchOut
        private async void startTrackingToolStripButton_Click(object sender, EventArgs e)
        {
            await Start_Tracking();
        }
        public async Task Start_Tracking()
        {
            TrackingService.Start();
            RefreshTimer.Start();
            RefreshTrackingButtons();
            this.trackingStartTimeToolStripTextBox.Text = TrackingService.StartTime.LocalDateTime.ToString("h\\:mm\\:ss");
            this.trackingElapsedTimeToolStripTextBox.Text = TrackingService.Elapsed;
        }

        private async void stopTrackingToolStripButton_Click(object sender, EventArgs e)
        {
            await Pause_Tracking();
        }
        public async Task Pause_Tracking()
        {
            await TrackingService.Stop();
            SetTotalTime();
            RefreshTimer.Stop();
            RefreshTrackingButtons();
        }

        private async void toolStripButton1_Click(object sender, EventArgs e)
        {
            await stopTracking();
        }
        private async Task stopTracking()
        {
            await TrackingService.Stop();
            SetTotalTime();
            RefreshTimer.Stop();
            RefreshTrackingButtons();
            this.trackingStartTimeToolStripTextBox.Text = "--:--:--";
            this.trackingElapsedTimeToolStripTextBox.Text = TrackingService.ZeroTime;
        }
        #endregion

        private void RefreshTrackingButtons()
        {
            var tracking = TrackingService.Tracking;
            this.startTrackingToolStripButton.Enabled = !tracking;
            this.stopTrackingToolStripButton.Enabled = tracking;
        }

        private void RefreshTrackingInfo(object sender, EventArgs e)
        {
            this.trackingElapsedTimeToolStripTextBox.Text = TrackingService.Elapsed;
        }

        private async void Application_FormClosing(object sender, FormClosingEventArgs e)
        {
            var tracking = TrackingService.Tracking;
            if (tracking)
            {
                await stopTracking();
            }
            else
            {
                if (this.idlePanel.Visible == true)
                {
                    await UpdateIdleTime(false);
                }
            }
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                // restore
                this.WindowState = FormWindowState.Normal;
                // this.ShowInTaskbar = this.showInTaskbarToolStripMenuItem.Checked;
            }
            else
            {
                // hide
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }
        }

        private void SetApplicationToFront()
        {
            Process[] processes = Process.GetProcessesByName("timetracker");

            if (processes.Length > 0)
            {
                Process targetProcess = processes[0];
                IntPtr mainWindowHandle = targetProcess.MainWindowHandle;

                if (mainWindowHandle != IntPtr.Zero)
                {
                    if (IsIconic(mainWindowHandle))
                    {
                        ShowWindow(mainWindowHandle, SW_RESTORE);
                    }

                    SetForegroundWindow(mainWindowHandle);
                }
            }
        }
        public void InternetStatus(bool isAvailable)
        {
            if (isAvailable)
            {
                this.internetStatus.Text = "Connected";
                this.internetStatus.BackColor = Color.FromArgb(31, 181, 173);
            }
            else
            {
                this.internetStatus.Text = "No Internet, still tracking!";
                this.internetStatus.BackColor = Color.FromArgb(234, 31, 75);
            }
        }
        public void ShowIdle()
        {
            this.IdleTimeDetection = DateTimeOffset.Now;
            this.toolStripMain.Enabled = false;
            SetApplicationToFront();
            this.idlePanel.Visible = true;
        }
        private async void workingBtn_Click(object sender, EventArgs e)
        {
            await UpdateIdleTime(true);
            await Start_Tracking();
        }

        private async void notWorking_Click(object sender, EventArgs e)
        {
            await UpdateIdleTime(false);
        }
        private async Task UpdateIdleTime(bool isYesWorking)
        {
            TimeSpan elapsedIdlePopupTime = DateTimeOffset.Now - this.IdleTimeDetection;
            await _dbAccessContext.RemoveIdleTimeFromActual(elapsedIdlePopupTime, isYesWorking);
            this.idlePanel.Visible = false;
            this.toolStripMain.Enabled = true;
            SetTotalTime();
        }
        public async Task SyncLocalDataToServer()
        {
            await _dbAccessContext.SyncLocalDataToServer();
        }
    }
}