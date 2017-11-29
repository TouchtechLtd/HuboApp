// <copyright file="LandingPageViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Acr.UserDialogs;
    using Xamarin.Forms;

    internal class LandingPageViewModel
    {
        private NavigationPage loadPage;

        private RestService restService;

        private BottomNavBar navBar = new BottomNavBar();

        private BottomBarPage bottomBarPage;

        public LandingPageViewModel()
        {
            LoginButtonText = Resource.LoginText;
            RegisterButton = new Command(NavigateToRegisterPage);

        }

        public INavigation Navigation { get; set; }

        public ICommand LoginButton { get; set; }

        public ICommand RegisterButton { get; set; }

        public string LoginButtonText { get; set; }

        public async Task<bool> Login(string username, string password)
        {
            using (UserDialogs.Instance.Loading(Resource.GetDetails, null, null, true, MaskType.Gradient))
            {
                restService = new RestService();
                DatabaseService db = new DatabaseService();

                if (!await restService.Login(username, password))
                {
                    return false;
                }

                UserTable user = db.GetUserInfo();

                if (!await restService.GetUser(user))
                {
                    db.ClearTablesForUserShifts();
                    await UserDialogs.Instance.AlertAsync(Resource.GetDetailsError, Resource.Alert, Resource.Okay);
                    return false;
                }

                if (!await restService.GetCompanies(user.DriverId))
                {
                    db.ClearTablesForUserShifts();
                    await UserDialogs.Instance.AlertAsync(Resource.GetDetailsError, Resource.Alert, Resource.Okay);
                    return false;
                }

                if (!await restService.GetVehicles())
                {
                    db.ClearTablesForUserShifts();
                    await UserDialogs.Instance.AlertAsync(Resource.GetDetailsError, Resource.Alert, Resource.Okay);
                    return false;
                }

                if (!await restService.GetShifts(user.DriverId))
                {
                    db.ClearTablesForUserShifts();
                    await UserDialogs.Instance.AlertAsync(Resource.GetDetailsError, Resource.Alert, Resource.Okay);
                    return false;
                }

                bottomBarPage = navBar.GetBottomBar();
                NavigationPage.SetHasNavigationBar(bottomBarPage, false);
                loadPage = new NavigationPage(bottomBarPage);
                MessagingCenter.Send<string>("ReloadPage", "ReloadPage");
                MessagingCenter.Send<string>("ReloadOthersPage", "ReloadOthersPage");
                MessagingCenter.Send<string>("UpdateVehicles", "UpdateVehicles");
                Application.Current.MainPage = loadPage;
                return true;

            }

        }

        private void NavigateToRegisterPage()
        {
            // Navigation.PushModalAsync(new RegisterPage());
        }
    }
}
