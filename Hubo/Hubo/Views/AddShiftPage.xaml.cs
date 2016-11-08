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
            addBreakButton.Clicked += AddBreakButton_Clicked;
            UpdateVehicleItems();
        }

        private void UpdateVehicleItems()
        {
            List<string> vehiclePickerItems = new List<string>();
            vehiclePickerItems = addShiftVM.vehicles;
            foreach(string vehicle in vehiclePickerItems)
            {
                vehiclePicker.Items.Add(vehicle);
            }
        }

        private void AddBreakButton_Clicked(object sender, EventArgs e)
        {
            if(startBreakText1.IsVisible)
            {
                if(startBreakText2.IsVisible)
                {
                    DisplayAlert("Alert", "Sorry, ability to add more than 2 will be implemented soon", "Ok");
                }
                else
                {
                    startBreakText2.IsVisible = true;
                    startBreakPicker2.IsVisible = true;
                    endBreakPicker2.IsVisible = true;
                    endBreakText2.IsVisible = true;
                }
            }
            else
            {
                startBreakText1.IsVisible = true;
                startBreakPicker1.IsVisible = true;
                EndBreakText1.IsVisible = true;
                endBreakPicker1.IsVisible = true;
            }
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}
