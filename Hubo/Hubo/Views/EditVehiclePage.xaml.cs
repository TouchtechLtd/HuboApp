using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class EditVehiclePage : ContentPage
    {
        VehicleTable currentVehicle;
        VehiclesViewModel vehiclesVM = new VehiclesViewModel();

        public EditVehiclePage()
        {
            InitializeComponent();
            BindingContext = vehiclesVM;
            Title = Resource.EditVehicleText;
        }

        private void Company_Completed(object sender, EventArgs e)
        {
            vehiclesVM.SaveVehicleDetails();
        }

        private void Model_Completed(object sender, EventArgs e)
        {
            company.Focus();
        }

        private void Make_Completed(object sender, EventArgs e)
        {
            model.Focus();
        }

        private void Registration_Completed(object sender, EventArgs e)
        {
            make.Focus();
        }

        public EditVehiclePage(VehicleTable chosenVehicle)
        {
            InitializeComponent();
            BindingContext = vehiclesVM;
            this.currentVehicle = chosenVehicle;
            vehiclesVM.UpdateEditPage(chosenVehicle.Registration);
            registration.Completed += Registration_Completed;
            make.Completed += Make_Completed;
            model.Completed += Model_Completed;
            company.Completed += Company_Completed;
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>("UpdateVehicles", "UpdateVehicles");
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<string>("UpdateVehicles", "UpdateVehicles", (sender) =>
            {
                Navigation.PopAsync();
            });
            base.OnAppearing();
        }
    }
}
