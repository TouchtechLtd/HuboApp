// <copyright file="HomePage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Collections.Generic;
    using Xamarin.Forms;
    using XFShapeView;

    public partial class HomePage : ContentPage
    {
        private readonly HomeViewModel homeVM = new HomeViewModel();

        private List<VehicleTable> vehicles = new List<VehicleTable>();

        public HomePage()
        {
            InitializeComponent();
            homeVM.Navigation = Navigation;
            BindingContext = homeVM;
            BackgroundColor = Color.FromHex("#FCFFF5");
            Title = Resource.Hubo;
            UpdateList();

            var driveButton = new TapGestureRecognizer();
            driveButton.SetBinding(TapGestureRecognizer.CommandProperty, "VehicleCommand");
            driveLabel.GestureRecognizers.Add(driveButton);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Send<string>("loadComplete", "loadComplete");
        }

        public void UpdateList()
        {
            vehicles = homeVM.GetVehicles();
        }
    }
}
