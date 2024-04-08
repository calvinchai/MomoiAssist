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
using System.Windows.Threading;

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

        private void UpdateWindow(object sender, EventArgs e)
        {

            Top = ViewModel.Top;
            Left = ViewModel.Left;

            Topmost = true;
        }
    }
}
