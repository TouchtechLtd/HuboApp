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
                IsBusy = true;
                SetLoadingText();
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

        private void SetLoadingText()
        {
            List<LoadTextTable> loadText = new List<LoadTextTable>();
            loadText = db.GetLoadingText();

            Random random = new Random();

            int id = random.Next(1, loadText.Count);

            LoadingText = loadText[id - 1].LoadText;

            var sec = TimeSpan.FromSeconds(5);

            Device.StartTimer(sec, () =>
            {
                id = random.Next(1, loadText.Count);

                LoadingText = loadText[id - 1].LoadText;

                return IsBusy;
            });
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
