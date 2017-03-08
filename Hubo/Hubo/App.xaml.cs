namespace Hubo
{
    using System;
    using Plugin.Geolocator;
    using Plugin.Geolocator.Abstractions;
    using Xamarin.Forms;

    public partial class Application : Xamarin.Forms.Application
    {
        public static IGeolocator locator = CrossGeolocator.Current;
        DatabaseService DbService = new DatabaseService();

        public Application()
        {
            InitializeComponent();
            //Run a scheduled task every minute
            Device.StartTimer(TimeSpan.FromMinutes(5), () =>
            {
                //ScheduledTasks.CheckOfflineData();
                return true;
            });
        }

        private void CheckLoggedInStatus()
        {
            MainPage = DbService.CheckLoggedIn() ? new NavigationPage(new NZTAMessagePage(1)) : new NavigationPage(new LandingPage());
        }

        protected override void OnStart()
        {
            //Handle when your app starts
            base.OnStart();
            CheckLoggedInStatus();
        }

        protected override void OnSleep()
        {
            //Handle when your app sleeps
            base.OnSleep();
            MessagingCenter.Unsubscribe<string>("AddBreak", "AddBreak");
        }

        protected override void OnResume()
        {
            //Implement check for logged in status
            base.OnResume();

            //if (DbService.CheckLoggedIn())
            //{
            //    MainPage.Navigation.PushModalAsync(new NZTAMessagePage(3));
            //}
        }
    }
}