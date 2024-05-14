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

namespace TimeTracker
{
    public class TrackingService
    {
        private readonly TimeTracker.Form.Application _application;
        public TrackingService(TimeTracker.Form.Application application)
        {
            _application = application;
        }

        #region Activity Check Fields

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
        #endregion

        public bool Tracking { get; private set; }
        private DateTimeOffset _startTime;
        private DateTimeOffset StartTimeInterval;
        private System.Windows.Forms.Timer timer;

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

        public DateTimeOffset Start()
        {
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

        public TimeTrackerData Stop()
        {
            if (!Tracking)
            {
                return null;
            }

            this.Tracking = false;
            ResetKeyCount();
            timer.Stop();
            return new TimeTrackerData(_startTime, DateTimeOffset.Now);
        }

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
            InternetManager ineterntM = new InternetManager(_application);
            var internetAvailavble = await ineterntM.CheckInternetConnected();
            int keyStrokes = CheckActivity();//Get KeyStrokes
            idleCheckAfter1Min(keyStrokes);
            if (internetAvailavble)
            {
                await SaveTimerDataAtEveryInterval(true);
                await Captures(keyStrokes);//Capture Camera Photo + ScreenShot
            }
            else
            {
                await SaveTimerDataAtEveryInterval(false);
            }
        }

        private async Task SaveTimerDataAtEveryInterval(bool isStoreToDB)
        {
            DBAccessContext dBAccessContext = new DBAccessContext();
            TimeSpan elapsedTime = GetIntervalTimeElasped();
            if (isStoreToDB)
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

        #region Activity Check Methods/Code
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

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        #endregion

        #region Idle Time Detection Code
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
            }
        }
        #endregion
    }

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