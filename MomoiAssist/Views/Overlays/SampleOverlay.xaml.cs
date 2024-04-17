using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace MomoiAssist.Views.Overlays
{
    /// <summary>
    /// Interaction logic for SampleOverlay1.xaml
    /// </summary>
    public partial class SampleOverlay : Window
    {
        public SampleOverlay()
        {
            InitializeComponent();
        }


        public bool IsClickThrough = true;

        public bool LockWindow = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsClickThrough)
                EnableClickThrough();
            MouseLeftButtonDown += Window_MouseLeftButtonDown;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("MouseLeftButtonDown");
            if (!LockWindow)
                this.DragMove();
        }

        private void EnableClickThrough()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_EXSTYLE, GetWindowLong(hwnd, GWL_EXSTYLE) | WS_EX_TRANSPARENT);
        }

        private void DisableClickThrough()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_EXSTYLE, GetWindowLong(hwnd, GWL_EXSTYLE) & ~WS_EX_TRANSPARENT);
        }

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
    }
}
