// <copyright file="EndShiftConfirmPage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

[assembly: Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]
namespace Hubo
{
    using System;
    using Acr.UserDialogs;
    using Xamarin.Forms;

    public partial class EndShiftConfirmPage : ContentPage
    {
        private readonly EndShiftConfirmViewModel endShiftConfirmVm = new EndShiftConfirmViewModel();

        public EndShiftConfirmPage()
        {
            InitializeComponent();
            BindingContext = endShiftConfirmVm;
            acceptButton.Clicked += AcceptButton_ClickedAsync;
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }

        private async void AcceptButton_ClickedAsync(object sender, EventArgs e)
        {
            if (await UserDialogs.Instance.ConfirmAsync("Are you sure these details are correct? You may not change these details after confirming.", "Confirm", "Agree", "Cancel"))
            {
                if (endShiftConfirmVm.WorkShift)
                {
                    // Load details of Driving Shifts and then animate the stuff away
                    endShiftConfirmVm.WorkShiftDone();
                }
                else if (endShiftConfirmVm.DriveShift)
                {
                    // Either increment or load details of Break Shifts
                }
                else
                {
                    // Either load next break or completed and make call to sync with DB
                }
            }
        }
    }
}
