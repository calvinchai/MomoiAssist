using MomoiAssist.Models;
using MomoiAssist.Services;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MomoiAssist.ViewModels.Windows
{
    public partial class MonitorWindowViewModel : ObservableObject
    {

        private readonly EmulatorWindowFinderService _windowFinderService;
        [ObservableProperty]
        private string title = "Monitor Window";

        [ObservableProperty]
        BitmapImage? screenshot = null;
        public MonitorWindowViewModel(EmulatorWindowFinderService windowFinderService)
        {
            _windowFinderService = windowFinderService;
            StartImageUpdateTimer();
        }
        [ObservableProperty]
        public int top;
        [ObservableProperty]
        public int left;

        private DispatcherTimer timer;

        private void StartImageUpdateTimer()
        {
            timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += UpdateWindow;
            timer.Start();
        }


        private EmulatorWindow? currentWindow = null;

        private void UpdateWindow(object sender, EventArgs e)
        {
            if (_windowFinderService.CurrentWindow == null)
            {
                return;
            }
            if (_windowFinderService.CurrentWindow != currentWindow)
            {
                currentWindow = _windowFinderService.CurrentWindow;
                return;
            }

            if (currentWindow != null)
            {
                //Screenshot = EmulatorWindow.BitmapToImageSource((System.Drawing.Bitmap?)currentWindow.Screenshot);
                Screenshot = currentWindow.ScreenshotImage;
                Top = currentWindow.WindowRect.Top;
                Left = currentWindow.WindowRect.Left;

                //Screenshot = null;
            }

        }

    }
}
