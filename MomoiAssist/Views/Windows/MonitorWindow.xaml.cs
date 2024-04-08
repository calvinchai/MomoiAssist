using MomoiAssist.Services;
using MomoiAssist.ViewModels.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui;
using MomoiAssist;

namespace MomoiAssist.Views.Windows
{
    /// <summary>
    /// Interaction logic for MonitorWindow.xaml
    /// </summary>
    public partial class MonitorWindow : Window
    {
        public MonitorWindowViewModel ViewModel { get;}
        public MonitorWindow(MonitorWindowViewModel monitorWindowViewModel)
        {
            ViewModel = monitorWindowViewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
