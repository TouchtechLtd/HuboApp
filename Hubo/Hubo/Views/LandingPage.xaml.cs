// <copyright file="LandingPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Hubo
{
    using Acr.UserDialogs;
    using System;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public partial class LandingPage : ContentPage
    {
        private readonly LandingPageViewModel landingPageVM = new LandingPageViewModel();
        internal bool isLoggingIn;

        public LandingPage()
        {
            InitializeComponent();
            BindingContext = landingPageVM;
            landingPageVM.Navigation = Navigation;
            loginButton.Clicked += LoginButton_ClickedAsync;
            //usernameEntry.Completed += UsernameEntry_Completed;
            usernameEntry.ReturnType = ReturnType.Next;
            usernameEntry.Next = passwordEntry;
            passwordEntry.ReturnType = ReturnType.Done;
            passwordEntry.Completed += PasswordEntry_Completed;
            rightButton.Clicked += RightButton_Clicked;
            isLoggingIn = false;

        }

        private async void RightButton_Clicked(object sender, EventArgs e)
        {
            if (isLoggingIn)
            {
                await huboLabelSecond.FadeTo(0, 250, Easing.Linear);
                await stackLayoutLogin.FadeTo(0, 250, Easing.Linear);
                await huboLabelFirst.FadeTo(1, 250, Easing.Linear);
                isLoggingIn = false;
                rightButton.Text = Resource.RegisterText;
            }
            else
            {
                await UserDialogs.Instance.ConfirmAsync("Alert", "Currently in progress of building", "Got it", "Still got it");
            }
        }

        private async void PasswordEntry_Completed(object sender, EventArgs e)
        {
            LoginButton_ClickedAsync(sender, e);
        }

        //private void UsernameEntry_Completed(object sender, EventArgs e)
        //{
        //    passwordEntry.Focus();
        //}

        public async void LoginButton_ClickedAsync(object sender, EventArgs e)
        {
            if (!isLoggingIn)
            {
                await huboLabelFirst.FadeTo(0, 250, Easing.Linear);
                await stackLayoutLogin.FadeTo(1, 252, Easing.Linear);
                await huboLabelSecond.FadeTo(1, 250, Easing.Linear);
                usernameEntry.Focus();
                isLoggingIn = true;
                rightButton.Text = Resource.Cancel;
            }
            else
            {
                if (usernameEntry.Text != string.Empty || passwordEntry.Text != string.Empty)
                {
                    if (!await landingPageVM.Login(usernameEntry.Text.Trim(), passwordEntry.Text))
                    {
                        usernameEntry.Text = string.Empty;
                        passwordEntry.Text = string.Empty;
                        UserDialogs.Instance.ShowError("Incorrect Username/Password", 1500);
                    }
                }
                else
                {
                    UserDialogs.Instance.ShowError("Please input both Username & Password", 1500);
                }
            }
        }
    }
}
