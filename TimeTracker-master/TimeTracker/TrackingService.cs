using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeTracker.Utilities;
using AForge.Video.DirectShow;
using System.Configuration;

namespace TimeTracker
{
    public class TrackingService
    {
        private readonly TimeTracker.Form.Application _application;
        private readonly DBAccessContext _dBAccessContext;
        private readonly InternetManager _internetManager;

        public TrackingService(TimeTracker.Form.Application application, DBAccessContext dBAccessContext, InternetManager internetManager)
        {
            _application = application;
            _dBAccessContext = dBAccessContext;
            _internetManager = internetManager;
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
        private System.Windows.Forms.Timer timerForIdle;
        private int keystrokesForIdle = 0;

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

        public async Task<DateTimeOffset> Start()
        {
            if (Tracking)
            {
                throw new TrackingServiceException("Tracking has already started.");
            }

            this.Tracking = true;
            this.StartTime = DateTimeOffset.Now;
            this.StartTimeInterval = this.StartTime;
            int timeIntervalMinutes;
            if (!int.TryParse(ConfigurationManager.AppSettings["TimeIntervalInMinutes"], out timeIntervalMinutes))
            {
                timeIntervalMinutes = 10; //Default Idle time set to 10 minutes
            }
            timer = new System.Windows.Forms.Timer();
            timer.Start();
            timer.Interval = 2 * 60 * 1000; //Interval For Screenshot & Time Logg in DB after every 3 minutes
            timer.Tick += Timer_Tick;
            //Timer For Idle 
            timerForIdle = new System.Windows.Forms.Timer();
            timerForIdle.Start();
            timerForIdle.Interval = timeIntervalMinutes * 60 * 1000;
            timerForIdle.Tick += Timer_Tick_ForIldeCheck;

            await SaveTimerData();
            return this.StartTime;
        }
        private void ResetIdleTimer()
        {
            if (Tracking)
            {
                timerForIdle.Stop();
                timerForIdle.Start();
            }
        }

        public async Task Stop()
        {
            if (Tracking)
            {
                await SaveTimerData();
                this.Tracking = false;
                timer.Stop();
                timerForIdle.Stop();
            }
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
            await SaveTimerData();
        }
        private async void Timer_Tick_ForIldeCheck(object sender, EventArgs e)
        {
            idleTimeCheck();
        }

        private async Task SaveTimerData()
        {
            var internetAvailavble = await _internetManager.CheckInternetConnected();
            int keyStrokes = CheckActivity();//Get KeyStrokes
            keystrokesForIdle += keyStrokes;
            TimeSpan elapsedTime = GetIntervalTimeElasped();
            this.StartTimeInterval = DateTimeOffset.Now;
            await _dBAccessContext.AddUpdateTrackerInfo(elapsedTime);
            if (internetAvailavble)
            {
                await Captures(keyStrokes);//Capture Camera Photo + ScreenShot
            }
            _application.SetTotalTime();
        }

        private async Task Captures(int keyStrokes)
        {
            Bitmap cameraCapture = await CapturePhotoAsync();
            if (cameraCapture != null)
            {
                await SaveCaptures(cameraCapture, null, true);
            }
            await CaptureScreen(keyStrokes);
        }

        private async Task SaveCaptures(Bitmap screenshot, int? keyStrokes, bool isCameraCapture)
        {
            byte[] screenshotBytes;
            using (MemoryStream stream = new MemoryStream())
            {
                screenshot.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                screenshotBytes = stream.ToArray();
            }
            await _dBAccessContext.SaveScreenshot(screenshotBytes, keyStrokes, isCameraCapture);
        }

        public TimeSpan GetIntervalTimeElasped()
        {
            TimeSpan elapsedTime = DateTimeOffset.Now - StartTimeInterval;
            return elapsedTime;
        }

        public async Task CaptureScreen(int? keyStrokes)
        {
            Screen[] screens = Screen.AllScreens;
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i].Bounds.Contains(Cursor.Position))
                {
                    Bitmap screenshot = new Bitmap(screens[i].Bounds.Width, screens[i].Bounds.Height);
                    using (Graphics graphics = Graphics.FromImage(screenshot))
                    {
                        graphics.CopyFromScreen(screens[i].Bounds.Location, Point.Empty, screens[i].Bounds.Size);
                    }
                    await SaveCaptures(screenshot, keyStrokes, false);
                    break;
                }
            }
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
                ResetIdleTimer();
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
                    ResetIdleTimer();
                }
                else if (wParam == (IntPtr)WM_MOUSEWHEEL)
                {
                    // Mouse wheel scrolled
                    mouseWheelCount++;
                    ResetIdleTimer();
                }
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }
        public int CheckActivity()
        {
            var totalKeyStrokeCount = keystrokeCount + mouseClickCount + mouseWheelCount;
            ResetKeyCount();
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
        private async Task idleTimeCheck()
        {
            timerForIdle.Stop();
            await _application.Pause_Tracking();
            _application.ShowIdle();
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