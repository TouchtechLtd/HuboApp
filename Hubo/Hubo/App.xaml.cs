// <copyright file="App.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using Plugin.Geolocator;
    using Plugin.Geolocator.Abstractions;
    using Xamarin.Forms;

    public partial class Application : Xamarin.Forms.Application
    {
        internal static IGeolocator Locator = CrossGeolocator.Current;
        private DatabaseService dbService = new DatabaseService();

        public Application()
        {
            InitializeComponent();

            // Run a scheduled task every minute
            Device.StartTimer(TimeSpan.FromMinutes(5), () =>
            {
                // ScheduledTasks.CheckOfflineData();
                return true;
            });
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            base.OnStart();
            CheckLoggedInStatus();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            base.OnSleep();
            MessagingCenter.Unsubscribe<string>("AddBreak", "AddBreak");
        }

        protected override void OnResume()
        {
            // Implement check for logged in status
            base.OnResume();
        }

        private void CheckLoggedInStatus()
        {
            if (dbService.CheckLoggedIn())
            {
                MainPage = new NZTAMessagePage(1);
            }
            else
            {
                MainPage = new LandingPage();
            }
        }
    }
}