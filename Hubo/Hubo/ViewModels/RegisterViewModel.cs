using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void ProceedToRegister()
        {
            //TODO: Check if email is in legit style
            if ((FirstName.Length != 0) && (LastName.Length != 0) && (Email.Length != 0) && (Password.Length != 0))
            {
                //TODO: Code to send info to the webservice

                MessagingCenter.Send<string>("SuccessfulRegister", "SuccessfulRegister");
            }
            else
            {
                MessagingCenter.Send<string>("IncompleteForm", "IncompleteForm");
            }
        }
    }
}
