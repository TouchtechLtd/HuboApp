// <copyright file="ProfileCompanyPage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using Xamarin.Forms;

    public partial class ProfileCompanyPage : ContentPage
    {
        private readonly ProfileViewModel profileVM = new ProfileViewModel();

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
