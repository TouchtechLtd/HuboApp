using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class VehiclesPage : ContentPage
    {
        VehiclesViewModel vehiclesVM = new VehiclesViewModel();
        List<string> vehicleNames;

        public VehiclesPage(int instruction)
        {
            //We are accessing vehicle page through home menu 
            InitializeComponent();
            vehiclesVM.Navigation = Navigation;
            BindingContext = vehiclesVM;
            //switchToggle.Toggled += SwitchToggle_Toggled;
            vehicleNames = new List<string>();
            vehicleNames = vehiclesVM.listOfVehicleRegistrations;
            autocomplete.AutoCompleteSource = vehicleNames;
            autocomplete.ShowSuggestionsOnFocus = true;
            autocomplete.Watermark = "Enter Vehicle";
            autocomplete.AutoCompleteMode = Syncfusion.SfAutoComplete.XForms.AutoCompleteMode.SuggestAppend;
            autocomplete.ValueChanged += Autocomplete_ValueChanged;
            MessagingCenter.Subscribe<string>("UpdateVehicles", "UpdateVehicles", (sender) =>
            {
                vehicleNames = vehiclesVM.GetVehicles();
                autocomplete.AutoCompleteSource = vehicleNames;
            });            
            vehiclesVM.Load(instruction);
        }

        private void Autocomplete_ValueChanged(object sender, Syncfusion.SfAutoComplete.XForms.ValueChangedEventArgs e)
        {
            foreach(string vehicle in vehicleNames)
            {
                if(vehicle==e.Value)
                {
                    vehiclesVM.UpdatePage(e.Value);
                }
            }
        }

        private void SwitchToggle_Toggled(object sender, ToggledEventArgs e)
        {
            vehiclesVM.ToggleSwitch(e.Value);
        }

        protected override void OnDisappearing()
        {
            //MessagingCenter.Unsubscribe<string>("UpdateVehicles", "UpdateVehicles");
            base.OnDisappearing();
        }
    }
}
