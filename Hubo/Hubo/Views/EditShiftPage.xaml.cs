// <copyright file="EditShiftPage.xaml.cs" company="Trio Technology LTD">
// Copyright (c) Trio Technology LTD. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Xamarin.Forms;

    public partial class EditShiftPage : ContentPage
    {
        private readonly EditShiftViewModel editShiftVM = new EditShiftViewModel();
        private readonly List<ShiftTable> listOfShifts = new List<ShiftTable>();

        public EditShiftPage(List<ShiftTable> shifts)
        {
            InitializeComponent();
            editShiftVM.Navigation = Navigation;
            BindingContext = editShiftVM;
            Title = Resource.EditShift;
            shiftPicker.Title = Resource.ShiftPickerTitle;

            driveList.ItemSelected += (sender, e) =>
            {
                if (((ListView)sender).SelectedItem == null)
                {
                    return;
                }
                editShiftVM.SelectedDrive = (driveList.ItemsSource as ObservableCollection<DriveTable>).IndexOf(e.SelectedItem as DriveTable);
                editShiftVM.EditShiftDetails("Drives");
                ((ListView)sender).SelectedItem = null;
            };

            listOfShifts = shifts;

            foreach (ShiftTable shift in listOfShifts)
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

            shiftPicker.SelectedIndexChanged += ShiftPicker_SelectedIndexChanged;
        }

        private void ShiftPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            editShiftVM.LoadInfoFromShift(listOfShifts[shiftPicker.SelectedIndex]);
        }
    }
}
