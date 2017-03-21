// <copyright file="RegisterViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Acr.UserDialogs;
    using Xamarin.Forms;
    using XLabs;

    public class RegisterViewModel
    {
        public RegisterViewModel()
        {
            RegisterButton = new RelayCommand(async () => await ProceedToRegister());
            RegisterButtonText = Resource.RegisterText;
            FirstNamePlaceholder = Resource.FirstName;
            LastNamePlaceholder = Resource.LastName;
            EmailPlaceholder = Resource.Email;
            PasswordPlaceholder = Resource.Password;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
        }

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

        public async Task ProceedToRegister()
        {
            FirstName = FirstName.Trim();
            LastName = LastName.Trim();
            Email = Email.Trim();
            if ((FirstName.Length != 0) && (LastName.Length != 0) && (Email.Length != 0) && (Password.Length != 0))
            {
                if (Regex.IsMatch(Email, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$"))
                {
                    RestService restAPI = new RestService();

                    UserTable user = new UserTable()
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        Email = Email
                    };
                    int result = await restAPI.RegisterUser(user, Password);

                    switch (result)
                    {
                        case -1:
                            await UserDialogs.Instance.ConfirmAsync("Unable to register user", Resource.RegisterSuccessTitle, Resource.DisplayAlertOkay);
                            return;
                        default:
                            break;
                    }

                    await UserDialogs.Instance.ConfirmAsync(Resource.RegisterSuccessText, Resource.RegisterSuccessTitle, Resource.DisplayAlertOkay);
                    Application.Current.MainPage = new NZTAMessagePage(1);
                }
                else
                {
                    await UserDialogs.Instance.ConfirmAsync(Resource.InvalidEmail, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                }
            }
            else
            {
                await UserDialogs.Instance.ConfirmAsync(Resource.MissingText, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
            }
        }
    }
}