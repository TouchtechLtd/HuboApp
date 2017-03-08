// <copyright file="HomePage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Collections.Generic;
    using Xamarin.Forms;

    public partial class HomePage : ContentPage
    {
        private static Countdown countdown = new Countdown();

        private readonly HomeViewModel homeVM = new HomeViewModel();

        private List<VehicleTable> vehicles = new List<VehicleTable>();
        private DatabaseService dbService = new DatabaseService();

        public HomePage()
        {
            InitializeComponent();
            breakGauge.BindingContext = countdown;

            foreach (var child in grid.Children)
            {
                if (!(child.BindingContext == countdown))
                {
                    child.BindingContext = homeVM;
                }
            }

            homeVM.Navigation = Navigation;
            BackgroundColor = Color.FromHex("#FCFFF5");
            Title = Resource.Hubo;
            UpdateList();

            //breakGauge.PropertyChanged += (sender, args) => Application.Current.MainPage.DisplayAlert("test", args.PropertyName, "ok");
        }

        public void UpdateList()
        {
            vehicles = homeVM.GetVehicles();
        }
    }
}
