using Plugin.Geolocator;
using System;
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

            BottomBarPage bottomBarPage = new BottomBarPage();

            bottomBarPage.BarTextColor = Color.Black;
            bottomBarPage.FixedMode = true;

            //TODO: add the tabs to the menubar
            MenuViewModel menuVM = new MenuViewModel();

            foreach (MenuItem item in menuVM.MenuPageList)
            {
                switch (item.Title)
                {
                    case "Home":
                        HomePage home = new HomePage();

                        FileImageSource homeIcon = (FileImageSource)FileImageSource.FromFile(string.Format(item.ImageSource, item.Title.ToLowerInvariant()));

                        home.Title = item.Title;
                        home.Icon = homeIcon;

                        bottomBarPage.Children.Add(home);
                        break;
                    //case "Profile":
                    //    ProfilePage profile = new ProfilePage();

                    //    FileImageSource profileIcon = (FileImageSource)FileImageSource.FromFile(string.Format(item.ImageSource, item.Title.ToLowerInvariant()));

                    //    profile.Title = item.Title;
                    //    profile.Icon = profileIcon;

                    //    bottomBarPage.Children.Add(profile);
                    //    break;
                    case "Vehicles":
                        EditVehiclePage vehicle = new EditVehiclePage();

                        FileImageSource vehicleIcon = (FileImageSource)FileImageSource.FromFile(string.Format(item.ImageSource, item.Title.ToLowerInvariant()));

                        vehicle.Title = item.Title;
                        vehicle.Icon = vehicleIcon;

                        bottomBarPage.Children.Add(vehicle);
                        break;
                    case "History":
                        HistoryPage history = new HistoryPage();

                        FileImageSource historyIcon = (FileImageSource)FileImageSource.FromFile(string.Format(item.ImageSource, item.Title.ToLowerInvariant()));

                        history.Title = item.Title;
                        history.Icon = historyIcon;

                        bottomBarPage.Children.Add(history);
                        break;
                    case "Add Shift":
                        AddShiftPage addShift = new AddShiftPage();

                        FileImageSource addShiftIcon = (FileImageSource)FileImageSource.FromFile(string.Format(item.ImageSource, item.Title.ToLowerInvariant()));

                        addShift.Title = item.Title;
                        addShift.Icon = addShiftIcon;

                        bottomBarPage.Children.Add(addShift);
                        break;
                }
            }

            OthersPage other = new OthersPage();
            bottomBarPage.Children.Add(other);

            NavigationPage.SetHasNavigationBar(bottomBarPage, false);
            MainPage = new NavigationPage(bottomBarPage);

            //TODO: Implement check for logged in status
            //CheckLoggedInStatus();

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
                MainPage = new NavigationPage(new NZTAMessagePage(1));
            }
            else
            {
                MainPage = new NavigationPage(new LandingPage());
            }
        }

        //public void SetRootPage()
        //{
        //    BottomBarPage bottomBarPage = new BottomBarPage();

        //    bottomBarPage.BarTextColor = Color.Pink;
        //    bottomBarPage.FixedMode = true;

        //    //TODO: add the tabs to the menubar
        //    MenuViewModel menuVM = new MenuViewModel();

        //    foreach (MenuItem item in menuVM.MenuPageList)
        //    {
        //        switch (item.Title)
        //        {
        //            case "Home":
        //                HomePage home = new HomePage();

        //                FileImageSource homeIcon = (FileImageSource)FileImageSource.FromFile(string.Format(item.ImageSource, item.Title.ToLowerInvariant()));

        //                home.Title = item.Title;
        //                home.Icon = homeIcon;

        //                bottomBarPage.Children.Add(home);
        //                break;
        //            case "Profile":
        //                ProfilePage profile = new ProfilePage();

        //                FileImageSource profileIcon = (FileImageSource)FileImageSource.FromFile(string.Format(item.ImageSource, item.Title.ToLowerInvariant()));

        //                profile.Title = item.Title;
        //                profile.Icon = profileIcon;

        //                bottomBarPage.Children.Add(profile);
        //                break;
        //            case "Vehicles":
        //                EditVehiclePage vehicle = new EditVehiclePage();

        //                FileImageSource vehicleIcon = (FileImageSource)FileImageSource.FromFile(string.Format(item.ImageSource, item.Title.ToLowerInvariant()));

        //                vehicle.Title = item.Title;
        //                vehicle.Icon = vehicleIcon;

        //                bottomBarPage.Children.Add(vehicle);
        //                break;
        //            case "History":
        //                HistoryPage history = new HistoryPage();

        //                FileImageSource historyIcon = (FileImageSource)FileImageSource.FromFile(string.Format(item.ImageSource, item.Title.ToLowerInvariant()));

        //                history.Title = item.Title;
        //                history.Icon = historyIcon;

        //                bottomBarPage.Children.Add(history);
        //                break;
        //            case "Add Shift":
        //                AddShiftPage addShift = new AddShiftPage();

        //                FileImageSource addShiftIcon = (FileImageSource)FileImageSource.FromFile(string.Format(item.ImageSource, item.Title.ToLowerInvariant()));

        //                addShift.Title = item.Title;
        //                addShift.Icon = addShiftIcon;

        //                bottomBarPage.Children.Add(addShift);
        //                break;
        //            case "Sign Out":

        //                break;
        //        }
        //    }

        //    MainPage = bottomBarPage;
        //}

        protected override void OnStart()
        {
            //TODO Handle when your app starts
            //CheckLoggedInStatus();
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

