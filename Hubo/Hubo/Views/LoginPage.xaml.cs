using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hubo
{
    public partial class LoginPage : ContentPage
    {
        LoginViewModel loginVM = new LoginViewModel();

        public LoginPage()
        {
            InitializeComponent();
            loginVM.Navigation = Navigation;
            BindingContext = loginVM;
            Title = Resource.LoginText;
            username.Completed += Username_Completed;
            password.Completed += Password_Completed;
        }

        private void Password_Completed(object sender, EventArgs e)
        {
            loginVM.NavigateToNZTAMessage();
        }

        private void Username_Completed(object sender, EventArgs e)
        {
            password.Focus();
        }
    }
}
