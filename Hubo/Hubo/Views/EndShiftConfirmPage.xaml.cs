[assembly: Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]
namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Acr.UserDialogs;

    public partial class EndShiftConfirmPage : ContentPage
    {
        private readonly EndShiftConfirmViewModel endShiftConfirmVm = new EndShiftConfirmViewModel();

        public EndShiftConfirmPage()
        {
            InitializeComponent();
            endShiftConfirmVm.Navigation = Navigation;
            BindingContext = endShiftConfirmVm;
            acceptButton.Clicked += AcceptButton_ClickedAsync;
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
                    endShiftConfirmVm.DriveShiftAccepted();
                }
                else
                {
                    //Either load next break or completed and make call to sync with DB
                    endShiftConfirmVm.BreakAccepted();
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }
    }
}
