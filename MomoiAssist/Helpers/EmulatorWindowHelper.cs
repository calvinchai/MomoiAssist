using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing.Imaging;
using MomoiAssist.Models;

namespace MomoiAssist.Helpers
{
    public class EmulatorWindowHelper
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out Rectangle rect);
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(
        uint eventMin,
        uint eventMax,
        IntPtr hmodWinEventProc,
        WinEventDelegate lpfnWinEventProc,
        uint idProcess,
        uint idThread,
        uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private const uint EVENT_OBJECT_DESTROY = 0x8000;
        private const uint WINEVENT_OUTOFCONTEXT = 0;

        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
        IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private static IntPtr winEventHook;
        private const int SRCCOPY = 0x00CC0020;


        public static string GetWindowName(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            StringBuilder windowText = new StringBuilder(length + 1);
            GetWindowText(hWnd, windowText, windowText.Capacity);
            return windowText.ToString();
        }


        public static IntPtr FindWindow(string title)
        {
            IntPtr foundWindow = IntPtr.Zero;

            EnumWindows((hWnd, lParam) =>
            {
                if (GetWindowName(hWnd).Contains(title))
                {
                    foundWindow = hWnd;
                    return false; 
                }
                return true; 
            }, IntPtr.Zero);

            return foundWindow;
        }

        internal static List<IntPtr> GetAllWindows()
        {
            List<IntPtr> windows = new List<IntPtr>();

            EnumWindows((hWnd, lParam) =>
            {
                windows.Add(hWnd);
                return true; 
            }, IntPtr.Zero);

            return windows;
        }

        internal static List<String> GetAllWindowNames()
        {
            List<String> windows = new List<String>();

            EnumWindows((hWnd, lParam) =>
            {
                windows.Add(GetWindowName(hWnd));
                return true; 
            }, IntPtr.Zero);

            return windows;
        }


        public static Bitmap CaptureWindow(IntPtr hWnd)
        {



            //Rectangle rc;
            //GetWindowRect(hWnd, out rc);

            //Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format24bppRgb);
            //Graphics gfxBmp = Graphics.FromImage(bmp);
            //IntPtr hdcBitmap = gfxBmp.GetHdc();

            //PrintWindow(hWnd, hdcBitmap, 0);

            //gfxBmp.ReleaseHdc(hdcBitmap);
            //gfxBmp.Dispose();

            //return bmp;

            GetWindowRect(hWnd, out Rectangle windowRect);
            int width = windowRect.Right - windowRect.Left;
            int height = windowRect.Bottom - windowRect.Top;

            IntPtr windowDC = GetWindowDC(hWnd);
            Console.WriteLine(width + " " + height);
            Console.WriteLine(windowRect.Height + " " + windowRect.Width);
            if (width <= 0 || height <= 0)
            {

                return null;
            }

            Bitmap screenshot = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            using (Graphics gfxScreenshot = Graphics.FromImage(screenshot))
            {
                IntPtr gfxDC = gfxScreenshot.GetHdc();
                BitBlt(gfxDC, 0, 0, width, height, windowDC, 0, 0, SRCCOPY);
                gfxScreenshot.ReleaseHdc(gfxDC);
            }

            ReleaseDC(hWnd, windowDC);

            return screenshot;
        }

        

    }
}
