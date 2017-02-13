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
        public int Instruction;

        public VehicleChecklistPage(int instruction, bool fromEndShift, int key = 0)
        {
            InitializeComponent();
            vehicleCheckListVM = new VehicleChecklistViewModel(instruction);
            vehicleCheckListVM.Navigation = Navigation;
            BindingContext = vehicleCheckListVM;
            vehicleCheckListVM.CurrentVehicleKey = key;
            canGoBack = fromEndShift;
            Instruction = instruction;

            huboEntry.ReturnType = ReturnType.Next;
            huboEntry.Next = locationEntry;

            locationEntry.ReturnType = ReturnType.Go;
            locationEntry.Completed += LocationEntry_Completed;
        }

        private void LocationEntry_Completed(object sender, EventArgs e)
        {
            vehicleCheckListVM.ContinueCommand.Execute(null);
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
