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

            username.ReturnType = ReturnType.Next;
            username.Next = password;

            password.ReturnType = ReturnType.Go;
            password.Completed += Password_Completed;
        }

        private void Password_Completed(object sender, EventArgs e)
        {
            loginVM.NavigateToNZTAMessage();
        }
    }
}
