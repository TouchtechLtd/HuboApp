namespace Hubo
{
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Input;
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

                    UserTable user = new UserTable();
                    user.FirstName = FirstName;
                    user.LastName = LastName;
                    user.Email = Email;

                    int result = await restAPI.RegisterUser(user, Password);

                    switch (result)
                    {
                        case -1:
                            await Application.Current.MainPage.DisplayAlert(Resource.RegisterSuccessTitle, "Unable to register user", Resource.DisplayAlertOkay);
                            return;
                        default:
                            break;
                    }

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
