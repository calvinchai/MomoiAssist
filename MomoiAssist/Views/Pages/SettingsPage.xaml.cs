﻿using MomoiAssist.ViewModels.Pages;
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
