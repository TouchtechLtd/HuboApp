// <copyright file="HomePage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Syncfusion.SfGauge.XForms;
    using Xamarin.Forms;

    public partial class HomePage : ContentPage
    {
        private readonly HomeViewModel homeVM = new HomeViewModel();

        private List<VehicleTable> vehicles = new List<VehicleTable>();
        private DatabaseService dbService = new DatabaseService();

        public HomePage()
        {
            InitializeComponent();
            BindingContext = homeVM;
            homeVM.Navigation = Navigation;
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
