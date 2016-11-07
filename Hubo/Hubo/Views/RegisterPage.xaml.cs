﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class RegisterPage : ContentPage
    {
        RegisterViewModel registerVM = new RegisterViewModel();

        public RegisterPage()
        {
            InitializeComponent();
            registerVM.Navigation = Navigation;
            BindingContext = registerVM;
            MessagingCenter.Subscribe<string>(this, "SuccessfulRegister", async (sender) =>
            {
                await DisplayAlert(Resource.RegisterSuccessTitle, Resource.RegisterSuccessText, Resource.DisplayAlertOkay);
                App.Current.MainPage = new NZTAMessagePage();
            });
            
            MessagingCenter.Subscribe<string>(this, "IncompleteForm", async (sender) =>
            {
                await DisplayAlert(Resource.DisplayAlertTitle, Resource.MissingText, Resource.DisplayAlertOkay);
            });

            firstName.Completed += FirstName_Completed;
            lastName.Completed += LastName_Completed;
            password.Completed += Password_Completed;
            email.Completed += Email_Completed;
        }

        private void Email_Completed(object sender, EventArgs e)
        {
            password.Focus();
        }

        private void Password_Completed(object sender, EventArgs e)
        {
            registerVM.ProceedToRegister();
        }

        private void LastName_Completed(object sender, EventArgs e)
        {
            email.Focus();
        }

        private void FirstName_Completed(object sender, EventArgs e)
        {
            lastName.Focus();
        }
    }
}
