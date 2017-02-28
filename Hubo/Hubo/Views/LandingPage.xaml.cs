// <copyright file="LandingPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Hubo
{
    using Xamarin.Forms;

    public partial class LandingPage : ContentPage
    {
        private readonly LandingPageViewModel landingPageVM = new LandingPageViewModel();

        public LandingPage()
        {
            InitializeComponent();
            BindingContext = landingPageVM;
            landingPageVM.Navigation = Navigation;
        }
    }
}
