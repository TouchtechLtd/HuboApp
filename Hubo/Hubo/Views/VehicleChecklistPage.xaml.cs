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
        VehicleChecklistViewModel vehicleCheckListVM;
        public bool canGoBack;
        public VehicleChecklistPage(int instruction, bool fromEndShift, int key=0)
        {
            InitializeComponent();
            vehicleCheckListVM = new VehicleChecklistViewModel(instruction);
            vehicleCheckListVM.Navigation = Navigation;
            BindingContext = vehicleCheckListVM;
            vehicleCheckListVM.CurrentVehicleKey = key;
            canGoBack = fromEndShift;
            //MessagingCenter.Subscribe<string>("UpdateVehicleInUse", "UpdateVehicleInUse", (sender) =>
            //{
            //    Navigation.PopAsync();
            //});
        }
        protected override bool OnBackButtonPressed()
        {
            if(canGoBack)
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
