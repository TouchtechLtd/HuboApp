using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using BottomBar.XamarinForms;

namespace Hubo
{
    public partial class RootPageBottomBar
    {
        public RootPageBottomBar()
        {
            //var othersPage = new OthersPage();
            //othersPage.CopyList.ItemSelected += (sender, e) =>
            //{
            //    if (((ListView)sender).SelectedItem == null)
            //    {
            //        return;
            //    }
            //    ((ListView)sender).SelectedItem = null;
            //    NavigateTo(e.SelectedItem as MenuItem);
            //};

            BottomBarPage bottomBarPage = new BottomBarPage();

            bottomBarPage.BarTextColor = Color.Pink;
            bottomBarPage.FixedMode = true;

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
                    case "Profile":
                        ProfilePage profile = new ProfilePage();

                        FileImageSource profileIcon = (FileImageSource)FileImageSource.FromFile(string.Format(item.ImageSource, item.Title.ToLowerInvariant()));

                        profile.Title = item.Title;
                        profile.Icon = profileIcon;

                        bottomBarPage.Children.Add(profile);
                        break;
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
                    case "Sign Out":

                        break;
                }
            }

            Application.Current.MainPage = bottomBarPage;

            //((Application)Application.Current).SetRootPage();
        }

        //async void NavigateTo(MenuItem menu)
        //{
        //    if (menu.TargetType == "Profile")
        //    {
        //        await Navigation.PushModalAsync(new ProfilePage());
        //    }
        //    else if (menu.TargetType == "Settings")
        //    {
        //        await Navigation.PushAsync(new SettingsPage());
        //    }
        //    else if (menu.TargetType == "SignOut")
        //    {
        //        await DisplayAlert("Logout Error", "Unable to logout at this time", "OK");
        //        //bool result = await DisplayAlert(Resource.LogOut, Resource.LogOutMessage, Resource.Yes, Resource.No);
        //        //if (result)
        //        //{
        //        //    DatabaseService dbService = new DatabaseService();
        //        //    dbService.Logout();
        //        //    Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new LandingPage());
        //        //}
        //    }
        //}
    }
}
