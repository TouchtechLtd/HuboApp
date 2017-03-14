// <copyright file="AddManBreakNoteViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using Acr.UserDialogs;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using Xamarin.Forms;

    internal class AddManBreakNoteViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService dbService = new DatabaseService();

        public AddManBreakNoteViewModel(string instructionCommand)
        {
            Instruction = instructionCommand;
            CancelText = Resource.Cancel;
            AddCommand = new Command(Add);
            CancelCommand = new Command(Cancel);
            Vehicle = Resource.Vehicle;
            AddingBreak = false;
            AddingNote = false;
            AddingDrive = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Instruction { get; set; }

        public INavigation Navigation { get; set; }

        public List<VehicleTable> Vehicles { get; set; }

        public string BreakStartText { get; set; }

        public string BreakEndText { get; set; }

        public string BreakStartTimeText { get; set; }

        public string BreakEndTimeText { get; set; }

        public string LocationStartText { get; set; }

        public string LocationEndText { get; set; }

        public string HuboStartText { get; set; }

        public string HuboEndText { get; set; }

        public string NoteText { get; set; }

        public string NoteTimeText { get; set; }

        public string NoteDetailText { get; set; }

        public TimeSpan BreakStart { get; set; }

        public TimeSpan BreakEnd { get; set; }

        public string LocationStart { get; set; }

        public string LocationEnd { get; set; }

        public string HuboStart { get; set; }

        public string HuboEnd { get; set; }

        public TimeSpan NoteTime { get; set; }

        public string NoteDetail { get; set; }

        public ICommand AddCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public string AddText { get; set; }

        public string CancelText { get; set; }

        public string DriveText { get; set; }

        public string DriveStartTimeText { get; set; }

        public string DriveEndTimeText { get; set; }

        public string DriveStartHuboText { get; set; }

        public string DriveEndHuboText { get; set; }

        public TimeSpan DriveStartTime { get; set; }

        public TimeSpan DriveEndTime { get; set; }

        public int SelectedVehicle { get; set; }

        public bool AddingBreak { get; set; }

        public bool AddingNote { get; set; }

        public bool AddingDrive { get; set; }

        public string Vehicle { get; set; }

        internal List<VehicleTable> GetVehicles()
        {
            Vehicles = dbService.GetVehicles();
            return Vehicles;
        }

        internal void InflatePage()
        {
            if (Instruction == "Break")
            {
                AddingBreak = true;
                AddText = Resource.AddBreak;
                BreakStartText = Resource.StartBreak;
                BreakStartTimeText = Resource.StartTime;
                LocationStartText = Resource.StartLocation;
                HuboStartText = Resource.StartHubo;
                BreakEndText = Resource.EndBreak;
                BreakEndTimeText = Resource.EndTime;
                LocationEndText = Resource.EndLocation;
                HuboEndText = Resource.EndHubo;

                OnPropertyChanged("AddingBreak");
                OnPropertyChanged("BreakStartText");
                OnPropertyChanged("BreakStartTimeText");
                OnPropertyChanged("LocationStartText");
                OnPropertyChanged("HuboStartText");
                OnPropertyChanged("BreakEndText");
                OnPropertyChanged("BreakEndTimeText");
                OnPropertyChanged("LocationEndText");
                OnPropertyChanged("HuboEndText");
            }
            else if (Instruction == "Note")
            {
                AddingNote = true;
                AddText = Resource.AddNote;
                NoteText = Resource.AddNote;
                NoteTimeText = Resource.Time;
                NoteDetailText = Resource.Note;

                OnPropertyChanged("AddingNote");
                OnPropertyChanged("NoteText");
                OnPropertyChanged("NoteTimeText");
                OnPropertyChanged("NoteDetailText");
            }
            else if (Instruction == "Drive Shift")
            {
                AddingDrive = true;
                AddText = Resource.AddDrive;
                DriveText = Resource.AddDrive;
                DriveStartTimeText = Resource.StartTime;
                DriveEndTimeText = Resource.EndTime;
                DriveStartHuboText = Resource.StartHubo;
                DriveEndHuboText = Resource.EndHubo;

                OnPropertyChanged("AddingDrive");
                OnPropertyChanged("DriveText");
                OnPropertyChanged("DriveStartTimeText");
                OnPropertyChanged("DriveEndTimeText");
                OnPropertyChanged("DriveStartHuboText");
                OnPropertyChanged("DriveEndHuboText");
            }

            OnPropertyChanged("AddText");
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private async void Cancel()
        {
            await Navigation.PopModalAsync();
        }

        private void Add()
        {
            if (Instruction == "Break")
            {
                if (!CheckValidHuboEntry(HuboStart))
                {
                    return;
                }

                if (!CheckValidHuboEntry(HuboEnd))
                {
                    return;
                }

                BreakTable breakAdd = new BreakTable();

                breakAdd.StartDate = BreakStart.ToString("yyyy-MM-dd HH:mm:ss.fff");
                breakAdd.EndDate = BreakEnd.ToString("yyyy-MM-dd HH:mm:ss.fff");
                breakAdd.StartLocation = LocationStart;
                breakAdd.EndLocation = LocationEnd;

                MessagingCenter.Send(this, "Break_Added", breakAdd);
            }
            else if (Instruction == "Note")
            {
                NoteTable note = new NoteTable();
                note.Date = NoteTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                note.Note = NoteDetail;

                MessagingCenter.Send(this, "Note_Added", note);
            }
            else if (Instruction == "Drive Shift")
            {
                if (!CheckValidHuboEntry(HuboStart))
                {
                    return;
                }

                if (!CheckValidHuboEntry(HuboEnd))
                {
                    return;
                }

                List<VehicleTable> vehicleKey = new List<VehicleTable>();
                vehicleKey = GetVehicles();

                DriveTable drive = new DriveTable();
                drive.StartDate = DriveStartTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                drive.EndDate = DriveEndTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                drive.StartHubo = int.Parse(HuboStart);
                drive.EndHubo = int.Parse(HuboEnd);
                drive.ActiveVehicle = false;
                drive.VehicleKey = vehicleKey[SelectedVehicle].Key;

                MessagingCenter.Send(this, "Drive_Added", drive);
            }

            Navigation.PopModalAsync();
        }

        private bool CheckValidHuboEntry(string huboValue)
        {
            Regex regex = new Regex("^[0-9]+$");
            if (huboValue.Length == 0)
            {
                UserDialogs.Instance.ConfirmAsync(Resource.InvalidHubo, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                return false;
            }

            if (!regex.IsMatch(huboValue))
            {
                UserDialogs.Instance.ConfirmAsync(Resource.InvalidHubo, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                return false;
            }

            return true;
        }
    }
}