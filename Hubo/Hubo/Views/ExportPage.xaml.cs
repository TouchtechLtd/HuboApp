using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class ExportPage : ContentPage
    {
        ExportViewModel exportVM = new ExportViewModel();
        public ExportPage()
        {
            InitializeComponent();
            BindingContext = exportVM;
            emailEntry.Completed += EmailEntry_Completed;
            Title = Resource.Export;
        }

        private void EmailEntry_Completed(object sender, EventArgs e)
        {
            exportVM.Export();
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<string>("PopAfterExport", "PopAfterExport", (sender) =>
            {
                Navigation.PopAsync();
            });
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<string>("PopAfterExport", "PopAfterExport");
            base.OnDisappearing();
        }
    }
}
