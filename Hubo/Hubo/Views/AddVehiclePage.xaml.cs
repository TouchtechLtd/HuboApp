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
            vehiclesVM.Navigation = Navigation;
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

            Device.OnPlatform(iOS: () => Grid.SetRow(activityLabel, 1));
            Device.OnPlatform(iOS: () => Grid.SetRowSpan(activityLabel, 7));
        }

        private void Hubo_Completed(object sender, EventArgs e)
        {
            vehiclesVM.InsertVehicle();
        }
    }
}
