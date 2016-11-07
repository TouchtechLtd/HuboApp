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
            username.Completed += Username_Completed;
            password.Completed += Password_Completed;
            MessagingCenter.Subscribe<string>(this, "EmptyDetails", (sender) =>
            {
                DisplayAlert(Resource.NoUsernameOrPasswordTitle, Resource.NoUsernameOrPasswordMessage, Resource.DisplayAlertOkay);
            });
            MessagingCenter.Subscribe<string>(this, "UnsuccessfulLogin", (sender) =>
            {
                DisplayAlert(Resource.DisplayAlertTitle, Resource.UnsuccessfulLogin, Resource.DisplayAlertOkay);
            });
        }

        private void Password_Completed(object sender, EventArgs e)
        {
            loginVM.NavigateToNZTAMessage();
        }

        private void Username_Completed(object sender, EventArgs e)
        {
            password.Focus();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>(this, "EmptyDetails");
            base.OnDisappearing();
        }
    }
}
