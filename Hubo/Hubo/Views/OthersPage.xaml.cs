namespace Hubo
{
    using Acr.UserDialogs;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public partial class OthersPage : ContentPage
    {
        private readonly OthersViewModel othersVM = new OthersViewModel();

        public OthersPage()
        {
            InitializeComponent();
            Title = Resource.OthersText;
            ToolbarItem topLeftText = new ToolbarItem();
            topLeftText.Text = "Other";
            ToolbarItems.Add(topLeftText);
            Icon = "Menu25.png";
            BindingContext = othersVM;

            othersList.ItemSelected += async (sender, e) =>
            {
                if (((ListView)sender).SelectedItem == null)
                {
                    return;
                }

                ((ListView)sender).SelectedItem = null;
                await NavigateTo(e.SelectedItem as MenuItem);
            };
        }

        private async Task NavigateTo(MenuItem menu)
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
                bool result = await UserDialogs.Instance.ConfirmAsync(Resource.LogOut, Resource.LogOutMessage, Resource.Yes, Resource.No);
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
