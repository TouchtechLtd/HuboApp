using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hubo;
using Xamarin.Forms;

namespace Hubo
{
    public partial class RegisterPage : ContentPage
    {
        RegisterViewModel registerVM = new RegisterViewModel();

        public RegisterPage()
        {
            InitializeComponent();
            registerVM.Navigation = Navigation;
            BindingContext = registerVM;
            Title = Resource.RegisterText;

            firstName.ReturnType = ReturnType.Next;
            firstName.Next = lastName;

            lastName.ReturnType = ReturnType.Next;
            lastName.Next = email;

            email.ReturnType = ReturnType.Next;
            email.Next = password;

            password.ReturnType = ReturnType.Done;
            password.Completed += Password_Completed;
        }

        private void Password_Completed(object sender, EventArgs e)
        {
            registerVM.ProceedToRegister();
        }

        protected override void OnDisappearing()
        {

            base.OnDisappearing();
        }
    }
}
