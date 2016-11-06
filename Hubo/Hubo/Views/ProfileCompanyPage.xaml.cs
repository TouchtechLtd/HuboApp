using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class ProfileCompanyPage : ContentPage
    {
        public ProfileCompanyPage()
        {
            InitializeComponent();
            name.Completed += Name_Completed;
            address.Completed += Address_Completed;
            companyEmail.Completed += CompanyEmail_Completed;
        }

        private void CompanyEmail_Completed(object sender, EventArgs e)
        {
            phone.Focus();
        }

        private void Address_Completed(object sender, EventArgs e)
        {
            companyEmail.Focus();
        }

        private void Name_Completed(object sender, EventArgs e)
        {
            address.Focus();
        }
    }
}
