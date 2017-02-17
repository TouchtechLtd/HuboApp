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

        public EditVehiclePage(VehicleTable chosenVehicle)
        {
            InitializeComponent();
            BindingContext = vehiclesVM;
            this.currentVehicle = chosenVehicle;
            //vehiclesVM.UpdateEditPage(chosenVehicle.Registration);

            registration.ReturnType = ReturnType.Next;
            registration.Next = make;

            make.ReturnType = ReturnType.Next;
            make.Next = model;

            model.ReturnType = ReturnType.Next;
            model.Next = company;

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
