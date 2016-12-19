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
        }

        protected override void OnAppearing()
        {
            //MessagingCenter.Send<string>("Remove_Settings", "Remove_Settings");

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Send<string>("Reset_Settings", "Reset_Settings");

            base.OnDisappearing();
        }
    }
}
