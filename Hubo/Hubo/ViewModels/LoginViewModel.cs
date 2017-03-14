// <copyright file="LoginViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Acr.UserDialogs;
    using Xamarin.Forms;
    using XLabs;

    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService db = new DatabaseService();

        private RestService restService;

        public LoginViewModel()
        {
            LoginButton = new RelayCommand(async () => await NavigateToNZTAMessage());
            LoginText = Resource.LoginText;
            Username = string.Empty;
            Password = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public INavigation Navigation { get; set; }

        public ICommand LoginButton { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string LoginText { get; set; }

        public async Task NavigateToNZTAMessage()
        {
            Username = Username.Trim();

            if ((Username.Length != 0) && (Password.Length != 0))
            {
                restService = new RestService();
                bool loggedIn;
                using (UserDialogs.Instance.Loading("Logging In....", null, null, true, MaskType.Gradient))
                {
                    loggedIn = await restService.Login(Username, Password);
                }

                if (loggedIn)
                {
                    UserTable user = db.GetUserInfo();
                    int userResult;
                    int shiftResult;

                    using (UserDialogs.Instance.Loading("Getting Details....", null, null, true, MaskType.Gradient))
                    {
                        userResult = await restService.GetUser(user);
                    }

                    if (userResult == 3)
                    {
                        using (UserDialogs.Instance.Loading("Getting Shifts....", null, null, true, MaskType.Gradient))
                        {
                            shiftResult = await restService.GetShifts(user.DriverId);
                        }

                        if (shiftResult == 4)
                        {
                            Application.Current.MainPage = new NZTAMessagePage(1);
                        }
                        else
                        {
                            await UserDialogs.Instance.ConfirmAsync(Resource.GetDetailsError, Resource.NoUsernameOrPasswordTitle, Resource.DisplayAlertOkay);
                            db.ClearTablesForUserShifts();
                        }
                    }
                    else
                    {
                        await UserDialogs.Instance.ConfirmAsync(Resource.GetDetailsError, Resource.NoUsernameOrPasswordTitle, Resource.DisplayAlertOkay);
                        db.ClearTablesForUserShifts();
                    }
                }
            }
            else
            {
                await UserDialogs.Instance.ConfirmAsync(Resource.NoUsernameOrPasswordMessage, Resource.NoUsernameOrPasswordTitle, Resource.DisplayAlertOkay);
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
