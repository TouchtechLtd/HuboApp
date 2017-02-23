using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class OthersPage : ContentPage
    {
        OthersViewModel othersVM = new OthersViewModel();

        public OthersPage()
        {
            InitializeComponent();
            Title = Resource.OthersText;
            ToolbarItem topLeftText = new ToolbarItem();
            topLeftText.Text = "Other";
            ToolbarItems.Add(topLeftText);
            Icon = "Menu25.png";
            BindingContext = othersVM;

            othersList.ItemSelected += (sender, e) =>
            {
                if (((ListView)sender).SelectedItem == null)
                {
                    return;
                }
                ((ListView)sender).SelectedItem = null;
                NavigateTo(e.SelectedItem as MenuItem);
            };
        }

        async void NavigateTo(MenuItem menu)
        {
            if (menu.TargetType == "Profile")
            {
                await Navigation.PushModalAsync(new ProfilePage());
            }
            else if (menu.TargetType == "Settings")
            {
                await Navigation.PushModalAsync(new SettingsPage());
            }
            else if (menu.TargetType == "SignOut")
            {
                //await DisplayAlert("Logout Error", "Unable to logout at this time", "OK");
                bool result = await DisplayAlert(Resource.LogOut, Resource.LogOutMessage, Resource.Yes, Resource.No);
                if (result)
                {
                    DatabaseService dbService = new DatabaseService();
                    dbService.Logout();
                    Xamarin.Forms.Application.Current.MainPage = new LandingPage();
                }
            }
        }
    }
}
