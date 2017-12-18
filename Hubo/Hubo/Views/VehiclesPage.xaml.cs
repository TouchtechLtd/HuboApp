// <copyright file="VehiclesPage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using Xamarin.Forms;

    public partial class VehiclesPage : ContentPage
    {
        private readonly VehiclesViewModel vehiclesVM = new VehiclesViewModel();
        private List<VehicleTable> vehicles = new List<VehicleTable>();
        private List<CompanyTable> companies = new List<CompanyTable>();

        public VehiclesPage()
        {
            InitializeComponent();
            vehiclesVM.Navigation = Navigation;
            BindingContext = vehiclesVM;
            UpdateList();
            Title = Resource.VehiclesText;
            vehiclePicker.SelectedIndexChanged += VehiclePicker_SelectedIndexChanged;
            vehiclePicker.Title = Resource.SelectAVehicle;

            MessagingCenter.Subscribe<string>("UpdateVehicles", "UpdateVehicles", (s) =>
            {
                UpdateList();
                MessagingCenter.Unsubscribe<string>("UpdateVehicles", "UpdateVehicles");
            });
        }

        internal void AddToolBar()
        {
            ToolbarItem topLeftText = new ToolbarItem()
            {
                Text = "Vehicle"
            };
            ToolbarItems.Add(topLeftText);
        }

        private void UpdateList()
        {
            vehicles = vehiclesVM.GetVehicles();

            vehiclePicker.Items.Clear();
            foreach (VehicleTable vehicle in vehicles)
            {
                vehiclePicker.Items.Add(vehicle.Registration + " - " + vehicle.MakeModel);
            }
        }

        private void VehiclePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            vehiclesVM.UpdatePage(vehiclePicker.SelectedIndex);
        }
    }
}
