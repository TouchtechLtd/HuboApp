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
            Title = Resource.AddVehicleText;

            registration.ReturnType = ReturnType.Next;
            registration.Next = make;

            make.ReturnType = ReturnType.Next;
            make.Next = model;

            model.ReturnType = ReturnType.Next;
            model.Next = company;

            company.ReturnType = ReturnType.Next;
            company.Next = hubo;

            hubo.ReturnType = ReturnType.Done;
            hubo.Completed += Hubo_Completed;
        }

        private void Hubo_Completed(object sender, EventArgs e)
        {
            vehiclesVM.InsertVehicle();
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
