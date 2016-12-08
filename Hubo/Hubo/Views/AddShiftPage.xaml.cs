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
            addShiftVM.BreakGrid = gridBreak;
            addShiftVM.NoteGrid = gridNote;
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
            var action = await DisplayActionSheet("Add Break or Note?", "Cancel", null, "Break", "Note");

            if (action != null && action != "Cancel")
            {
                addShiftVM.Add = action;
                await Navigation.PushModalAsync(new NavigationPage(new AddManBreakNotePage(action)));
            }
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}
