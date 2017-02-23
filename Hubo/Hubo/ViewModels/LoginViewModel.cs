using Acr.UserDialogs;
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

        private string _loadingText;
        public string LoadingText
        {
            get
            {
                return _loadingText;
            }

            set
            {
                _loadingText = value;
                OnPropertyChanged("LoadingText");
            }
        }
        RestService restService;

        DatabaseService db = new DatabaseService();

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
                bool loggedIn = false;
                //TODO: Check username & password against database.
                using (UserDialogs.Instance.Loading("Logging In....", null, null, true, MaskType.Gradient))
                    loggedIn = await restService.Login(Username, Password);

                if (loggedIn)
                {
                    UserTable user = db.GetUserInfo();

                    using (UserDialogs.Instance.Loading("Getting Details....", null, null, true, MaskType.Gradient))
                        await restService.GetUser(user);

                    using (UserDialogs.Instance.Loading("Getting Shifts....", null, null, true, MaskType.Gradient))
                        await restService.GetShifts(user.DriverId);

                    Application.Current.MainPage = new NZTAMessagePage(1);
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
