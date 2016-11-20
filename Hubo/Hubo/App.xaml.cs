﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hubo
{
    public partial class Application : Xamarin.Forms.Application
    {
        DatabaseService dbService;

        public Application()
        {
            InitializeComponent();
            //MainPage = new NavigationPage(new HomePage());

            //TODO: Implement check for logged in status
            CheckLoggedInStatus();


            //TODO run a scheduled task every minute
            Device.StartTimer(TimeSpan.FromMinutes(1), () => {
                return ScheduledTasks.testTask();
            });
        }

        private void CheckLoggedInStatus()
        {
            dbService = new DatabaseService();
            if (dbService.CheckLoggedIn())
            {
                MainPage = new NZTAMessagePage(1);
            }
            else
            {
                MainPage = new NavigationPage(new LandingPage());
            }
        }

        protected override void OnStart()
        {
            //TODO Handle when your app starts
            CheckLoggedInStatus();
        }

        protected override void OnSleep()
        {
            //TODO Handle when your app sleeps
            MessagingCenter.Unsubscribe<string>("AddBreak", "AddBreak");
            

        }

        protected override void OnResume()
        {
            //TODO: Implement check for logged in status
            //CheckLoggedInStatus();
        }
    }
}

