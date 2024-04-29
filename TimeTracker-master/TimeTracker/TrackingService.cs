using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeTracker.Model;
using TimeTracker.Utilities;

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

        private LowLevelKeyboardProc _keyboardProc;
        private LowLevelMouseProc _mouseProc;
        private IntPtr _keyboardHookID = IntPtr.Zero;
        private IntPtr _mouseHookID = IntPtr.Zero;

        private int keystrokeCount = 0;
        private int mouseClickCount = 0;

        private const int ActivityThreshold = 80; // Adjust this threshold as needed
        //// Define your dialog here
        //private LowActivityDialog lowActivityDialog;
        //Activity Check -----------------------------------------------
        private DateTimeOffset _startTime;
        private Timer timer;

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
            if (Tracking)
            {
                throw new TrackingServiceException("Tracking has already started.");
            }

            this.Tracking = true;
            this.StartTime = DateTimeOffset.Now;
            ResetKeyCount();
            timer = new Timer();
            timer.Start();
            timer.Interval = 5 * 60 * 1000; // 5 minutes interval
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
                throw new TrackingServiceException("Tracking was not started.");
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

        private async void Timer_Tick(object sender, EventArgs e)
        {
            int keyStrokes = CheckActivity();
            idleCheckAfter1Min(keyStrokes);
            Bitmap screenshot = CaptureScreen();
            //var screenshotFolderPath = @"D:/Screenshots";
            //string fileName = $"screenshot_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
            //string filePath = System.IO.Path.Combine(screenshotFolderPath, fileName);
            //screenshot.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

            //Save in db
            byte[] screenshotBytes;
            using (MemoryStream stream = new MemoryStream())
            {
                screenshot.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                screenshotBytes = stream.ToArray();
            }
            DBAccessContext dBAccessContext = new DBAccessContext();
            await dBAccessContext.SaveScreenshot(screenshotBytes, keyStrokes);
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
            if (nCode >= 0 && (wParam == (IntPtr)WM_LBUTTONDOWN || wParam == (IntPtr)WM_RBUTTONDOWN))
            {
                mouseClickCount++;
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }
        public int CheckActivity(bool is1MinCheck = false)
        {
            //if (keystrokeCount <= ActivityThreshold || mouseClickCount <= ActivityThreshold)
            //{
            //    Console.WriteLine("------------------------------");
            //}
            var totalKeyStrokeCount = keystrokeCount + mouseClickCount;
            // Reset counters for next interval
            if (!is1MinCheck)
            {
                keystrokeCount = 0;
                mouseClickCount = 0;
            }
            return totalKeyStrokeCount;
        }

        public void ResetKeyCount()
        {
            keystrokeCount = 0;
            mouseClickCount = 0;
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
            int keyStrokesIn1Min = CheckActivity(true);
            if (keyStrokesIn1Min + oldKeyStrokes <= 5)
            {
                await _application.Pause_Tracking();
                _application.ShowIdleAlert();
                IntPtr handle = _application.Handle;
                SetForegroundWindow(handle);
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