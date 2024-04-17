using MomoiAssist.Helpers;
using MomoiAssist.Helpers.PInvoke;
using MomoiAssist.Services;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace MomoiAssist.Views.Overlays
{
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    public partial class Overlay : Window
    {
        public Overlay()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;

        }
        private const uint EVENT_SYSTEM_MOVESIZESTART = 0x000A;
        private const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;
        private IntPtr winEventHook;

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        private void OnWinEvent(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (hwnd != windowHandle)
            {
                return;
            }
            Console.WriteLine("Event: " + eventType + " hwnd: " + hwnd);
            if (eventType == EVENT_SYSTEM_MOVESIZESTART)
            {
                // 窗口开始移动
                // Window starts moving
            }
            else if (eventType == EVENT_SYSTEM_MOVESIZEEND)
            {
                var rect = EmulatorWindowHelper.GetClientRect(hwnd);
                Left = rect.Left;
                Top = rect.Top;
                Height = rect.Height;
                Width = rect.Width;

            }

        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);


        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;


        WinEventDelegate winEventProc;
        private IntPtr windowHandle;
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            windowHandle = App.GetService<EmulatorWindowFinderService>().CurrentWindow?.Handle ?? IntPtr.Zero;
            winEventProc = new WinEventDelegate(OnWinEvent);
            winEventHook = SetWinEventHook(EVENT_SYSTEM_MOVESIZESTART, EVENT_SYSTEM_MOVESIZEEND, IntPtr.Zero, winEventProc, 0, 0, 0);
            var hWnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, extendedStyle | WS_EX_TOOLWINDOW);

        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            UnhookWinEvent(winEventHook);
        }


        private void TopBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            /* 当点击拖拽区域的时候，让窗口跟着移动
             (When clicking the drag area, make the window follow) */
            DragMove();
        }

    }
}
