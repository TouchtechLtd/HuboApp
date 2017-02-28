// <copyright file="HistoryPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public partial class HistoryPage : ContentPage
    {
        private readonly DatabaseService dbService = new DatabaseService();
        private readonly HistoryViewModel historyVM;

        public HistoryPage()
        {
            InitializeComponent();
            historyVM = new HistoryViewModel();
            ToolbarItem topLeftText = new ToolbarItem();
            topLeftText.Text = "History";
            ToolbarItems.Add(topLeftText);
            historyVM.Navigation = Navigation;
            BindingContext = historyVM;
            Title = Resource.HistoryText;
            picker.DateSelected += Picker_DateSelected;

            // LoadTip();
        }

        private void Picker_DateSelected(object sender, DateChangedEventArgs e)
        {
            historyVM.UpdateShift();
        }

        private async Task LoadTip()
        {
            if (dbService.ShowTip("HistoryViewModel"))
            {
                bool tipResult = await DisplayAlert(Resource.Tip, Resource.HistoryTip, Resource.GotIt, Resource.DontShowAgain);
                if (!tipResult)
                {
                    dbService.HideTip("HistoryViewModel");
                }
            }
        }
    }
}
