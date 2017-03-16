// <copyright file="EditShiftDetailsPage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using Xamarin.Forms;

    public partial class EditShiftDetailsPage : ContentPage
    {
        private EditShiftDetailsViewModel editShiftDetailsVM;

        public EditShiftDetailsPage(string instruction, DriveTable currentDrive = null, ShiftTable currentShift = null, BreakTable currentBreak = null, NoteTable currentNote = null)
        {
            InitializeComponent();
            editShiftDetailsVM = new EditShiftDetailsViewModel(instruction, currentDrive, currentShift, currentBreak, currentNote)
            {
                Navigation = Navigation
            };
            BindingContext = editShiftDetailsVM;

            if (instruction == "Breaks")
            {
                Title = Resource.BreaksText;

                breakStartLocation.ReturnType = ReturnType.Next;
                breakStartLocation.Next = breakEndLocation;

                breakEndLocation.ReturnType = ReturnType.Done;
            }

            if (instruction == "Notes")
            {
                Title = Resource.NotesText;

                noteEntry.ReturnType = ReturnType.Done;
            }
            else if (instruction == "Drives")
            {
                Title = Resource.DriveText;
                List<VehicleTable> vehicles = new List<VehicleTable>();
                vehicles = editShiftDetailsVM.LoadVehicle();

                foreach (VehicleTable vehicle in vehicles)
                {
                    vehiclePicker.Items.Add(vehicle.Registration);
                }

                vehiclePicker.Title = Resource.SelectVehicle;

                if (editShiftDetailsVM.VehicleId != -1)
                {
                    vehiclePicker.SelectedIndex = editShiftDetailsVM.VehicleId - 1;
                }

                vehiclePicker.SelectedIndexChanged += VehiclePicker_SelectedIndexChanged;

                driveHuboStart.ReturnType = ReturnType.Next;
                driveHuboStart.Next = driveHuboEnd;

                driveHuboEnd.ReturnType = ReturnType.Done;
            }
        }

        private void VehiclePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            editShiftDetailsVM.SelectedIndex = vehiclePicker.SelectedIndex;
        }
    }
}
