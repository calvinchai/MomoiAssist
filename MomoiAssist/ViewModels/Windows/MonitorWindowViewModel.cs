using MomoiAssist.Models;
using MomoiAssist.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Wpf.Ui;

namespace MomoiAssist.ViewModels.Windows
{
    public partial class MonitorWindowViewModel : ObservableObject
    {

        private readonly WindowFinderService _windowFinderService;
        [ObservableProperty]
        private string title = "Monitor Window";

        [ObservableProperty]
        BitmapImage? screenshot=null;
        public MonitorWindowViewModel(WindowFinderService windowFinderService)
        {
            _windowFinderService = windowFinderService;
            StartImageUpdateTimer();
        }

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
                //Screenshot = null;
            }

        }

    }
}
