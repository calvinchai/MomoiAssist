using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using Wpf.Ui.Controls;
using WPFLocalizeExtension.Extensions;
using WPFLocalizeExtension.Providers;
namespace MomoiAssist.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {

        [ObservableProperty]
        private string _applicationTitle = "MomoiAssist";

    }
}
