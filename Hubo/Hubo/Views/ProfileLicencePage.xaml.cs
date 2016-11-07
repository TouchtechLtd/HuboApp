using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{ 
    public partial class ProfileLicencePage : ContentPage
    {
        public ProfileLicencePage()
        {
            InitializeComponent();
            licenseNumber.Completed += LicenseNumber_Completed;
            licenseVersion.Completed += LicenseVersion_Completed;
        }

        private void LicenseVersion_Completed(object sender, EventArgs e)
        {
            endorsements.Focus();
        }

        private void LicenseNumber_Completed(object sender, EventArgs e)
        {
            licenseVersion.Focus();
        }
    }
}
