using System.ComponentModel;
using System.Diagnostics;
using MomoiAssist.Helpers;
using MomoiAssist.Models;
using MomoiAssist.Properties;
using MomoiAssist.Services;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace MomoiAssist.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _duration = "Duration";

        [RelayCommand]
        private void OnCounterIncrement() {
            Task.Run(UpdateWindow);
            //Task.Run(UpdateDurationDisplay);

        }
        public DashboardViewModel(LocalizationService localizationService, WindowFinderService windowFinderService, ISnackbarService snackbarService)
        {
            _localizationService = localizationService;
            _windowFinderService = windowFinderService;
            _snackbarService = snackbarService;
            
        }

        private ISnackbarService _snackbarService;
        private LocalizationService _localizationService;
        private WindowFinderService _windowFinderService;

        public async void UpdateDurationDisplay() {
            
            ImageRecognizer imageRecognizer = new ImageRecognizer();

            while (true) {
                String text = await imageRecognizer.RecognizeTextAsync();

                if (ImageRecognizer.IsValidDurationFormat(text))
                    Duration = text;

                //Trace.WriteLine(text);
            }

        }

        private void UpdateWindow()
        {
            EmulatorWindows = _windowFinderService.GetPossibleEmulatorWindows();
            AllWindows = _windowFinderService.AllWindowNames;

            foreach (EmulatorWindow emulatorWindow in EmulatorWindows)
            {
                Console.WriteLine(emulatorWindow.Title);
            }
        }

        [ObservableProperty]
        public List<String> allWindows = null;
        [ObservableProperty]
        public List<EmulatorWindow> emulatorWindows = new List<EmulatorWindow>();
    }
}

