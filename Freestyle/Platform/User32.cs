using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Freestyle.Platform
{
    class User32
    {
        internal class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);


            public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
            [DllImport("user32.dll")]
            public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
            [DllImport("user32.dll")]
            public static extern void UnhookWinEvent(IntPtr hook);

            public const uint WINEVENT_OUTOFCONTEXT = 0;
            public enum SystemEvents : uint
            {
                EVENT_SYSTEM_FOREGROUND = 3,
                // Note: Incomplete
            }

            [DllImport("user32.dll")]
            public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out IntPtr ProcessId);
        }

        public static int GetWindowThreadProcessId(IntPtr hWnd)
        {
            IntPtr pid;
            Platform.User32.NativeMethods.GetWindowThreadProcessId(hWnd, out pid);
            return pid.ToInt32();
        }

        public static IntPtr GetForegroundWindow()
        {
            return NativeMethods.GetForegroundWindow();
        }

        public static string GetClassName(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder(255);
            NativeMethods.GetClassName(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        public static string GetWindowText(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder(255);
            NativeMethods.GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }
    }
}
