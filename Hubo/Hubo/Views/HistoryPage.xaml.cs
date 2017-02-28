// <copyright file="HistoryPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.XamarinForms.Chart;
using Xamarin.Forms;

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

            ChartPalette basePalette = new ChartPalette();
            basePalette.Entries.Add(new PaletteEntry() { FillColor = Color.Green, StrokeColor = Color.Green });
            basePalette.Entries.Add(new PaletteEntry() { FillColor = Color.Blue, StrokeColor = Color.Blue });

            chart.Palette = basePalette;

            ChartPalette selectedPalette = new ChartPalette();
            selectedPalette.Entries.Add(new PaletteEntry() { FillColor = Color.Green, StrokeColor = Color.Green });
            selectedPalette.Entries.Add(new PaletteEntry() { FillColor = Color.Blue, StrokeColor = Color.Blue });

            chart.SelectionPalette = selectedPalette;
        }

        private void SelectionChangedHandler(object sender, EventArgs e)
        {
            var selectedSeries = (sender as ChartSelectionBehavior).SelectedSeries.FirstOrDefault();

            var selectedDetail = selectedSeries.ItemsSource.Cast<CategoryData>();

            List<CategoryData> selectedDetails = new List<CategoryData>(selectedDetail);

            string date = selectedDetails[0].Category.ToString();

            DateTime now = DateTime.Now;

            DateTime selectedDate = DateTime.Parse(date + "/" + now.Year);
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
