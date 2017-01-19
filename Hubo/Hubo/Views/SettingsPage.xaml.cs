using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class SettingsPage : ContentPage
    {
        SettingsViewModel settingsVM = new SettingsViewModel();
        public SettingsPage()
        {
            InitializeComponent();
            MessagingCenter.Send<string>("Remove_Settings", "Remove_Settings");
            BindingContext = settingsVM;
            Title = Resource.SettingsText;

            hamburgerSwitch.OnChanged += HamburgerSwitch_OnChanged;
            
            
        }

        private void HamburgerSwitch_OnChanged(object sender, ToggledEventArgs e)
        {
            settingsVM.OnPropertyChanged("HamburgerSettings");
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
