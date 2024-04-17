using MomoiAssist.Helpers.PInvoke;
using System.Windows.Interop;

namespace MomoiAssist.Helpers
{
    static class WindowHelper
    {
        public static void SetToolWindow(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var exStyle = User32.GetWindowLong(hwnd, (int)WindowLongParam.GWL_EXSTYLE);
            User32.SetWindowLong(hwnd, (int)WindowLongParam.GWL_EXSTYLE, exStyle | (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW);
        }

    }
}
