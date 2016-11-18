using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class VehicleChecklistPage : ContentPage
    {
        VehicleChecklistViewModel vehicleCheckListVM = new VehicleChecklistViewModel();
        public VehicleChecklistPage(int instruction, int key=0)
        {
            InitializeComponent();
            vehicleCheckListVM.Navigation = Navigation;
            BindingContext = vehicleCheckListVM;
            vehicleCheckListVM.CurrentVehicleKey = key;
            vehicleCheckListVM.Load(instruction);
            //MessagingCenter.Subscribe<string>("UpdateVehicleInUse", "UpdateVehicleInUse", (sender) =>
            //{
            //    Navigation.PopAsync();
            //});
        }
    }
}
