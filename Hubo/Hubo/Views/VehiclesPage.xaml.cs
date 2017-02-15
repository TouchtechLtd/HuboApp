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
        VehiclesViewModel vehiclesVM;
        List<VehicleTable> vehicles = new List<VehicleTable>();
        List<CompanyTable> companies = new List<CompanyTable>();



        public VehiclesPage(int instruction)
        {
            InitializeComponent();
            vehiclesVM = new VehiclesViewModel(instruction);
            vehiclesVM.Navigation = Navigation;
            BindingContext = vehiclesVM;
            UpdateList();
            Title = Resource.VehiclesText;
            companyPicker.SelectedIndexChanged += CompanyPicker_SelectedIndexChanged;
            vehiclePicker.SelectedIndexChanged += VehiclePicker_SelectedIndexChanged;
            vehiclePicker.Title = Resource.SelectAVehicle;
            MessagingCenter.Subscribe<string, int>("UpdateVehicles", "UpdateVehicles", (sender, vehicleID) =>
            {
                UpdateList();
                vehiclePicker.SelectedIndex = vehicleID - 1;
            });

            registration.ReturnType = ReturnType.Next;
            registration.Next = makeModel;

            makeModel.ReturnType = ReturnType.Next;
            makeModel.Next = fleet;

            fleet.ReturnType = ReturnType.Done;

            Device.OnPlatform(iOS: () => Grid.SetRow(activityLabel, 1));
            Device.OnPlatform(iOS: () => Grid.SetRowSpan(activityLabel, 7));
        }

        private void CompanyPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (companyPicker.SelectedIndex != -1)
            {
                vehiclesVM.SelectedCompany = companyPicker.SelectedIndex;
            }
        }

        private void UpdateList()
        {
            vehicles = vehiclesVM.GetVehicles();
            companies = vehiclesVM.GetCompanies();

            vehiclePicker.Items.Clear();
            foreach (VehicleTable vehicle in vehicles)
            {
                vehiclePicker.Items.Add(vehicle.Registration);
            }

            vehiclePicker.Items.Add("Add Vehicle...");

            companyPicker.Items.Clear();
            foreach (CompanyTable item in companies)
            {
                companyPicker.Items.Add(item.Name);
            }
        }

        private void VehiclePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (vehiclePicker.SelectedIndex != -1 && vehiclePicker.SelectedIndex < vehiclePicker.Items.Count - 1)
            {
                int id = vehiclesVM.UpdatePage(vehiclePicker.SelectedIndex);

                companyPicker.SelectedIndex = id - 1;
            }
            else
            {
                vehiclesVM.UpdatePageAdd();

                companyPicker.SelectedIndex = 0;
            }
        }

        internal void AddToolBar()
        {
            ToolbarItem topLeftText = new ToolbarItem();
            topLeftText.Text = "Vehicle";
            ToolbarItems.Add(topLeftText);
        }
    }
}
