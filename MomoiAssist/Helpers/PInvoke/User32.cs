using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace MomoiAssist.Helpers.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
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
        public Rectangle ToRectangle() => Rectangle.FromLTRB(Left, Top, Right, Bottom);
        public Rectangle ToRectangleOffset(POINT p) => Rectangle.FromLTRB(Left + p.X, Top + p.Y, Right + p.X, Bottom);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    public delegate void WinEventDelegate(nint hWinEventHook, uint eventType,
            nint hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    static class User32
    {
        public delegate bool EnumWindowsProc(nint hWnd, nint lParam);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(nint hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(nint hWnd, nint hDC);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(nint hWnd, out RECT rect);

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(nint hWnd, nint hdcBlt, int nFlags);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(nint hWnd, out POINT point);


        [DllImport("user32.dll")]
        public static extern nint SetWinEventHook(
            uint eventMin,
            uint eventMax,
            nint hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc,
            uint idProcess,
            uint idThread,
            uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(nint hWinEventHook);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, nint lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowText(nint hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(nint hWnd);

        [DllImport("user32.dll")]
        public static extern nint GetWindowDC(nint hWnd);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);


    }
}
