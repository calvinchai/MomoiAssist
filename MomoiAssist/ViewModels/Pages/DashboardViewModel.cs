using System.ComponentModel;
using System.Diagnostics;

namespace MomoiAssist.ViewModels.Pages {
    public partial class DashboardViewModel : ObservableObject {

        [ObservableProperty]
        private string _duration = "Duration";

        [RelayCommand]
        private void OnCounterIncrement() {
            Task.Run(UpdateDurationDisplay);
        }

        public async void UpdateDurationDisplay() {
            ImageRecognizer imageRecognizer = new ImageRecognizer();

            while (true) {
                String text = await imageRecognizer.RecognizeTextAsync();

                if (ImageRecognizer.IsValidDurationFormat(text))
                    Duration = text;

                //Trace.WriteLine(text);
            }

        }
    }
}

