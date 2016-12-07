using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class AddShiftPage : ContentPage
    {
        AddShiftViewModel addShiftVM = new AddShiftViewModel();
        public AddShiftPage()
        {
            InitializeComponent();
            addShiftVM.Navigation = Navigation;
            BindingContext = addShiftVM;
            saveButton.Clicked += SaveButton_Clicked;
            addButton.Clicked += AddButton_Clicked;
            UpdateVehicleItems();
            Title = Resource.AddShiftText;
        }

        private void UpdateVehicleItems()
        {
            List<VehicleTable> vehiclePickerItems = new List<VehicleTable>();
            vehiclePickerItems = addShiftVM.GetVehicles();
            if(vehiclePickerItems!=null)
            {
                foreach(VehicleTable vehicle in vehiclePickerItems)
                {
                    vehiclePicker.Items.Add(vehicle.Registration);
                }
            }
            
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            //if(startBreakText1.IsVisible)
            //{
            //    if(startBreakText2.IsVisible)
            //    {
            //        DisplayAlert("Alert", "Sorry, ability to add more than 2 will be implemented soon", "Ok");
            //    }
            //    else
            //    {
            //        startBreakText2.IsVisible = true;
            //        startBreakPicker2.IsVisible = true;
            //        endBreakPicker2.IsVisible = true;
            //        endBreakText2.IsVisible = true;
            //    }
            //}
            //else
            //{
            //    startBreakText1.IsVisible = true;
            //    startBreakPicker1.IsVisible = true;
            //    EndBreakText1.IsVisible = true;
            //    endBreakPicker1.IsVisible = true;
            //}

            var action = await DisplayActionSheet("Add Break or Note?", "Cancel", null, "Break", "Note");

            if (action != null && action != "Cancel")
            {
                await Navigation.PushModalAsync(new NavigationPage(new AddManBreakNotePage(action)));
            }
;        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}
