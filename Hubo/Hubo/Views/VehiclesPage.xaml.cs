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

        public VehiclesPage()
        {
            InitializeComponent();
            vehiclesVM.Navigation = Navigation;
            BindingContext = vehiclesVM;
            switchToggle.Toggled += SwitchToggle_Toggled;
            vehicleNames = new List<string>();
            vehicleNames = vehiclesVM.listOfVehicleRegistrations;
            autocomplete.AutoCompleteSource = vehicleNames;
            autocomplete.ShowSuggestionsOnFocus = true;
            autocomplete.Watermark = "Enter Vehicle";
            autocomplete.AutoCompleteMode = Syncfusion.SfAutoComplete.XForms.AutoCompleteMode.SuggestAppend;
            autocomplete.ValueChanged += Autocomplete_ValueChanged;
        }

        private void Autocomplete_ValueChanged(object sender, Syncfusion.SfAutoComplete.XForms.ValueChangedEventArgs e)
        {
            if(e.Value.Length == 6)
            {
                foreach(string vehicle in vehicleNames)
                {
                    if(vehicle==e.Value)
                    {
                        vehiclesVM.UpdatePage(e.Value);
                    }
                }
            }
        }

        private void SwitchToggle_Toggled(object sender, ToggledEventArgs e)
        {
            vehiclesVM.ToggleSwitch(e.Value);
        }
    }
}
