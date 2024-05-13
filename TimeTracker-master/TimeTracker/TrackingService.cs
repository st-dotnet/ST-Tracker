using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeTracker.Model;
using TimeTracker.Utilities;
using AForge.Video.DirectShow;
using System.Drawing.Imaging;
using System.Net.NetworkInformation;

namespace TimeTracker
{
    /// <summary>
    /// A helper service managing tracking
    /// </summary>
    public class TrackingService
    {
        private readonly TimeTracker.Form.Application _application;
        public TrackingService(TimeTracker.Form.Application application)
        {
            _application = application;
        }
        //Activity Check -----------------------------------------------
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_MOUSEWHEEL = 0x020A;

        private LowLevelKeyboardProc _keyboardProc;
        private LowLevelMouseProc _mouseProc;
        private IntPtr _keyboardHookID = IntPtr.Zero;
        private IntPtr _mouseHookID = IntPtr.Zero;

        private int keystrokeCount = 0;
        private int mouseClickCount = 0;
        private int mouseWheelCount = 0;

        private const int ActivityThreshold = 80; // Adjust this threshold as needed
        //// Define your dialog here
        //private LowActivityDialog lowActivityDialog;
        //Activity Check -----------------------------------------------
        private DateTimeOffset _startTime;
        private DateTimeOffset StartTimeInterval;
        private System.Windows.Forms.Timer timer;

        /// <summary>
        /// Stores whether we are currently tracking
        /// </summary>
        public bool Tracking { get; private set; }

        /// <summary>
        /// Returns tracking start time
        /// </summary>
        public DateTimeOffset StartTime
        {
            get
            {
                if (!Tracking)
                {
                    throw new TrackingServiceException("Cannot return tracking start time - tracking has not started.");
                }

                return _startTime;
            }

            private set { _startTime = value; }
        }

        /// <summary>
        /// Starts tracking
        /// </summary>
        /// <returns></returns>
        public DateTimeOffset Start()
        {
            CheckInternetConnected();
            if (Tracking)
            {
                throw new TrackingServiceException("Tracking has already started.");
            }

            this.Tracking = true;
            this.StartTime = DateTimeOffset.Now;
            this.StartTimeInterval = this.StartTime;
            ResetKeyCount();
            int timeIntervalMinutes;
            if (!int.TryParse(ConfigurationManager.AppSettings["TimeIntervalInMinutes"], out timeIntervalMinutes))
            {
                timeIntervalMinutes = 5; //Default time set to 5 minutes
            }
            timer = new System.Windows.Forms.Timer();
            timer.Start();
            timer.Interval = timeIntervalMinutes * 30 * 1000;
            timer.Tick += Timer_Tick;
            return this.StartTime;
        }

        /// <summary>
        /// Stops tracking
        /// </summary>
        /// <returns>The resulting TimeTrackerData</returns>
        public TimeTrackerData Stop()
        {
            if (!Tracking)
            {
                return null;
                //throw new TrackingServiceException("Tracking was not started.");
            }

            this.Tracking = false;
            ResetKeyCount();
            timer.Stop();
            return new TimeTrackerData(_startTime, DateTimeOffset.Now);
        }

        /// <summary>
        /// Returns the elapsed time since tracking started
        /// </summary>
        public String Elapsed
        {
            get
            {
                if (!Tracking)
                {
                    throw new TrackingServiceException("Tracking was not started.");
                }

                System.TimeSpan timeSpan = DateTimeOffset.Now.Subtract(this.StartTime);

                return timeSpan.Format();
            }
        }
        public String ZeroTime
        {
            get
            {
                System.TimeSpan timeSpan = TimeSpan.Zero;

                return timeSpan.Format();
            }
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            await SaveTimerDataAtEveryInterval();
            int keyStrokes = CheckActivity();//Get KeyStrokes
            await Captures(keyStrokes);//Capture Camera Photo + ScreenShot
            idleCheckAfter1Min(keyStrokes);
        }
        public async Task<bool> CheckInternetConnected()
        {
            var internetAvailable = false;
            if (IsInternetAvailable())
            {
                internetAvailable = true;
            }
            _application.InternetStatus(internetAvailable);
            return internetAvailable;
        }
        private static bool IsInternetAvailable()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var result = ping.Send("8.8.8.8", 2000);
                    return (result.Status == IPStatus.Success);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        private async Task SaveTimerDataAtEveryInterval()
        {
            DBAccessContext dBAccessContext = new DBAccessContext();
            TimeSpan elapsedTime = GetIntervalTimeElasped();
            var internetAvailavble = await CheckInternetConnected();
            if(internetAvailavble)
            {
                await dBAccessContext.AddUpdateTrackerInfo(elapsedTime);
            }
            else
            {
                await dBAccessContext.StoreTrackerDataToLocal(elapsedTime);
            }
            this.StartTimeInterval = DateTimeOffset.Now;
        }
        private async Task Captures(int keyStrokes)
        {
            Bitmap cameraCapture = await CapturePhotoAsync();
            if (cameraCapture != null)
            {
                await SaveCaptures(cameraCapture, null, true);
            }
            Bitmap screenshot = CaptureScreen();
            await SaveCaptures(screenshot, keyStrokes, false);
        }
        private async Task SaveCaptures(Bitmap screenshot, int? keyStrokes, bool isCameraCapture)
        {
            DBAccessContext dBAccessContext = new DBAccessContext();
            byte[] screenshotBytes;
            using (MemoryStream stream = new MemoryStream())
            {
                screenshot.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                screenshotBytes = stream.ToArray();
            }
            await dBAccessContext.SaveScreenshot(screenshotBytes, keyStrokes, isCameraCapture);
        }
        public TimeSpan GetIntervalTimeElasped()
        {
            TimeSpan elapsedTime = DateTimeOffset.Now - StartTimeInterval;
            return elapsedTime;
        }
        /// <summary>
        /// Capture screenshot
        /// </summary>
        /// <returns></returns>
        public Bitmap CaptureScreen()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
            return bitmap;
        }
        static async Task<Bitmap> CapturePhotoAsync()
        {
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count > 0)
            {
                VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                Bitmap screenshot = null;

                var completionSource = new TaskCompletionSource<bool>();
                videoSource.NewFrame += (sender, eventArgs) =>
                {
                    screenshot = (Bitmap)eventArgs.Frame.Clone();
                    completionSource.TrySetResult(true);
                };

                videoSource.Start();

                await Task.Delay(5000);

                videoSource.SignalToStop();
                videoSource.WaitForStop();

                await completionSource.Task;

                return screenshot;
            }
            return null;
        }
            private static void VideoSource_NewFrame(Bitmap bitmap)
        {
            string screenshotFolderPath = @"D:/Screenshots"; // Change this to your desired folder path
            string fileName = $"screenshot_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.jpg"; // Change the extension to match your desired format
            string filePath = System.IO.Path.Combine(screenshotFolderPath, fileName);
            bitmap.Save(filePath, ImageFormat.Jpeg);
        }

        //Activity Check -----------------------------------------------
        public void InitializeHooks()
        {
            _keyboardProc = HookCallbackKeyboard;
            _mouseProc = HookCallbackMouse;

            _keyboardHookID = SetHook(WH_KEYBOARD_LL, _keyboardProc);
            _mouseHookID = SetHook(WH_MOUSE_LL, _mouseProc);
        }
        private IntPtr SetHook(int hookType, Delegate hookProc)
        {
            using (Process currentProcess = Process.GetCurrentProcess())
            using (ProcessModule currentModule = currentProcess.MainModule)
            {
                return SetWindowsHookEx(hookType, hookProc, GetModuleHandle(currentModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallbackKeyboard(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                keystrokeCount++;
            }
            return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
        }

        private IntPtr HookCallbackMouse(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (wParam == (IntPtr)WM_LBUTTONDOWN || wParam == (IntPtr)WM_RBUTTONDOWN)
                {
                    // Left or right mouse button down
                    mouseClickCount++;
                }
                else if (wParam == (IntPtr)WM_MOUSEWHEEL)
                {
                    // Mouse wheel scrolled
                    mouseWheelCount++;
                }
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }
        public int CheckActivity(bool is1MinCheck = false)
        {
            //if (keystrokeCount <= ActivityThreshold || mouseClickCount <= ActivityThreshold)
            //{
            //    Console.WriteLine("------------------------------");
            //}
            var totalKeyStrokeCount = keystrokeCount + mouseClickCount + mouseWheelCount;
            // Reset counters for next interval
            if (!is1MinCheck)
            {
                ResetKeyCount();
            }
            return totalKeyStrokeCount;
        }

        public void ResetKeyCount()
        {
            keystrokeCount = 0;
            mouseClickCount = 0;
            mouseWheelCount = 0;
        }
        //protected override void OnFormClosing(FormClosingEventArgs e)
        //{
        //    base.OnFormClosing(e);
        //    UnhookWindowsHookEx(_keyboardHookID);
        //    UnhookWindowsHookEx(_mouseHookID);
        //}

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        //Activity Check -----------------------------------------------
        //Idle Check -----------------------------------------------
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private async void idleCheckAfter1Min(int oldKeyStrokes)
        {
            await Task.Delay(TimeSpan.FromMinutes(1));
            int keysThresholdToStopTracking;
            if (!int.TryParse(ConfigurationManager.AppSettings["KeysThresholdToStopTracking"], out keysThresholdToStopTracking))
            {
                keysThresholdToStopTracking = 5; //Default key threshhold set to 5 keys in a default 5 minutes
            }
            int keyStrokesIn1Min = CheckActivity(true);
            if (keyStrokesIn1Min + oldKeyStrokes <= keysThresholdToStopTracking)
            {
                await _application.Pause_Tracking();
                _application.ShowIdleAlert();
                //IntPtr handle = _application.Handle;
                //SetForegroundWindow(handle);
            }
        }
        //Idle Check -----------------------------------------------
    }

    /// <summary>
    /// Tracking Service Exception
    /// </summary>
    public class TrackingServiceException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public TrackingServiceException(string message) : base(message)
        {
        }
    }
}