using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class EditShiftDetailsPage : ContentPage
    {
        EditShiftDetailsViewModel editShiftDetailsVM = new EditShiftDetailsViewModel();

        public EditShiftDetailsPage(int instruction, ShiftTable currentShift)
        {
            InitializeComponent();
            editShiftDetailsVM.Navigation = Navigation;
            BindingContext = editShiftDetailsVM;
            editShiftDetailsVM.Load(instruction, currentShift);
            picker.SelectedIndexChanged += Picker_SelectedIndexChanged;

            //Load details for Breaks
            if(instruction==1)
            {

            }
            //Load details for Notes
            else if (instruction==2)
            {

            }
            //Load details for Vehicles
            else if (instruction==3)
            {
                List<VehicleInUseTable> usedVehicles = new List<VehicleInUseTable>();
                usedVehicles = editShiftDetailsVM.LoadVehicles();
                foreach(VehicleInUseTable vehicle in usedVehicles)
                {
                    VehicleTable vehicleInfo = editShiftDetailsVM.LoadVehicleInfo(vehicle);
                    picker.Items.Add(vehicleInfo.Registration + " - HuboStart: " + vehicle.HuboStart + "- HuboEnd: " + vehicle.HuboEnd);
                }
                picker.Title = "Select a vehicle";
            }
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            editShiftDetailsVM.DisplayDetails(picker.SelectedIndex);
        }
    }
}
