﻿// <copyright file="SettingsPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Hubo
{
    using Xamarin.Forms;

    public partial class SettingsPage : ContentPage
    {
        private readonly SettingsViewModel settingsVM = new SettingsViewModel();

        public SettingsPage()
        {
            InitializeComponent();
            MessagingCenter.Send<string>("Remove_Settings", "Remove_Settings");
            BindingContext = settingsVM;
            Title = Resource.SettingsText;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Send<string>("Reset_Settings", "Reset_Settings");
            base.OnDisappearing();
        }
    }
}
