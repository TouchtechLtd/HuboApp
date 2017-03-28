// <copyright file="NZTAMessageViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.ComponentModel;
    using System.Windows.Input;
    using Xamarin.Forms;

    internal class NZTAMessageViewModel : INotifyPropertyChanged
    {
        private BottomNavBar navBar = new BottomNavBar();

        private BottomBarPage bottomBarPage;

        public NZTAMessageViewModel(int instruction)
        {
            if (instruction == 1)
            {
                NZTAButtonText = Resource.NZTAButtonText;
                NZTADisclaimer = Resource.NZTADisclaimer;
                NZTAButton = new Command(ProceedToHomePage);
                this.bottomBarPage = navBar.GetBottomBar();
                NavigationPage.SetHasNavigationBar(bottomBarPage, false);
            }
            else if (instruction == 2)
            {
                NZTADisclaimer = Resource.EndShiftDisclaimer;
                NZTAButtonText = Resource.Continue;
                NZTAButton = new Command(PopPage);
            }
            else if (instruction == 3)
            {
                NZTAButtonText = Resource.NZTAButtonText;
                NZTADisclaimer = Resource.NZTADisclaimer;
                NZTAButton = new Command(PopPage);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public INavigation Navigation { get; set; }

        public string NZTADisclaimer { get; set; }

        public string NZTAButtonText { get; set; }

        public ICommand NZTAButton { get; set; }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void PopPage()
        {
            Navigation.PopModalAsync();
        }

        private void ProceedToHomePage()
        {
            Application.Current.MainPage = new NavigationPage(bottomBarPage);
        }
    }
}
