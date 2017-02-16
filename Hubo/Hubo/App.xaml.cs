using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Hubo.Helpers;

namespace Hubo
{
    public partial class Application : Xamarin.Forms.Application
    {
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

            //if (DbService.CheckLoggedIn())
            //{
            //    MainPage = new NavigationPage(new NZTAMessagePage(1));
            //}
            //else
            //{
            //    MainPage = new NavigationPage(new LandingPage());
            //}
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

            //CheckLoggedInStatus();

            if (DbService.CheckLoggedIn())
            {
                MainPage.Navigation.PushModalAsync(new NZTAMessagePage(3));
            }
        }
    }
}

