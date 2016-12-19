using System;
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
        RestService restService;

        public LoginViewModel()
        {
            LoginButton = new Command(NavigateToNZTAMessage);
            LoginText = Resource.LoginText;
            Username = "";
            Password = "";
        }

        public async void NavigateToNZTAMessage()
        {
            Username = Username.Trim();

            if ((Username.Length != 0) && (Password.Length != 0))
            {
                restService = new RestService();
                //TODO: Check username & password against database.
                if (await restService.Login(Username, Password))
                {
                    Application.Current.MainPage = new NZTAMessagePage(1);
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(Resource.NoUsernameOrPasswordTitle, Resource.NoUsernameOrPasswordMessage, Resource.DisplayAlertOkay);
            }
        }
    }
}
