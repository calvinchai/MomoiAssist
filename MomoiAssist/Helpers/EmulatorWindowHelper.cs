using MomoiAssist.Helpers.PInvoke;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace MomoiAssist.Helpers
{

    public class EmulatorWindowHelper
    {

        public static string GetWindowName(IntPtr hWnd)
        {
            int length = User32.GetWindowTextLength(hWnd);
            StringBuilder windowText = new StringBuilder(length + 1);
            User32.GetWindowText(hWnd, windowText, windowText.Capacity);
            return windowText.ToString();
        }


        public static IntPtr FindWindow(string title)
        {
            IntPtr foundWindow = IntPtr.Zero;

            User32.EnumWindows((hWnd, lParam) =>
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

            User32.EnumWindows((hWnd, lParam) =>
            {
                windows.Add(hWnd);
                return true;
            }, IntPtr.Zero);

            return windows;
        }

        internal static List<String> GetAllWindowNames()
        {
            List<String> windows = new List<String>();

            User32.EnumWindows((hWnd, lParam) =>
            {
                windows.Add(GetWindowName(hWnd));
                return true;
            }, IntPtr.Zero);

            return windows;
        }

        public static Bitmap CaptureWindowPrintWindow(IntPtr hWnd)
        {
            User32.GetWindowRect(hWnd, out RECT rc);
            Console.WriteLine("Screen Capture with PrintWindow: " + rc.Left + " " + rc.Top + " " + rc.Right + " " + rc.Bottom);
            Console.WriteLine("Width: " + rc.Width + " Height: " + rc.Height);
            Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format24bppRgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap = gfxBmp.GetHdc();

            User32.PrintWindow(hWnd, hdcBitmap, 0);

            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();

            return bmp;
        }

        




        public static Bitmap? CaptureWindowGDI(IntPtr hWnd)
        {
            var windowRect = GetWindowRect(hWnd);
            var clientRect = GetClientRect(hWnd);
            int TopOffset = clientRect.Top - windowRect.Top;
            int LeftOffset = clientRect.Left - windowRect.Left;
            return CaptureWindowGDI(hWnd, LeftOffset, TopOffset, clientRect.Width, clientRect.Height);
        }

        const int SRCCOPY = 0x00CC0020;
        public static Bitmap? CaptureWindowGDI(IntPtr hWnd, int nXsrc, int nYsrc, int nWidth, int nHeight)
        {
            IntPtr windowDC = User32.GetWindowDC(hWnd);
            if (windowDC == IntPtr.Zero) 
            { 
                return null;
            }
            if (nWidth <= 0 || nHeight <= 0)
            {
                return null;
            }
            Bitmap screenshot = new Bitmap(nWidth, nHeight, PixelFormat.Format24bppRgb);
            using (Graphics gfxScreenshot = Graphics.FromImage(screenshot))
            {
                IntPtr gfxDC = gfxScreenshot.GetHdc();
                Gdi32.BitBlt(gfxDC, 0, 0, nWidth, nHeight, windowDC, nXsrc, nYsrc, SRCCOPY);
                gfxScreenshot.ReleaseHdc(gfxDC);
            }

            User32.ReleaseDC(hWnd, windowDC);

            return screenshot;
        }


        public static Bitmap CaptureWindowCopyScreen(IntPtr hWnd)
        {
            User32.GetWindowRect(hWnd, out RECT rc);

            Bitmap bitmap = new Bitmap(rc.Width, rc.Height);
            Graphics g = Graphics.FromImage(bitmap);

            g.CopyFromScreen(rc.X, rc.Y, 0, 0, new System.Drawing.Size(rc.Width, rc.Height));


            return bitmap;
        }

        public static Rectangle GetWindowRect(IntPtr hWnd)
        {
            User32.GetWindowRect(hWnd, out RECT rect);
            return rect.ToRectangle();
        }

        public static Rectangle GetClientRect(IntPtr hWnd, int TopOffset = 35)
        {
            User32.GetWindowRect(hWnd, out RECT rect);
            User32.ClientToScreen(hWnd, out POINT point);
            int Top = TopOffset + point.Y;
            int Left = point.X;
            int Width = rect.Width - 2 * (point.X- rect.Left);
            int Height = rect.Height - (point.X-rect.Left) - (point.Y - rect.Top) - TopOffset;
            return new Rectangle(Left, Top, Width, Height);
        }

    }
}
