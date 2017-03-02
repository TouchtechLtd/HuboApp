// <copyright file="AddManBreakNotePage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using Xamarin.Forms;

    public partial class AddManBreakNotePage : ContentPage
    {
        private readonly AddManBreakNoteViewModel addBreakNoteVM;

        public AddManBreakNotePage(string instuction)
        {
            InitializeComponent();
            addBreakNoteVM = new AddManBreakNoteViewModel(instuction);
            BindingContext = addBreakNoteVM;
            addBreakNoteVM.Navigation = Navigation;
            UpdateVehicleItems();
            addButton.Clicked += AddButton_Clicked;

            if (instuction == "Break")
            {
                Title = Resource.AddBreak;

                startBreakLocation.ReturnType = ReturnType.Next;
                startBreakLocation.Next = endBreakLocation;

                endBreakLocation.ReturnType = ReturnType.Done;
            }
            else if (instuction == "Note")
            {
                Title = Resource.AddNote;

                noteDetail.ReturnType = ReturnType.Done;
            }
            else if (instuction == "Drive Shift")
            {
                Title = Resource.AddDrive;

                startDriveHubo.ReturnType = ReturnType.Next;
                startDriveHubo.Next = endDriveHubo;

                endDriveHubo.ReturnType = ReturnType.Done;
            }

            addBreakNoteVM.InflatePage();
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            if (vehiclePicker.SelectedIndex != -1)
            {
                addBreakNoteVM.SelectedVehicle = vehiclePicker.SelectedIndex;
            }
        }

        private void UpdateVehicleItems()
        {
            List<VehicleTable> vehiclePickerItems = addBreakNoteVM.GetVehicles();
            if (vehiclePickerItems != null)
            {
                foreach (VehicleTable vehicle in vehiclePickerItems)
                {
                    vehiclePicker.Items.Add(vehicle.Registration);
                }
            }
        }
    }
}
