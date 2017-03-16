// <copyright file="LoginPage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using Xamarin.Forms;

    public partial class LoginPage : ContentPage
    {
        private readonly LoginViewModel loginVM = new LoginViewModel();

        public LoginPage()
        {
            InitializeComponent();
            loginVM.Navigation = Navigation;
            BindingContext = loginVM;
            Title = Resource.LoginText;

            username.ReturnType = ReturnType.Next;
            username.Next = password;

            password.ReturnType = ReturnType.Go;
            password.Completed += Password_Completed;
        }

        private async void Password_Completed(object sender, EventArgs e)
        {
            await loginVM.NavigateToNZTAMessage();
        }
    }
}
