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
        List<VehicleTable> vehicles = new List<VehicleTable>();

        public VehiclesPage(int instruction)
        {
            InitializeComponent();
            vehiclesVM.Navigation = Navigation;
            BindingContext = vehiclesVM;
            UpdateList();
            Title = Resource.VehiclesText;
            vehiclePicker.SelectedIndexChanged += VehiclePicker_SelectedIndexChanged;
            vehiclePicker.Title = Resource.SelectAVehicle;
            MessagingCenter.Subscribe<string>("UpdateVehicles", "UpdateVehicles", (sender) =>
            {
                UpdateList();
            });            
            vehiclesVM.Load(instruction);
        }

        private void UpdateList()
        {
            vehicles = vehiclesVM.GetVehicles();
            vehiclePicker.Items.Clear();
            foreach (VehicleTable vehicle in vehicles)
            {
                vehiclePicker.Items.Add(vehicle.Registration);
            }
        }

        private void VehiclePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(vehiclePicker.SelectedIndex!=-1)
            {
                vehiclesVM.UpdatePage(vehiclePicker.SelectedIndex);
            }
        }

    }
}
