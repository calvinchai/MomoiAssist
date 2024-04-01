using MomoiAssist.Properties;
using MomoiAssist.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata;
using System.Windows.Data;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using MomoiAssist;

namespace MomoiAssist.ViewModels.Pages
{
    enum Theme
    {
        FollowSystem,
        
        Dark,

        Light,
    }
    
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        public Array Themes
        {
            get
            {
                return Enum.GetValues(typeof(Theme));
            }
        }
        
        public SettingsViewModel(LocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        private LocalizationService _localizationService;
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _appVersion = String.Empty;

        [ObservableProperty]
        private int _currentTheme = Settings.Default.Theme;

        [ObservableProperty]
        private string _userLanguage = String.Empty;

        partial void OnCurrentThemeChanged(int value)
        {
            Console.WriteLine($"Theme changed to {(int)value}");
            if (value == (int)ApplicationTheme.Unknown)
                return;

            ApplicationThemeManager.Apply((ApplicationTheme)value);

            Console.WriteLine($"Theme changed to {value}");
        }
        partial void OnUserLanguageChanged(string value)
        {
            _localizationService.SetLanguage(value);

        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            UserLanguage = _localizationService.GetLanguage();
            CurrentTheme = (int)ApplicationThemeManager.GetAppTheme();
            AppVersion = $"UiDesktopApp1 - {GetAssemblyVersion()}";
            _isInitialized = true;
        }
        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? String.Empty;
        }
        

    }
}
