using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hubo;
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
            addShiftVM.FullGrid = grid;

            startLocation.ReturnType = ReturnType.Next;
            startLocation.Next = endLocation;

            endLocation.ReturnType = ReturnType.Next;
            endLocation.Next = startHubo;

            startHubo.ReturnType = ReturnType.Next;
            startHubo.Next = endHubo;

            endHubo.ReturnType = ReturnType.Done;
            endHubo.Completed += SaveButton_Clicked;
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
            var action = await DisplayActionSheet(Resource.AddBreakNote, Resource.Cancel, null, Resource.Break, Resource.NoteText);

            if (action != null && action != "Cancel")
            {
                addShiftVM.Add = action;
                await Navigation.PushModalAsync(new NavigationPage(new AddManBreakNotePage(action)));
            }
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (vehiclePicker.SelectedIndex != -1)
            {
                addShiftVM.selectedVehicle = vehiclePicker.SelectedIndex;
            }
        }
    }
}
