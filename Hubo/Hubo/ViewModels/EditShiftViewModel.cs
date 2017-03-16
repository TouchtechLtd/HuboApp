// <copyright file="EditShiftViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Input;
    using Acr.UserDialogs;
    using Xamarin.Forms;

    internal class EditShiftViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService dbService = new DatabaseService();
        private readonly List<AmendmentTable> listOfAmendments = new List<AmendmentTable>();

        private ShiftTable currentShift = new ShiftTable();
        private List<DriveTable> driveList = new List<DriveTable>();
        private List<BreakTable> breakList = new List<BreakTable>();
        private List<NoteTable> noteList = new List<NoteTable>();

        public EditShiftViewModel()
        {
            DashText = Resource.Dash;
            ShiftStartTime = Resource.ShiftStartTime;
            ShiftEndTime = Resource.ShiftEndTime;
            ShiftStartInfoVisible = false;
            ShiftEndInfoVisible = false;
            DrivesAvailable = false;
            BreaksAvailable = false;
            NotesAvailable = false;
            SaveCommand = new Command(Save);
            NotesCommand = new Command(EditShiftDetails);
            DrivesCommand = new Command(EditShiftDetails);
            SaveText = Resource.Save;
            AddDrivesText = Resource.AddDrive;
            EditNotesText = Resource.EditNotes;
            CancelText = Resource.Cancel;
            ChangesMade = false;
            ShiftSelected = false;
            SelectedDrive = -1;
            SelectedBreak = -1;
            SelectedNote = -1;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public INavigation Navigation { get; set; }

        public string ShiftStartTime { get; set; }

        public string ShiftEndTime { get; set; }

        public string DriveText { get; set; }

        public string BreakText { get; set; }

        public string NoteText { get; set; }

        public TimeSpan ShiftStartTimePicker { get; set; }

        public TimeSpan ShiftEndTimePicker { get; set; }

        public DateTime ShiftStartDatePicker { get; set; }

        public DateTime ShiftEndDatePicker { get; set; }

        public ICommand SaveCommand { get; set; }

        public ICommand DrivesCommand { get; set; }

        public ICommand NotesCommand { get; set; }

        public string SaveText { get; set; }

        public string AddDrivesText { get; set; }

        public string EditNotesText { get; set; }

        public string CancelText { get; set; }

        public bool ChangesMade { get; set; }

        public bool ShiftStartInfoVisible { get; set; }

        public bool ShiftEndInfoVisible { get; set; }

        public bool DrivesAvailable { get; set; }

        public bool BreaksAvailable { get; set; }

        public bool NotesAvailable { get; set; }

        public bool ShiftSelected { get; set; }

        public string DashText { get; set; }

        public int SelectedDrive { get; set; }

        public int SelectedBreak { get; set; }

        public int SelectedNote { get; set; }

        public ObservableCollection<DriveTable> Drives { get; set; }

        public ObservableCollection<BreakTable> Breaks { get; set; }

        public ObservableCollection<NoteTable> Notes { get; set; }

        public void EditShiftDetails(object obj)
        {
            BreakText = "Breaks";
            DriveText = "Drives";
            NoteText = "Notes";

            if (currentShift.Key != 0)
            {
                if (SelectedDrive > -1)
                {
                    Navigation.PushModalAsync(new EditShiftDetailsPage(obj.ToString(), Drives[SelectedDrive]));

                    driveList = dbService.GetDriveShifts(currentShift.Key);
                    Drives = new ObservableCollection<DriveTable>(driveList);

                    SelectedDrive = -1;

                    OnPropertyChanged("Drives");
                    OnPropertyChanged("SelectedDrive");
                }
                else if (SelectedBreak > -1)
                {
                    Navigation.PushModalAsync(new EditShiftDetailsPage(obj.ToString(), null, null, Breaks[SelectedBreak]));

                    breakList = dbService.GetBreaks(currentShift);
                    Breaks = new ObservableCollection<BreakTable>(breakList);

                    SelectedBreak = -1;

                    OnPropertyChanged("Breaks");
                    OnPropertyChanged("SelectedBreak");
                }
                else if (SelectedNote > -1)
                {
                    Navigation.PushModalAsync(new EditShiftDetailsPage(obj.ToString(), null, null, null, Notes[SelectedNote]));

                    noteList = dbService.GetNotes(currentShift.Key);
                    Notes = new ObservableCollection<NoteTable>(noteList);

                    SelectedNote = -1;

                    OnPropertyChanged("Notes");
                    OnPropertyChanged("SelectedNote");
                }
                else
                {
                    Navigation.PushModalAsync(new EditShiftDetailsPage(obj.ToString(), null, currentShift));
                }
            }
            else
            {
                UserDialogs.Instance.ConfirmAsync(Resource.SelectAShift, Resource.Alert, Resource.DisplayAlertOkay);
            }
        }

        internal void LoadInfoFromShift(ShiftTable shiftTable)
        {
            ShiftStartInfoVisible = true;
            if (shiftTable.EndLocation != null)
            {
                ShiftEndInfoVisible = true;
                ShiftEndDatePicker = DateTime.Parse(shiftTable.EndDate).Date;
                ShiftEndTimePicker = ShiftEndDatePicker.TimeOfDay;
            }

            currentShift = shiftTable;

            ShiftStartDatePicker = DateTime.Parse(shiftTable.StartDate).Date;

            ShiftStartTimePicker = DateTime.Parse(shiftTable.StartDate).TimeOfDay;

            driveList = dbService.GetDriveShifts(currentShift.Key);
            Drives = new ObservableCollection<DriveTable>(driveList);

            if (Drives.Count != 0)
            {
                DrivesAvailable = true;
            }

            breakList = dbService.GetBreaks(currentShift);
            Breaks = new ObservableCollection<BreakTable>(breakList);

            if (Breaks.Count != 0)
            {
                BreaksAvailable = true;
            }

            noteList = dbService.GetNotes(currentShift.Key);
            Notes = new ObservableCollection<NoteTable>(noteList);

            if (Notes.Count != 0)
            {
                NotesAvailable = true;
            }

            ShiftSelected = true;

            OnPropertyChanged("ShiftSelected");
            OnPropertyChanged("ShiftStartInfoVisible");
            OnPropertyChanged("ShiftEndInfoVisible");
            OnPropertyChanged("ShiftStartTimePicker");
            OnPropertyChanged("ShiftEndTimePicker");
            OnPropertyChanged("ShiftStartDatePicker");
            OnPropertyChanged("ShiftEndDatePicker");
            OnPropertyChanged("Drives");
            OnPropertyChanged("DrivesAvailable");
            OnPropertyChanged("Breaks");
            OnPropertyChanged("BreaksAvailable");
            OnPropertyChanged("Notes");
            OnPropertyChanged("NotesAvailable");
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Save()
        {
            if (currentShift.EndDate != "Current")
            {
                DateTime oldEndShiftDate = DateTime.Parse(currentShift.EndDate).Date;
                TimeSpan oldEndShiftTime = DateTime.Parse(currentShift.EndDate).TimeOfDay;

                if ((ShiftEndDatePicker != oldEndShiftDate) || (ShiftEndTimePicker != oldEndShiftTime))
                {
                    AmendmentTable newStopShift = new AmendmentTable()
                    {
                        Field = "EndDate",
                        ShiftId = currentShift.Key,
                        Table = "ShiftTable",
                        TimeStamp = DateTime.Now.ToString(Resource.DatabaseDateFormat),
                        BeforeValue = currentShift.EndDate,
                        AfterValue = (ShiftEndDatePicker + ShiftEndTimePicker).ToString(Resource.DatabaseDateFormat)
                    };
                    currentShift.EndDate = (ShiftEndDatePicker + ShiftEndTimePicker).ToString(Resource.DatabaseDateFormat);
                    listOfAmendments.Add(newStopShift);
                }
            }

            DateTime oldStartShiftDate = DateTime.Parse(currentShift.StartDate).Date;
            TimeSpan oldStartShiftTime = DateTime.Parse(currentShift.StartDate).TimeOfDay;

            if ((ShiftStartDatePicker != oldStartShiftDate) || (oldStartShiftTime != ShiftStartTimePicker))
            {
                AmendmentTable newStartShift = new AmendmentTable()
                {
                    Field = "StartTime",
                    ShiftId = currentShift.Key,
                    Table = "ShiftTable",
                    TimeStamp = DateTime.Now.ToString(Resource.DatabaseDateFormat),
                    BeforeValue = currentShift.StartDate,
                    AfterValue = (ShiftStartDatePicker + ShiftStartTimePicker).ToString(Resource.DatabaseDateFormat)
                };
                listOfAmendments.Add(newStartShift);
                currentShift.StartDate = (ShiftStartDatePicker + ShiftStartTimePicker).ToString(Resource.DatabaseDateFormat);
            }

            if (listOfAmendments.Count > 0)
            {
                dbService.AddAmendments(listOfAmendments, currentShift);
            }

            MessagingCenter.Send<string>("ShiftEdited", "ShiftEdited");
        }
    }
}