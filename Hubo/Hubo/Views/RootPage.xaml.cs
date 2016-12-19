using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    public partial class RootPage : MasterDetailPage
    {
        public RootPage()
        {
            var menuPage = new MenuPage();
            menuPage.CopyList.ItemSelected += (sender, e) =>
            {
                if (((ListView)sender).SelectedItem == null)
                {
                    return;
                }
                ((ListView)sender).SelectedItem = null;
                NavigateTo(e.SelectedItem as MenuItem);
            };
            Master = menuPage;
            Detail = new NavigationPage(new HomePage())
            {
                BarTextColor = Color.White
            };
            this.PropertyChanged += RootPage_PropertyChanged;
            ToolbarItem Settings = new ToolbarItem();
            Settings.Icon = "Settings96.png";
            Settings.Text = "Settings";
            Settings.Command = new Command(NavigateToSettingsPage);
            ToolbarItems.Add(Settings);

            MessagingCenter.Subscribe<string>("Remove_Settings", "Remove_Settings", (sender) =>
            {
                ToolbarItems.Remove(Settings);
            });

            MessagingCenter.Subscribe<string>("Reset_Settings", "Reset_Settings", (sender) =>
            {
                ToolbarItems.Add(Settings);
            });
        }

        private void NavigateToSettingsPage()
        {
            Detail.Navigation.PushAsync(new SettingsPage());
        }

        private void RootPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IsGestureEnabled = true;
        }

        async void NavigateTo(MenuItem menu)
        {
            if(menu.TargetType == "Profile")
            {
                await Detail.Navigation.PushModalAsync(new ProfilePage());
            }
            else if (menu.TargetType == "Vehicles")
            {
                await Detail.Navigation.PushAsync(new VehiclesPage(1));
            }
            else if (menu.TargetType == "History")
            {
                await Detail.Navigation.PushAsync(new HistoryPage());
            }
            else if (menu.TargetType == "AddShift")
            {
                await Detail.Navigation.PushAsync(new AddShiftPage());
            }
            else if (menu.TargetType == "SignOut")
            {
                bool result = await DisplayAlert(Resource.LogOut, Resource.LogOutMessage, Resource.Yes, Resource.No);
                if (result)
                {
                    DatabaseService dbService = new DatabaseService();
                    dbService.Logout();
                    Application.Current.MainPage = new NavigationPage(new LandingPage());
                }
            }
                IsPresented = false;
                IsGestureEnabled = false;
        }
    }
}
