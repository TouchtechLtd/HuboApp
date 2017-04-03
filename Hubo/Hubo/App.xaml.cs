// <copyright file="App.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using Plugin.Geolocator;
    using Plugin.Geolocator.Abstractions;

    public partial class Application : Xamarin.Forms.Application
    {
        internal static IGeolocator Locator = CrossGeolocator.Current;
        private DatabaseService dbService = new DatabaseService();

        public Application()
        {
            InitializeComponent();

            if (dbService.CheckLoggedIn())
            {
                MainPage = new NZTAMessagePage(1);
            }
            else
            {
                MainPage = new LandingPage();
            }

            // MainPage = new EndShiftConfirmPage();

            // Run a scheduled task every minute
            // Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            // {
            //    ScheduledTasks.CheckOfflineData();
            //    return false;
            // });
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Implement check for logged in status
        }
    }
}