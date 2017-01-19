﻿using Hubo.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class NZTAMessageViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public string NZTADisclaimer { get; set; }
        public string NZTAButtonText { get; set; }
        public ICommand NZTAButton { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public NZTAMessageViewModel(int instruction)
        {
            if (instruction == 1)
            {
                NZTAButtonText = Resource.NZTAButtonText;
                NZTADisclaimer = Resource.NZTADisclaimer;
                NZTAButton = new Command(ProceedToHomePage);
            }

            else if (instruction == 2)
            {
                NZTADisclaimer = Resource.EndShiftDisclaimer;
                NZTAButtonText = Resource.Continue;
                NZTAButton = new Command(PopPage);
            }
        }

        private void PopPage()
        {
            Navigation.PopModalAsync();
        }

        private void ProceedToHomePage()
        {
            if (Settings.HamburgerSettings == false)
            {
                BottomNavBar navBar = new BottomNavBar();

                BottomBarPage bottomBarPage = navBar.GetBottomBar();

                NavigationPage.SetHasNavigationBar(bottomBarPage, false);
                Application.Current.MainPage = new NavigationPage(bottomBarPage);
            }
            else
            {
                Application.Current.MainPage = new RootPage();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
