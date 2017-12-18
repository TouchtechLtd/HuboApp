// <copyright file="SettingsPage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
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
            closeButton.Clicked += CloseButton_Clicked;
        }

        private void CloseButton_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
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
