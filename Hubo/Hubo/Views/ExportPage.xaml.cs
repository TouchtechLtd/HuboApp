// <copyright file="ExportPage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public partial class ExportPage : ContentPage
    {
        private readonly ExportViewModel exportVM = new ExportViewModel();

        public ExportPage()
        {
            InitializeComponent();
            BindingContext = exportVM;
            exportVM.Navigation = Navigation;
            emailEntry.Completed += EmailEntry_Completed;
            Title = Resource.Export;
        }

        //protected override void OnAppearing()
        //{
        //    MessagingCenter.Subscribe<string>("PopAfterExport", "PopAfterExport", (sender) =>
        //    {
        //        Navigation.PopModalAsync();
        //    });
        //    base.OnAppearing();
        //}

        //protected override void OnDisappearing()
        //{
        //    MessagingCenter.Unsubscribe<string>("PopAfterExport", "PopAfterExport");
        //    base.OnDisappearing();
        //}

        private async void EmailEntry_Completed(object sender, EventArgs e)
        {
            await exportVM.Export();
        }
    }
}
