namespace Hubo
{
    using Acr.UserDialogs;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public partial class RootPage : MasterDetailPage
    {
        public RootPage()
        {
            var menuPage = new MenuPage();
            menuPage.CopyList.ItemSelected += async (sender, e) =>
            {
                if (((ListView)sender).SelectedItem == null)
                {
                    return;
                }

                ((ListView)sender).SelectedItem = null;
                await NavigateTo(e.SelectedItem as MenuItem);
            };
            Master = menuPage;
            Detail = new NavigationPage(new HomePage())
            {
                BarTextColor = Color.White
            };
            this.PropertyChanged += RootPage_PropertyChanged;
            ToolbarItem settings = new ToolbarItem();
            settings.Icon = "Settings96.png";
            settings.Text = "Settings";
            settings.Command = new Command(NavigateToSettingsPage);
            ToolbarItems.Add(settings);

            MessagingCenter.Subscribe<string>("Remove_Settings", "Remove_Settings", (sender) =>
            {
                ToolbarItems.Remove(settings);
            });

            MessagingCenter.Subscribe<string>("Reset_Settings", "Reset_Settings", (sender) =>
            {
                ToolbarItems.Add(settings);
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

        private async Task NavigateTo(MenuItem menu)
        {
            if (menu.TargetType == "Home")
            {
                await Detail.Navigation.PopToRootAsync(true);
            }
            else if (menu.TargetType == "Profile")
            {
                await Detail.Navigation.PushModalAsync(new ProfilePage());
            }
            else if (menu.TargetType == "Vehicles")
            {
                await Detail.Navigation.PushAsync(new VehiclesPage());
            }
            else if (menu.TargetType == "History")
            {
                await Detail.Navigation.PushAsync(new HistoryPage());
            }
            else if (menu.TargetType == "AddShift")
            {
                await Detail.Navigation.PushModalAsync(new AddShiftPage());
            }
            else if (menu.TargetType == "SignOut")
            {
                bool result = await UserDialogs.Instance.ConfirmAsync(Resource.LogOut, Resource.LogOutMessage, Resource.Yes, Resource.No);
                if (result)
                {
                    DatabaseService dbService = new DatabaseService();
                    dbService.Logout();
                    Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new LandingPage());
                }
            }

            IsPresented = false;
            IsGestureEnabled = false;
        }
    }
}
