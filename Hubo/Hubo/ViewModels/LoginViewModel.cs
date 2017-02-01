using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class LoginViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public ICommand LoginButton { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isBusy;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

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
            IsBusy = false;
        }

        public async void NavigateToNZTAMessage()
        {
            Username = Username.Trim();

            if ((Username.Length != 0) && (Password.Length != 0))
            {
                restService = new RestService();
                IsBusy = true;
                //TODO: Check username & password against database.
                if (await restService.Login(Username, Password))
                {
                    Application.Current.MainPage = new NZTAMessagePage(1);
                    IsBusy = false;
                }
                else
                {
                    IsBusy = false;
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(Resource.NoUsernameOrPasswordTitle, Resource.NoUsernameOrPasswordMessage, Resource.DisplayAlertOkay);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
