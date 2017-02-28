// <copyright file="RegisterPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using Xamarin.Forms;

    public partial class RegisterPage : ContentPage
    {
        private readonly RegisterViewModel registerVM = new RegisterViewModel();

        public RegisterPage()
        {
            InitializeComponent();
            registerVM.Navigation = Navigation;
            BindingContext = registerVM;
            Title = Resource.RegisterText;

            firstName.ReturnType = ReturnType.Next;
            firstName.Next = lastName;

            lastName.ReturnType = ReturnType.Next;
            lastName.Next = email;

            email.ReturnType = ReturnType.Next;
            email.Next = password;

            password.ReturnType = ReturnType.Done;
            password.Completed += Password_Completed;
        }

        private async void Password_Completed(object sender, EventArgs e)
        {
            await registerVM.ProceedToRegister();
        }
    }
}
