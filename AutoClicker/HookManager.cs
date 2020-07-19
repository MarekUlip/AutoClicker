using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace AutoClicker
{
    interface KeyBindingCallback
    {
        void onKeyBindingChange(Keys binding);
        Keys getSetKeyBinding();
        void onBindedKeyPush();
    }
    class HookManager
    {
        private static KeyBindingCallback callback;
        public static bool isChangingKey = false;
        public HookManager()
        {
            _hookID = SetHook(_proc);

        }

        public void setOnKeyBindingChangeListener(KeyBindingCallback callback)
        {
            HookManager.callback = callback;
        }

        public void stopHookManager()
        {
            UnhookWindowsHookEx(_hookID);
        }
        private const int WH_KEYBOARD_LL = 13;

        private const int WM_KEYDOWN = 0x0100;

        private static LowLevelKeyboardProc _proc = HookCallback;

        private static IntPtr _hookID = IntPtr.Zero;
        private string keyString = "";
        private static IntPtr SetHook(LowLevelKeyboardProc proc)

        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(

            int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(

            int nCode, IntPtr wParam, IntPtr lParam)

        {

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)

            {

                int vkCode = Marshal.ReadInt32(lParam);
                Keys codeString = (Keys)vkCode;
                if (isChangingKey)
                {
                    HookManager.callback.onKeyBindingChange(codeString);
                }
                else
                {
                    if (codeString.Equals(callback.getSetKeyBinding()))
                    {
                        callback.onBindedKeyPush();
                    }
                }
                /*keyString += codeString;
                Console.WriteLine((Keys)vkCode);
                if (keyString.Length > 100 || codeString == Keys.Return)
                {
                    WriteToFile();
                }*/
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);

        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern IntPtr SetWindowsHookEx(int idHook,

            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,

            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
