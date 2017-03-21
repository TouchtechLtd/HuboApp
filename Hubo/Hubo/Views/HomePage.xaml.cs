// <copyright file="HomePage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Collections.Generic;
    using Xamarin.Forms;
    

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
        }

        public void UpdateList()
        {
            vehicles = homeVM.GetVehicles();
        }
    }
}
