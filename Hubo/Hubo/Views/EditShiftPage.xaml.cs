// <copyright file="EditShiftPage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
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

            breakList.ItemSelected += (sender, e) =>
            {
                if (((ListView)sender).SelectedItem == null)
                {
                    return;
                }

                editShiftVM.SelectedBreak = (breakList.ItemsSource as ObservableCollection<BreakTable>).IndexOf(e.SelectedItem as BreakTable);
                editShiftVM.EditShiftDetails("Breaks");
                ((ListView)sender).SelectedItem = null;
            };

            noteList.ItemSelected += (sender, e) =>
            {
                if (((ListView)sender).SelectedItem == null)
                {
                    return;
                }

                editShiftVM.SelectedNote = (noteList.ItemsSource as ObservableCollection<NoteTable>).IndexOf(e.SelectedItem as NoteTable);
                editShiftVM.EditShiftDetails("Notes");
                ((ListView)sender).SelectedItem = null;
            };

            listOfShifts = shifts;

            foreach (ShiftTable shift in listOfShifts)
            {
                DateTime shiftStart = DateTime.Parse(shift.StartDate);

                // Format and add shifts to picker
                if (shift.EndDate == null)
                {
                    shift.EndDate = "Current";
                }
                else
                {
                    DateTime shiftEnd = DateTime.Parse(shift.EndDate);

                    shiftPicker.Items.Add(string.Format("{0:dd/MM}", shiftStart) + ") " + string.Format("{0:hh:mm tt}", shiftStart) + " - " + string.Format("{0:hh:mm tt}", shiftEnd));
                }
            }

            shiftPicker.SelectedIndexChanged += ShiftPicker_SelectedIndexChanged;
            editShiftVM.LoadInfoFromShift(shifts[0]);
        }

        private void ShiftPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            editShiftVM.LoadInfoFromShift(listOfShifts[shiftPicker.SelectedIndex]);
        }
    }
}
