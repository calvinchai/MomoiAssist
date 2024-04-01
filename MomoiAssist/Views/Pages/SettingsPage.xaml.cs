using MomoiAssist.ViewModels.Pages;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace MomoiAssist.Views.Pages
{
    public partial class SettingsPage : INavigableView<SettingsViewModel>
    {
        public SettingsViewModel ViewModel { get; }

        public SettingsPage(SettingsViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

    
    }
}
