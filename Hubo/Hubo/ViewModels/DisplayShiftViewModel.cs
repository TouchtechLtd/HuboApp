// <copyright file="DisplayShiftViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Input;
    using Xamarin.Forms;

    internal class DisplayShiftViewModel : INotifyPropertyChanged
    {
        private DatabaseService db = new DatabaseService();

        public DisplayShiftViewModel()
        {
            DateText = Resource.DateEquals;
            LocationText = Resource.Location;
            HuboText = Resource.HuboEquals;
            Dash = Resource.Dash;
        }

        public DisplayShiftViewModel(DateTime date)
        {
            ShiftSelected = false;
            DrivesAvailable = false;
            NotesAvailable = false;
            BreaksAvailable = false;
            SelectedDate = date;
            DateText = Resource.DateEquals;
            DrivesText = Resource.Drive;
            LocationText = Resource.Location;
            NotesText = Resource.NotesText;
            BreaksText = Resource.BreaksText;
            CloseCommand = new Command(Close);
            CloseText = Resource.CloseText;

            // GetCommand = new Command(GetBreaks);
            ShiftList = db.GetDayShifts(date);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<ShiftTable> ShiftList { get; set; }

        public DateTime SelectedDate { get; set; }

        public bool ShiftSelected { get; set; }

        public bool DrivesAvailable { get; set; }

        public bool NotesAvailable { get; set; }

        public string ShiftDate { get; set; }

        public string ShiftLocation { get; set; }

        public string Dash { get; set; }

        public string CloseText { get; set; }

        public string DateText { get; set; }

        public string LocationText { get; set; }

        public string HuboText { get; set; }

        public string DrivesText { get; set; }

        public string NotesText { get; set; }

        public string BreaksText { get; set; }

        public List<DriveTable> Drives { get; set; }

        public List<BreakTable> Breaks { get; set; }

        public List<NoteTable> Notes { get; set; }

        public ICommand CloseCommand { get; set; }

        public INavigation Navigation { get; set; }

        public bool BreaksAvailable { get; set; }

        public void Close()
        {
            Navigation.PopModalAsync();
        }

        public void LoadShiftDetails(ShiftTable shift)
        {
            Drives = new List<DriveTable>();
            Notes = new List<NoteTable>();
            Breaks = new List<BreakTable>();

            Drives = db.GetDriveShifts(shift.Key);
            Notes = db.GetNotes(shift.Key);
            Breaks = db.GetBreaks(shift);

            if (Drives.Count != 0)
            {
                DrivesAvailable = true;
            }
            else
            {
                DrivesAvailable = false;
            }

            if (Notes.Count != 0)
            {
                NotesAvailable = true;
            }
            else
            {
                NotesAvailable = false;
            }

            if (Breaks.Count != 0)
            {
                BreaksAvailable = true;
            }
            else
            {
                BreaksAvailable = false;
            }

            if (shift.EndDate != string.Empty)
            {
                ShiftDate = DateTime.Parse(shift.StartDate).ToString("g") + " - " + DateTime.Parse(shift.EndDate).ToString("g");
            }
            else
            {
                ShiftDate = DateTime.Parse(shift.StartDate).ToString("g") + " - Current";
            }

            if (shift.StartLocation != null)
            {
                if (shift.EndLocation != null)
                {
                    ShiftLocation = shift.StartLocation + " - " + shift.EndLocation;
                }
                else
                {
                    ShiftLocation = shift.StartLocation + " -";
                }
            }
            else
            {
                ShiftLocation = "Unknown - Unknown";
            }

            ShiftSelected = true;

            OnPropertyChanged("Drives");
            OnPropertyChanged("Notes");
            OnPropertyChanged("Breaks");
            OnPropertyChanged("ShiftDate");
            OnPropertyChanged("ShiftLocation");
            OnPropertyChanged("ShiftSelected");
            OnPropertyChanged("DrivesAvailable");
            OnPropertyChanged("BreaksAvailable");
            OnPropertyChanged("NotesAvailable");
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
