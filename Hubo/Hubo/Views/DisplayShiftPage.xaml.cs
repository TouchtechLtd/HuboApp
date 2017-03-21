// <copyright file="DisplayShiftPage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using Xamarin.Forms;

    public partial class DisplayShiftPage : ContentPage
    {
        private DisplayShiftViewModel displayVM;
        private List<ShiftTable> shifts = new List<ShiftTable>();

        public DisplayShiftPage(DateTime selectedDate)
        {
            InitializeComponent();
            displayVM = new DisplayShiftViewModel(selectedDate)
            {
                Navigation = Navigation
            };
            BindingContext = displayVM;
            shiftPicker.Title = Resource.ShiftPickerTitle;

            shifts = displayVM.ShiftList;

            foreach (ShiftTable shift in shifts)
            {
                // Format and add shifts to picker
                if (shift.EndDate == null)
                {
                    shift.EndDate = "Current";
                }

                DateTime shiftStart = DateTime.Parse(shift.StartDate);
                DateTime shiftEnd = DateTime.Parse(shift.EndDate);

                shiftPicker.Items.Add(string.Format("{0:dd/MM}", shiftStart) + ") " + string.Format("{0:hh:mm tt}", shiftStart) + " - " + string.Format("{0:hh:mm tt}", shiftEnd));
            }

            driveList.ItemSelected += (sender, e) =>
            {
                if (((ListView)sender).SelectedItem == null)
                {
                    return;
                }

                ((ListView)sender).SelectedItem = null;
            };

            noteList.ItemSelected += (sender, e) =>
             {
                 if (((ListView)sender).SelectedItem == null)
                 {
                     return;
                 }

                ((ListView)sender).SelectedItem = null;
             };

            breakList.ItemSelected += (sender, e) =>
             {
                 if (((ListView)sender).SelectedItem == null)
                 {
                     return;
                 }

                ((ListView)sender).SelectedItem = null;
             };

            shiftPicker.SelectedIndexChanged += ShiftPicker_SelectedIndexChanged;
        }

        private void ShiftPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayVM.LoadShiftDetails(shifts[shiftPicker.SelectedIndex]);
        }
    }
}
