﻿// <copyright file="LandingPageViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using Acr.UserDialogs;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Xamarin.Forms;

    internal class LandingPageViewModel
    {
        private RestService restService;

        private BottomNavBar navBar = new BottomNavBar();

        private BottomBarPage bottomBarPage;

        public NavigationPage loadPage;

        public LandingPageViewModel()
        {
            LoginButtonText = Resource.LoginText;
            LoginButton = new Command(NavigateToLoginPage);
            RegisterButton = new Command(NavigateToRegisterPage);
            bottomBarPage = navBar.GetBottomBar();
            NavigationPage.SetHasNavigationBar(bottomBarPage, false);
            loadPage = new NavigationPage(bottomBarPage);
        }

        public INavigation Navigation { get; set; }

        public ICommand LoginButton { get; set; }

        public ICommand RegisterButton { get; set; }


        public string LoginButtonText { get; set; }

        private void NavigateToRegisterPage()
        {
            Navigation.PushModalAsync(new RegisterPage());
        }

        private void NavigateToLoginPage()
        {
            Navigation.PushModalAsync(new LoginPage(), false);
        }

        public async Task<bool> Login(string username, string password)
        {
            restService = new RestService();
            DatabaseService db = new DatabaseService();
            bool loggedIn;
            using (UserDialogs.Instance.Loading("Logging In....", null, null, true, MaskType.Gradient))
            {
                loggedIn = await restService.Login(username, password);
            }

            if (loggedIn)
            {
                UserTable user = db.GetUserInfo();
                int userResult;
                int shiftResult;

                using (UserDialogs.Instance.Loading("Getting Details....", null, null, true, MaskType.Gradient))
                {
                    userResult = await restService.GetUser(user);
                }

                if (userResult == 3)
                {
                    using (UserDialogs.Instance.Loading("Getting Shifts....", null, null, true, MaskType.Gradient))
                    {
                        shiftResult = await restService.GetShifts(user.DriverId);
                    }

                    if (shiftResult == 4)
                    {
                        //Application.Current.MainPage = new NZTAMessagePage(1);
                        Application.Current.MainPage = loadPage;
                        return true;
                    }
                    else
                    {
                        await UserDialogs.Instance.ConfirmAsync(Resource.GetDetailsError, Resource.NoUsernameOrPasswordTitle, Resource.DisplayAlertOkay);
                        db.ClearTablesForUserShifts();
                    }
                }
                else
                {
                    await UserDialogs.Instance.ConfirmAsync(Resource.GetDetailsError, Resource.NoUsernameOrPasswordTitle, Resource.DisplayAlertOkay);
                    db.ClearTablesForUserShifts();
                }
            }
            return false;
        }
    }
}
