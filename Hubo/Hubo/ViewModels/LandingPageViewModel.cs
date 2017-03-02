// <copyright file="LandingPageViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Windows.Input;
    using Xamarin.Forms;

    internal class LandingPageViewModel
    {
        public LandingPageViewModel()
        {
            Login = Resource.LoginText;
            Register = Resource.RegisterText;

            LoginButton = new Command(NavigateToLoginPage);
            RegisterButton = new Command(NavigateToRegisterPage);
        }

        public INavigation Navigation { get; set; }

        public ICommand LoginButton { get; set; }

        public ICommand RegisterButton { get; set; }

        public string Login { get; set; }

        public string Register { get; set; }

        private void NavigateToRegisterPage()
        {
            Navigation.PushModalAsync(new RegisterPage());
        }

        private void NavigateToLoginPage()
        {
            Navigation.PushModalAsync(new LoginPage(), false);
        }
    }
}
