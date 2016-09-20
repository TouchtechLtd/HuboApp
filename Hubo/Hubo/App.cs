using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hubo.Services;
using Hubo.View;
using Hubo.ViewModel;
using Xamarin.Forms;

namespace Hubo
{
    public class App : Application
    {
        AppViewModel appVM = new AppViewModel();

        public App()
        {
            DatabaseService.initDb();
            MainPage = new NavigationPage(new HomePage());
        }

        protected override void OnStart()
        {
            //TODO Handle when your app starts
        }

        protected override void OnSleep()
        {
            //TODO Handle when your app sleeps
        }

        protected override void OnResume()
        {
            //TODO Handle when your app resumes
        }
    }
}
