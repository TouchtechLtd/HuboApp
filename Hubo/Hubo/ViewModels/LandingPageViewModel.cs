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
            bottomBarPage = navBar.GetBottomBar();
            NavigationPage.SetHasNavigationBar(bottomBarPage, false);
            loadPage = new NavigationPage(bottomBarPage);
        }

        public INavigation Navigation { get; set; }

        public ICommand LoginButton { get; set; }

        public ICommand RegisterButton { get; set; }

        public string LoginButtonText { get; set; }

        public async Task<bool> Login(string username, string password)
        {
            restService = new RestService();
            DatabaseService db = new DatabaseService();
            bool loggedIn;
            using (UserDialogs.Instance.Loading(Resource.LoggingIn, null, null, true, MaskType.Gradient))
            {
                loggedIn = await restService.Login(username, password);
            }

            if (loggedIn)
            {
                UserTable user = db.GetUserInfo();
                int userResult;
                int shiftResult;

                using (UserDialogs.Instance.Loading(Resource.GetDetails, null, null, true, MaskType.Gradient))
                {
                    userResult = await restService.GetUser(user);
                }

                if (userResult == 3)
                {
                    using (UserDialogs.Instance.Loading(Resource.GetShifts, null, null, true, MaskType.Gradient))
                    {
                        shiftResult = await restService.GetShifts(user.DriverId);
                    }

                    if (shiftResult == 4)
                    {
                        MessagingCenter.Send<string>("ReloadPage", "ReloadPage");
                        MessagingCenter.Send<string>("ReloadOthersPage", "ReloadOthersPage");
                        MessagingCenter.Send<string>("UpdateVehicles", "UpdateVehicles");
                        Application.Current.MainPage = loadPage;
                        return true;
                    }
                    else
                    {
                        await UserDialogs.Instance.AlertAsync(Resource.GetDetailsError, Resource.Alert, Resource.Okay);
                        db.ClearTablesForUserShifts();
                    }
                }
                else
                {
                    await UserDialogs.Instance.AlertAsync(Resource.GetDetailsError, Resource.Alert, Resource.Okay);
                    db.ClearTablesForUserShifts();
                }
            }

            return false;
        }

        private void NavigateToRegisterPage()
        {
            // Navigation.PushModalAsync(new RegisterPage());
        }
    }
}
