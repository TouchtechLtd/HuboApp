﻿using System;
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
            switchToggle.Toggled += SwitchToggle_Toggled;
        }

        public EditVehiclePage(VehicleTable chosenVehicle)
        {
            InitializeComponent();
            BindingContext = vehiclesVM;
            this.currentVehicle = chosenVehicle;
            vehiclesVM.UpdateEditPage(chosenVehicle.Registration);
            switchToggle.Toggled += SwitchToggle_Toggled;
            MessagingCenter.Subscribe<string>("UpdateVehicles", "UpdateVehicles", (sender) =>
            {
                Navigation.PopAsync();
            });

        }
        private void SwitchToggle_Toggled(object sender, ToggledEventArgs e)
        {
            vehiclesVM.ToggleSwitch(e.Value);
        }
    }
}
