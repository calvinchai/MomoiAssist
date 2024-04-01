using MomoiAssist.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui;

namespace MomoiAssist.Services
{
    public class LocalizationService
    {
        private readonly IServiceProvider _serviceProvider;

        private string locale { get; set; } = Settings.Default.UserLanguage;

        public LocalizationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
            Console.WriteLine($"User language: {locale}");
            if (string.IsNullOrEmpty(locale))
            {
                SetSystemLanguage();
            }
            else
            {
                SetLanguage(locale);
            }

        }
            
        private bool IsSupportedLanguage(string language)
        {
            return language == "en" || language == "ja-JP" || language == "zh-CN";
        }

        private string GetSystemLanguage()
        {
            return System.Globalization.CultureInfo.CurrentCulture.Name;
        }

        private void SetSystemLanguage()
        {
            if (IsSupportedLanguage(GetSystemLanguage()))
            {
                SetLanguage(GetSystemLanguage());
            }
            else
            {
                SetLanguage("en");
            }
        }

        public void SetLanguage(string language)
        {
            if (!IsSupportedLanguage(language))
            {
                throw new NotSupportedException($"Language {language} is not supported.");
            }
            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = new CultureInfo(language);

            Settings.Default.UserLanguage = language;
            Settings.Default.Save();
        }

        public string GetLanguage()
        {
            return locale;
        }

    }
}
