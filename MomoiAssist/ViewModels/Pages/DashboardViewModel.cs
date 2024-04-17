using MomoiAssist.Models;
using MomoiAssist.Services;
using MomoiAssist.Views.Overlays;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Wpf.Ui;

namespace MomoiAssist.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _duration = "Duration";

        [ObservableProperty]
        BitmapImage? screenshot = null;

        [RelayCommand]
        private void OnCounterIncrement()
        {
            //UpdateWindow();

            //Task.Run(UpdateDurationDisplay);
            Task.Run(StartOverlay);
            //App.GetService<Overlay>().Show();
            overlay = new Overlay();
            overlay.Show();
        }
        Overlay overlay;
        void StartOverlay()
        {
            using (var example = new Example(currentWindow.Handle))
            {
                example.Run();
            }


            //App.GetService<Overlay>().Hide();

        }


        public DashboardViewModel(LocalizationService localizationService, EmulatorWindowFinderService windowFinderService, ISnackbarService snackbarService)
        {
            _localizationService = localizationService;
            _windowFinderService = windowFinderService;
            _snackbarService = snackbarService;
            StartImageUpdateTimer();

        }

        private ISnackbarService _snackbarService;
        private LocalizationService _localizationService;
        private EmulatorWindowFinderService _windowFinderService;

        public async void UpdateDurationDisplay()
        {

            ImageRecognizer imageRecognizer = new ImageRecognizer();

            while (true)
            {
                String text = await imageRecognizer.RecognizeTextAsync();

                if (ImageRecognizer.IsValidDurationFormat(text))
                    Duration = text;

                //Trace.WriteLine(text);
            }

        }

        private DispatcherTimer timer;

        private void StartImageUpdateTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
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
                Screenshot = currentWindow.ScreenshotImage;
            }

        }

        //public void UpdateWindow()
        //{
        //    Console.WriteLine("Updating window");

        //    if (EmulatorWindows == null || EmulatorWindows.Count == 0)
        //    {
        //        EmulatorWindows = _windowFinderService.GetPossibleEmulatorWindows();
        //    }

        //    AllWindows = _windowFinderService.AllWindowNames;
        //    if (EmulatorWindows.Count > 0)
        //    {
        //        Screenshot = EmulatorWindows[0].ScreenshotImage;
        //    }
        //    foreach (EmulatorWindow emulatorWindow in EmulatorWindows)
        //    {
        //        Console.WriteLine(emulatorWindow.Title);
        //        Console.WriteLine(emulatorWindow.ScreenshotImage);
        //        emulatorWindow.UpdateScreenshot();
        //    }
        //}

        //BitmapImage BitmapToImageSource(Bitmap bitmap)
        //{
        //    using (MemoryStream memory = new MemoryStream())
        //    {
        //        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
        //        memory.Position = 0;
        //        BitmapImage bitmapimage = new BitmapImage();
        //        bitmapimage.BeginInit();
        //        bitmapimage.StreamSource = memory;
        //        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmapimage.EndInit();

        //        return bitmapimage;
        //    }
        //}

        //[ObservableProperty]
        //public List<String>? allWindows = null;
        //[ObservableProperty]
        //public List<EmulatorWindow> emulatorWindows = null;


    }
}

