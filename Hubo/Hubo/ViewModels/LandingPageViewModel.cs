using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;


namespace Hubo
{
    class LandingPageViewModel
    {
        public INavigation Navigation { get; set; }
        public ICommand LoginButton { get; set; }
        public ICommand RegisterButton { get; set; }

        public string Login { get; set; }
        public string Register { get; set; }

        public LandingPageViewModel()
        {
            Login = Resource.LoginText;
            Register = Resource.RegisterText;

            LoginButton = new Command(NavigateToLoginPage);
            RegisterButton = new Command(NavigateToRegisterPage);
        }

        private void NavigateToRegisterPage()
        {
            throw new NotImplementedException();
        }

        private void NavigateToLoginPage()
        {
            Navigation.PushAsync(new LoginPage());
        }
    }
}
