// <copyright file="HistoryPage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Telerik.XamarinForms.Chart;
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
            ChartPalette basePalette = new ChartPalette();
            basePalette.Entries.Add(new PaletteEntry() { FillColor = Color.Green, StrokeColor = Color.Green });
            basePalette.Entries.Add(new PaletteEntry() { FillColor = Color.Blue, StrokeColor = Color.Blue });

            chart.Palette = basePalette;

            ChartPalette selectedPalette = new ChartPalette();
            selectedPalette.Entries.Add(new PaletteEntry() { FillColor = Color.Green, StrokeColor = Color.Green });
            selectedPalette.Entries.Add(new PaletteEntry() { FillColor = Color.Blue, StrokeColor = Color.Blue });

            chart.SelectionPalette = selectedPalette;
        }

        private async void SelectionChangedHandler(object sender, EventArgs e)
        {
            var selectedPoint = (sender as ChartSelectionBehavior).SelectedPoints.FirstOrDefault();

            var date = (CategoryData)selectedPoint.DataItem;

            DateTime now = DateTime.Now;

            DateTime selectedDate = DateTime.Parse(date.Category + "/" + now.Year);

            await Navigation.PushModalAsync(new DisplayShiftPage(selectedDate));
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
