using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing.Imaging;
using MomoiAssist.Models;
using System.IO;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Security.Permissions;
using System.Reflection.Metadata;

namespace MomoiAssist.Helpers
{
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
        public int Width => Right - Left;
        public int Height => Bottom - Top;
        public int X => Left;
        public int Y => Top;
        public Rectangle ToRectangle() => Rectangle.FromLTRB((int)Left, (int)Top, (int)Right, (int)Bottom);
        public Rectangle ToRectangleOffset(POINT p) => Rectangle.FromLTRB((int)Left + p.x, (int)Top + p.y, (int)Right + p.x, (int)Bottom);
    }

    public struct POINT
    {
        public int x;
        public int y;
    }

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
        static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, out POINT point);


       
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

        public static Bitmap CaptureWindowPrintWindow(IntPtr hWnd)
        {
            GetWindowRect(hWnd, out RECT RC);
            var rc = RC.ToRectangle();
            Console.WriteLine("Screen Capture with PrintWindow: " + rc.Left + " " + rc.Top + " " +rc.Right + " " + rc.Bottom);
            Console.WriteLine("Width: " + rc.Width + " Height: " + rc.Height);
            Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format24bppRgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap = gfxBmp.GetHdc();

            PrintWindow(hWnd, hdcBitmap, 0);

            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();

            return bmp;
        }

        public static Bitmap CaptureWindowGDI(IntPtr hWnd)
        {
            GetWindowRect(hWnd, out RECT RC);
            Console.WriteLine("Screen Capture: " + RC.Left + " " + RC.Top + " " + RC.Right + " " + RC.Bottom);
            Console.WriteLine(RC.Width + " " + RC.Height);
            ClientToScreen(hWnd, out POINT point);
            var rc = RC;
            int Width = rc.Width;
            int Height = rc.Height;
            int titleBarHeight = (int)System.Windows.SystemParameters.CaptionHeight;
            Height -= rc.Top - point.y;
            
            IntPtr windowDC = GetWindowDC(hWnd);
            Console.WriteLine("Screen Capture with GDI: " + rc.Left + " " + rc.Top + " " + Width + " " + Height);
            Console.WriteLine("Boarder: " + (rc.Left-point.x) + " " + (rc.Top-point.y));

            if (Width <= 0 || Height <= 0)
            {

                return null;
            }

            Bitmap screenshot = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            using (Graphics gfxScreenshot = Graphics.FromImage(screenshot))
            {
                IntPtr gfxDC = gfxScreenshot.GetHdc();
                BitBlt(gfxDC, 0, 0, Width, Height, windowDC, 0, 0, SRCCOPY);
                gfxScreenshot.ReleaseHdc(gfxDC);
            }

            ReleaseDC(hWnd, windowDC);

            return screenshot;
        }

        //public static Bitmap CaptureWindowCopyScreen(IntPtr hWnd)
        //{
        //    GetWindowRect(hWnd, out RECT rc);

        //    Console.WriteLine("Screen Capture with CopyFromScreen: " + rc.Top + " " + rc.Left + " " + rc.Width + " " + rc.Height);
        //    Bitmap bitmap = new Bitmap(rc.Width, rc.Height);
        //    Graphics g = Graphics.FromImage(bitmap);

        //    g.CopyFromScreen(rc.X, rc.Y, 0, 0, new System.Drawing.Size(rc.Width, rc.Height));

        //    //MemoryStream memoryStream = new MemoryStream();
        //    //bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

        //    return bitmap;
        //}
        

    }
}
