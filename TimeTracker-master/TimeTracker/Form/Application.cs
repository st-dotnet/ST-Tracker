using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TimeTracker.Model;
using TimeTracker.Properties;
using TimeTracker.Utilities;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace TimeTracker.Form
{
    public partial class Application : System.Windows.Forms.Form
    {
        const int CATEGORY_MAXLENGTH = 255;
        private BindingList<TimeTrackerData> Data;
        private TrackingService TrackingService;
        private InternetManager InternetManager;
        private Timer RefreshTimer;
        private ToolTip toolTip = new ToolTip();
        private FileInfo file;
        private TimeSpan totalTimeToShow;
        private const int SW_RESTORE = 9;

        //To show application on Front
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        public Application()
        {
            TrackingService = new TrackingService(this);
            InternetManager = new InternetManager(this);

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Data = new BindingList<TimeTrackerData>();
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
            this.dataGridViewMain.DataSource = Data;
            this.categoryToolStripComboBox.MaxLength = CATEGORY_MAXLENGTH;
            this.Refresh();
            Data.ListChanged += new ListChangedEventHandler(DataListChanged);
            RefreshTitle();
            RefreshTrackingButtons();
            RefreshStatistics();
            AssignEmployeeValues();
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
            bool isInternet = await InternetManager.CheckInternetConnected();
            if (isInternet)
            {
                DBAccessContext dBAccessContext = new DBAccessContext();
                var employeeData = await dBAccessContext.GetEmployeeDetail();
                if (employeeData.ProfilePicture != null && employeeData.ProfilePicture.Length > 0)
                {
                    Image profileImage;
                    using (MemoryStream ms = new MemoryStream(employeeData.ProfilePicture))
                    {
                        profileImage = Image.FromStream(ms);
                    }
                    profilePictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    profilePictureBox.Image = profileImage;
                }
                this.EmployeeName.Text = employeeData.FirstName + " " + employeeData.LastName;
            }
        }
        #endregion

        public async void SetTotalTime()
        {
            DBAccessContext dBAccessContext = new DBAccessContext();
            var isInternet = await InternetManager.CheckInternetConnected();
            var totalTime = await dBAccessContext.GetTotalTime(isInternet, totalTimeToShow);
            if (isInternet)
                totalTimeToShow = totalTime;
            this.statsTotalText.Text = String.Format(Properties.Resources.Application_statsTotal_Text, totalTime.Format());
        }
        private void RefreshStatistics()
        {
            for (int i = Data.Count - 1; i >= 0; i--)
            {
                if (Data[i] == null)
                {
                    Data.RemoveAt(i);
                }
            }

            DataGridView grid = this.dataGridViewMain;

            if (grid.SelectedRows.Count > 1)
            {
                var selectionEnumerator = grid.SelectedRows.GetEnumerator();
                TimeSpan statSelection = new TimeSpan();
                while (selectionEnumerator.MoveNext())
                {
                    var row = (DataGridViewRow)selectionEnumerator.Current;
                    var data = (TimeTrackerData)row.DataBoundItem;
                    statSelection = statSelection.Add(data.GetTimeElapsed());
                }

                this.statsSelectedText.Text = String.Format(Properties.Resources.Application_statsSelected_Text, grid.SelectedRows.Count, statSelection.Format());
                this.statsSelectedText.Visible = true;
            }
            else
            {
                this.statsSelectedText.Visible = false;
            }

            if (grid.SelectedRows.Count == 1)
            {
                var selected = (TimeTrackerData)grid.SelectedRows[0].DataBoundItem;
                var category = selected.Category;
                TimeSpan statCategory = Data.Where(value => category == null ? value.Category == null : value.Category != null && value.Category.Equals(category)).Sum(value => value.GetTimeElapsed());

                this.statsCategoryText.Text = String.Format(Properties.Resources.Application_statsCategory_Text, category == null ? "" : category.Name, statCategory.Format());
                this.statsCategoryText.Visible = true;
            }
            else
            {
                this.statsCategoryText.Visible = false;
            }
        }

        private void DataListChanged(object sender, ListChangedEventArgs e)
        {
            RefreshStatistics();
        }

        //private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        //{
        //    System.Windows.Forms.Application.Exit();
        //}

        //private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        //{
        //    AboutBox aboutBox = new AboutBox();
        //    aboutBox.ShowDialog();
        //}

        private void dataGridViewMain_SelectionChanged(object sender, EventArgs e)
        {
            RefreshStatistics();
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
            TimeTrackerData item = await TrackingService.Stop();
            RefreshTimer.Stop();

            // fill in category
            if (categoryToolStripComboBox.Text.Length > 0)
            {
                item.Category = new TrackedDataCategory(categoryToolStripComboBox.Text.Trim(' '));
            }

            Data.Add(item);
            RefreshTrackingButtons();
            RefreshCategoryPicker();
        }

        private async void toolStripButton1_Click(object sender, EventArgs e)
        {
            await stopTracking();
        }
        private async Task stopTracking()
        {
            await TrackingService.Stop();
            RefreshTimer.Stop();
            RefreshTrackingButtons();
            RefreshCategoryPicker();
            this.trackingStartTimeToolStripTextBox.Text = "--:--:--";
            this.categoryToolStripComboBox.Items.Clear();
            this.categoryToolStripComboBox.Text = "";
            this.trackingElapsedTimeToolStripTextBox.Text = TrackingService.ZeroTime;
            Data.Clear();
            // No need to close handles here, FileInfo doesn't use them
            file = null;
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
        }

        private HashSet<TrackedDataCategory> GetUsedCategories()
        {
            var result = new HashSet<TrackedDataCategory>();
            foreach (var value in Data)
            {
                if (value.Category == null)
                {
                    continue;
                }

                result.Add(value.Category);
            }

            return result;
        }

        private void RefreshCategoryPicker()
        {
            var items = this.categoryToolStripComboBox.Items;
            items.Clear();
            items.AddRange(GetUsedCategories().ToArray());
        }

        private void dataGridViewMain_Paint(object sender, PaintEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            if (grid.Rows.Count == 0)
            {
                var font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));

                // figure out label position
                System.Drawing.SizeF labelSize = e.Graphics.MeasureString(Resources.Application_noDataLabel, font);
                float vertPos = (grid.Width - labelSize.Width) / 2;
                float horizPos = (grid.Height + grid.ColumnHeadersHeight - labelSize.Height) / 2;

                e.Graphics.DrawString(Resources.Application_noDataLabel, font, System.Drawing.Brushes.DimGray, new System.Drawing.PointF(vertPos < 0 ? 0 : vertPos, horizPos < grid.ColumnHeadersHeight ? grid.ColumnHeadersHeight : horizPos));
            }
        }

        private void dataGridViewMain_Resize(object sender, EventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            if (grid.Rows.Count == 0)
            {
                // repaint whole area to make sure our custom text gets drawn in correct place
                grid.Invalidate();
            }
        }

        private void categoryToolStripComboBox_TextUpdate(object sender, EventArgs e)
        {
            // make sure that there are no invalid characters in the category field
            ToolStripComboBox box = (ToolStripComboBox)sender;
            var text = box.Text;
            var original = text;

            Regex regex = new Regex("[^-_: \\w]");
            text = regex.Replace(text, "");

            if (original != text)
            {
                toolTip.Hide(this.categoryToolStripComboBox.Control);
                toolTip.Show(String.Format(Resources.Application_categoryToolTip_Text, "-_: "), this.categoryToolStripComboBox.Control, 5000);
            }

            if (text.Length > CATEGORY_MAXLENGTH)
            {
                text = text.Substring(0, CATEGORY_MAXLENGTH);
            }

            box.Text = text;
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

        public void ShowIdleAlert()
        {
            SetTotalTime();
            SetApplicationToFront();
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;
            Point form1Center = new Point(this.Left + this.Width / 2, this.Top + this.Height / 2);

            IdleDialog idlePopup = new IdleDialog(this);
            int form2X = form1Center.X - idlePopup.Width / 2;
            int form2Y = form1Center.Y - idlePopup.Height / 2;
            idlePopup.StartPosition = FormStartPosition.Manual;
            idlePopup.Location = new Point(form2X, form2Y);
            idlePopup.Show();
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
    }
}