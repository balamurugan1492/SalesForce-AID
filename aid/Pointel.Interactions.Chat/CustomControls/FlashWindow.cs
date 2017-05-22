using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Pointel.Interactions.Chat.CustomControls
{
    public static class FlashWindow
    {
        private struct FLASHWINFO
        {
            public uint cbSize;
            public IntPtr hwnd;
            public uint dwFlags;
            public uint uCount;
            public uint dwTimeout;
        }
        public const uint FLASHW_STOP = 0u;
        public const uint FLASHW_CAPTION = 1u;
        public const uint FLASHW_TRAY = 2u;
        public const uint FLASHW_ALL = 3u;
        public const uint FLASHW_TIMER = 4u;
        public const uint FLASHW_TIMERNOFG = 12u;
        private static bool Win2000OrLater
        {
            get
            {
                return Environment.OSVersion.Version.Major >= 5;
            }
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FlashWindow.FLASHWINFO pwfi);
        public static bool Flash(IntPtr handle)
        {
            if (FlashWindow.Win2000OrLater)
            {
                FlashWindow.FLASHWINFO fLASHWINFO = FlashWindow.Create_FLASHWINFO(handle, 15u, 4294967295u, 0u);
                return FlashWindow.FlashWindowEx(ref fLASHWINFO);
            }
            return false;
        }
        private static FlashWindow.FLASHWINFO Create_FLASHWINFO(IntPtr handle, uint flags, uint count, uint timeout)
        {
            FlashWindow.FLASHWINFO fLASHWINFO = default(FlashWindow.FLASHWINFO);
            fLASHWINFO.cbSize = Convert.ToUInt32(Marshal.SizeOf(fLASHWINFO));
            fLASHWINFO.hwnd = handle;
            fLASHWINFO.dwFlags = flags;
            fLASHWINFO.uCount = count;
            fLASHWINFO.dwTimeout = timeout;
            return fLASHWINFO;
        }
        public static bool Flash(IntPtr handle, uint count)
        {
            if (FlashWindow.Win2000OrLater)
            {
                FlashWindow.FLASHWINFO fLASHWINFO = FlashWindow.Create_FLASHWINFO(handle, 3u, count, 0u);
                return FlashWindow.FlashWindowEx(ref fLASHWINFO);
            }
            return false;
        }
        public static bool Start(IntPtr handle)
        {
            if (FlashWindow.Win2000OrLater)
            {
                FlashWindow.FLASHWINFO fLASHWINFO = FlashWindow.Create_FLASHWINFO(handle, 3u, 4294967295u, 0u);
                return FlashWindow.FlashWindowEx(ref fLASHWINFO);
            }
            return false;
        }
        public static bool Stop(IntPtr handle)
        {
            if (FlashWindow.Win2000OrLater)
            {
                FlashWindow.FLASHWINFO fLASHWINFO = FlashWindow.Create_FLASHWINFO(handle, 0u, 4294967295u, 0u);
                return FlashWindow.FlashWindowEx(ref fLASHWINFO);
            }
            return false;
        }
    }
}
