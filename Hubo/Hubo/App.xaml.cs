using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hubo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
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

