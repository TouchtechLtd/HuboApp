using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class RegisterViewModel
    {
        public INavigation Navigation { get; set; }
        public ICommand RegisterButton { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstNamePlaceholder { get; set; }
        public string LastNamePlaceholder { get; set; }
        public string EmailPlaceholder { get; set; }
        public string PasswordPlaceholder { get; set; }
        public string RegisterButtonText { get; set; }

        public RegisterViewModel()
        {
            RegisterButton = new Command(ProceedToRegister);
            RegisterButtonText = Resource.RegisterText;
            FirstNamePlaceholder = Resource.FirstName;
            LastNamePlaceholder = Resource.LastName;
            EmailPlaceholder = Resource.Email;
            PasswordPlaceholder = Resource.Password;
            FirstName = "";
            LastName = "";
            Email = "";
            Password = "";
        }

        public async void ProceedToRegister()
        {
            //TODO: Check if email is in legit style
            FirstName = FirstName.Trim();
            LastName = LastName.Trim();
            Email = Email.Trim();
            if ((FirstName.Length != 0) && (LastName.Length != 0) && (Email.Length != 0) && (Password.Length != 0))
            {
                if((Regex.IsMatch(Email, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$")))
                {
                    //TODO: Code to send info to the webservice
                    await Application.Current.MainPage.DisplayAlert(Resource.RegisterSuccessTitle, Resource.RegisterSuccessText, Resource.DisplayAlertOkay);
                    Application.Current.MainPage = new NZTAMessagePage(1);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.InvalidEmail, Resource.DisplayAlertOkay);
                }
                
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.MissingText, Resource.DisplayAlertOkay);
            }
        }
    }
}
