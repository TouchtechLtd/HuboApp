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
        ProfileViewModel profileVM = new ProfileViewModel();
        public ProfileCompanyPage()
        {
            InitializeComponent();
            BindingContext = profileVM;

            companyList.ItemSelected += (sender, e) =>
            {
                if (((ListView)sender).SelectedItem == null)
                {
                    return;
                }
                ((ListView)sender).SelectedItem = null;
            };
        }
    }
}
