using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace AutoClicker
{
    class ClickManager
    {
        private string windowFocus;
        private int min;
        private int max;
        private bool simulateHumanClick;
        private Thread clickThread;
        private Random random = new Random();
        public bool IsAutoClickEnabled { set; get; }

        public void StartAutoClicker(string windowFocus, int min, int max, bool simulateHumanClick)
        {
            Console.WriteLine(windowFocus + min + " " + max + simulateHumanClick);
            IsAutoClickEnabled = true;
            this.windowFocus = windowFocus;
            this.min = min;
            this.max = max;
            this.simulateHumanClick = simulateHumanClick;
            this.clickThread = new Thread(new ThreadStart(Autoclicking));
            this.clickThread.Start();
        }

        public void StopAutoClicker()
        {
            IsAutoClickEnabled = false;
        }

        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public static MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        public static void MouseEvent(MouseEventFlags value)
        {
            MousePoint position = GetCursorPosition();

            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 0,
                 0)
                ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        public static string GetCaptionOfActiveWindow()
        {
            var strTitle = string.Empty;
            var handle = GetForegroundWindow();
            // Obtain the length of the text   
            var intLength = GetWindowTextLength(handle) + 1;
            var stringBuilder = new StringBuilder(intLength);
            if (GetWindowText(handle, stringBuilder, intLength) > 0)
            {
                strTitle = stringBuilder.ToString();
            }
            return strTitle;
        }

        private void Autoclicking()
        {
            while (IsAutoClickEnabled)
            {
                IntPtr zero = IntPtr.Zero;
                for (int i = 0; (i < 60) && (zero == IntPtr.Zero); i++)
                {
                    Console.WriteLine("attempt");
                    //Thread.Sleep(500);
                    //MouseEvent(MouseEventFlags.LeftDown | MouseEventFlags.LeftUp);
                    zero = FindWindow(null, windowFocus);
                    if (zero == IntPtr.Zero)
                    {
                        Thread.Sleep(500);
                    }
                }

                if (zero != IntPtr.Zero)
                {
                    Console.WriteLine("Found it");
                    SetForegroundWindow(zero);
                    Console.WriteLine("Making click");
                    MouseEvent(MouseEventFlags.LeftDown);
                    if (simulateHumanClick)
                    {
                        int waiter = random.Next(45, 140);
                        Console.WriteLine("Waiting");
                        Thread.Sleep(waiter);
                    }
                    MouseEvent(MouseEventFlags.LeftUp);
                }
                int waitTime = random.Next(min, max);
                Thread.Sleep(waitTime);
            }
        }
    }
}
