using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class ProfilePersonalPage : ContentPage
    {
        public ProfilePersonalPage()
        {
            InitializeComponent();
            firstName.Completed += FirstName_Completed;
            lastName.Completed += LastName_Completed;
            email.Completed += Email_Completed;
        }

        private void Email_Completed(object sender, EventArgs e)
        {
            password.Focus();
        }

        private void LastName_Completed(object sender, EventArgs e)
        {
            email.Focus();
        }

        private void FirstName_Completed(object sender, EventArgs e)
        {
            lastName.Focus();
        }
    }
}
