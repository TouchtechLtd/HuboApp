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
        public bool CanGoBack;
        public VehicleChecklistPage(int instruction, bool fromEndShift, int key=0)
        {
            InitializeComponent();
            vehicleCheckListVM.Navigation = Navigation;
            BindingContext = vehicleCheckListVM;
            vehicleCheckListVM.CurrentVehicleKey = key;
            vehicleCheckListVM.Load(instruction);
            CanGoBack = fromEndShift;
            //MessagingCenter.Subscribe<string>("UpdateVehicleInUse", "UpdateVehicleInUse", (sender) =>
            //{
            //    Navigation.PopAsync();
            //});
        }
        protected override bool OnBackButtonPressed()
        {
            if(CanGoBack)
            {
                return base.OnBackButtonPressed();
            }
            else
            {
                MessagingCenter.Unsubscribe<string>("EndShiftRegoEntered", "EndShiftRegoEntered");
                return base.OnBackButtonPressed();
            }
        }
    }
}
