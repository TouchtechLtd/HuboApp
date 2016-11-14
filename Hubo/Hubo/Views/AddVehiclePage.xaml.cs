using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class AddVehiclePage : ContentPage
    {
        VehiclesViewModel vehiclesVM = new VehiclesViewModel();
        public AddVehiclePage()
        {
            InitializeComponent();
            BindingContext = vehiclesVM;
            registration.Completed += Registration_Completed;
            make.Completed += Make_Completed;
            model.Completed += Model_Completed;
            company.Completed += Company_Completed;
        }

        private void Company_Completed(object sender, EventArgs e)
        {
            vehiclesVM.InsertVehicle();
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
