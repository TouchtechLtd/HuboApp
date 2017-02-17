using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Hubo.Helpers;
using Acr.UserDialogs;
using MvvmCross.Platform;

namespace Hubo
{
    public partial class Application : Xamarin.Forms.Application
    {
        DatabaseService DbService;

        public Application()
        {
            InitializeComponent();
            //Check for logged in status
            //CheckLoggedInStatus();

            //Run a scheduled task every minute
            Device.StartTimer(TimeSpan.FromMinutes(1), () =>
            {
                return ScheduledTasks.testTask();
            });
        }

        private void CheckLoggedInStatus()
        {
            DbService = new DatabaseService();
            if (DbService.CheckLoggedIn())
            {
                MainPage = new NavigationPage(new NZTAMessagePage(1));
            }
            else
            {
                Application.Current.MainPage = new NavigationPage(new LandingPage());
            }
        }

        protected override void OnStart()
        {
            //Handle when your app starts
            Application.Current.MainPage = new NavigationPage(new LandingPage());
            //CheckLoggedInStatus();
        }

        protected override void OnSleep()
        {
            //Handle when your app sleeps
            //MessagingCenter.Unsubscribe<string>("AddBreak", "AddBreak");
        }

        protected override void OnResume()
        {
            //Implement check for logged in status
                MainPage = new NavigationPage(new NZTAMessagePage(1));
            // CheckLoggedInStatus();
        }
    }
}

