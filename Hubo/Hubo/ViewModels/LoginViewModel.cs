﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class LoginViewModel
    {
        public INavigation Navigation { get; set; }
        public ICommand LoginButton { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string LoginText { get; set; }

        public LoginViewModel()
        {
            LoginButton = new Command(NavigateToNZTAMessage);
            LoginText = Resource.LoginText;
            Username = "";
            Password = "";
        }

        public void NavigateToNZTAMessage()
        {
            if ((Username.Length != 0) && (Password.Length != 0))
            {
                //TODO: Check username & password against database.
                App.Current.MainPage = new NZTAMessagePage();
                
            }
            else
            {
                MessagingCenter.Send<string>("EmptyDetails", "EmptyDetails");
            }
        }
    }
}