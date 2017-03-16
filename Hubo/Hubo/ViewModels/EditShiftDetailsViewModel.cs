// <copyright file="EditShiftDetailsViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using Acr.UserDialogs;
    using Xamarin.Forms;

    public class EditShiftDetailsViewModel : INotifyPropertyChanged
    {
        private readonly ShiftTable currentShift = new ShiftTable();
        private readonly DatabaseService dbService = new DatabaseService();

        private DriveTable currentDrive = new DriveTable();
        private List<AmendmentTable> listOfAmendments = new List<AmendmentTable>();
        private List<DriveTable> listUsedVehicles = new List<DriveTable>();
        private DriveTable currentVehicleInUse = new DriveTable();
        private BreakTable currentBreak = new BreakTable();
        private List<NoteTable> listOfNotes = new List<NoteTable>();
        private NoteTable currentNote = new NoteTable();

        public EditShiftDetailsViewModel(string instructionCommand, DriveTable drive = null, ShiftTable shift = null, BreakTable breakItem = null, NoteTable note = null)
        {
            DashText = Resource.Dash;
            VehicleStartHubo = string.Empty;
            VehicleEndHubo = string.Empty;
            currentDrive = drive;
            currentShift = shift;
            currentBreak = breakItem;
            currentNote = note;
            Instruction = instructionCommand;
            EditingVehicle = false;
            SaveText = Resource.Save;
            CancelText = Resource.Cancel;
            SaveCommand = new Command(Save);
            CancelCommand = new Command(Cancel);
            ShowSaveButton = false;
            EditingVehicle = false;
            EditingNote = false;
            EditingBreakList = false;
            SelectedBreak = -1;

            DisplayDetails();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<BreakTable> ListOfBreaks { get; set; }

        public string Instruction { get; set; }

        public INavigation Navigation { get; set; }

        public string VehicleStartHubo { get; set; }

        public string VehicleEndHubo { get; set; }

        public bool EditingVehicle { get; set; }

        public string SaveText { get; set; }

        public string CancelText { get; set; }

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public ICommand BreakCommand { get; set; }

        public bool EditingStartBreak { get; set; }

        public bool EditingEndBreak { get; set; }

        public string BreakStartLabel { get; set; }

        public string BreakStartNote { get; set; }

        public string BreakEndLabel { get; set; }

        public string BreakEndNote { get; set; }

        public string BreakStartLocation { get; set; }

        public string BreakEndLocation { get; set; }

        public TimeSpan BreakStartTime { get; set; }

        public TimeSpan BreakEndTime { get; set; }

        public DateTime BreakStartDate { get; set; }

        public DateTime BreakEndDate { get; set; }

        public bool StartNoteChanged { get; set; }

        public bool EndNoteChanged { get; set; }

        public bool BreakInfoChanged { get; set; }

        public bool ShowSaveButton { get; set; }

        public bool EditingNote { get; set; }

        public string NoteEntry { get; set; }

        public string LocationEntry { get; set; }

        public string HuboEntry { get; set; }

        public DateTime NoteDate { get; set; }

        public TimeSpan NoteTime { get; set; }

        public string VehicleStartLocation { get; set; }

        public string VehicleStartNote { get; set; }

        public string VehicleEndLocation { get; set; }

        public string VehicleEndNote { get; set; }

        public string HuboStartText { get; set; }

        public string HuboEndText { get; set; }

        public string StartLocationText { get; set; }

        public string EndLocationText { get; set; }

        public string StartNoteText { get; set; }

        public string EndNoteText { get; set; }

        public string StartTimeText { get; set; }

        public string EndTimeText { get; set; }

        public string NoteText { get; set; }

        public string LocationText { get; set; }

        public string HuboText { get; set; }

        public string DateText { get; set; }

        public bool EditingStartDrive { get; set; }

        public bool EditingEndDrive { get; set; }

        public string DashText { get; set; }

        public bool EditNote { get; set; }

        public bool EditingBreakList { get; set; }

        public DateTime DriveStartDate { get; private set; }

        public TimeSpan DriveStartTime { get; private set; }

        public DateTime DriveEndDate { get; private set; }

        public TimeSpan DriveEndTime { get; private set; }

        public int VehicleId { get; set; }

        public int SelectedIndex { get; set; }

        public string DriveStartHubo { get; set; }

        public string DriveEndHubo { get; set; }

        public string AddBreakText { get; set; }

        public int SelectedBreak { get; set; }

        internal List<VehicleTable> LoadVehicle()
        {
            return dbService.LoadVehicle();
        }

        internal void DisplayDetails(int selectedIndex = -1)
        {
            if (Instruction == "Breaks")
            {
                if (currentBreak != null)
                {
                    BreakStartLabel = Resource.StartLocation;
                    BreakStartDate = DateTime.Parse(currentBreak.StartDate).Date;
                    BreakStartTime = DateTime.Parse(currentBreak.StartDate).TimeOfDay;
                    BreakStartLocation = currentBreak.StartLocation;
                    StartTimeText = Resource.StartTime;

                    if (currentBreak.EndDate != null)
                    {
                        BreakEndDate = DateTime.Parse(currentBreak.EndDate).Date;
                        BreakEndTime = DateTime.Parse(currentBreak.EndDate).TimeOfDay;
                        BreakEndLocation = currentBreak.EndLocation;
                        EndTimeText = Resource.EndTime;
                        BreakEndLabel = Resource.EndLocation;
                        EditingEndBreak = true;
                    }
                }
                else
                {
                    BreakStartLabel = Resource.StartBreak;
                    BreakEndLabel = Resource.EndBreak;

                    StartTimeText = Resource.StartTime;
                    EndTimeText = Resource.EndTime;

                    BreakStartDate = DateTime.Now.Date;
                    BreakStartTime = DateTime.Now.TimeOfDay;
                    BreakEndDate = DateTime.Now.Date;
                    BreakEndTime = DateTime.Now.TimeOfDay;

                    EditingEndBreak = true;
                }

                EditingStartBreak = true;
                OnPropertyChanged("EditingStartBreak");
                OnPropertyChanged("EditingEndBreak");
                OnPropertyChanged("BreakStartDate");
                OnPropertyChanged("BreakStartTime");
                OnPropertyChanged("BreakEndDate");
                OnPropertyChanged("BreakEndTime");
                OnPropertyChanged("BreakStartLabel");
                OnPropertyChanged("BreakEndLabel");
                OnPropertyChanged("BreakStartLocation");
                OnPropertyChanged("BreakEndLocation");
                OnPropertyChanged("StartTimeText");
                OnPropertyChanged("EndTimeText");
            }

            if (Instruction == "Notes")
            {
                if (currentNote != null)
                {
                    NoteEntry = currentNote.Note;
                    EditingNote = true;

                    NoteDate = DateTime.Parse(currentNote.Date);
                    NoteTime = NoteDate.TimeOfDay;
                }
                else
                {
                    NoteDate = DateTime.Now;
                }

                NoteText = Resource.Note;
                LocationText = Resource.Location;
                HuboText = Resource.HuboEquals;
                DateText = Resource.DateEquals;

                OnPropertyChanged("NoteEntry");
                OnPropertyChanged("NoteDate");
                OnPropertyChanged("NoteTime");
                OnPropertyChanged("EditingNote");
                OnPropertyChanged("NoteText");
                OnPropertyChanged("LocationText");
                OnPropertyChanged("HuboText");
                OnPropertyChanged("DateText");
            }

            if (Instruction == "Drives")
            {
                if (currentDrive != null)
                {
                    DriveStartDate = DateTime.Parse(currentDrive.StartDate).Date;
                    DriveStartTime = DateTime.Parse(currentDrive.StartDate).TimeOfDay;
                    DriveStartHubo = currentDrive.StartHubo.ToString();

                    if (currentDrive.EndDate != null)
                    {
                        DriveEndDate = DateTime.Parse(currentDrive.EndDate).Date;
                        DriveEndTime = DateTime.Parse(currentDrive.EndDate).TimeOfDay;
                        DriveEndHubo = currentDrive.EndHubo.ToString();
                        EditingEndDrive = true;
                    }

                    VehicleId = currentDrive.VehicleKey;
                    StartTimeText = Resource.StartTime;
                    EndTimeText = Resource.EndTime;
                    HuboText = Resource.HuboEquals;
                    AddBreakText = Resource.AddBreak;
                }
                else
                {
                    DriveStartDate = DateTime.Now.Date;
                    DriveStartTime = DateTime.Now.TimeOfDay;
                    DriveEndDate = DateTime.Now.Date;
                    DriveEndTime = DateTime.Now.TimeOfDay;

                    StartTimeText = Resource.StartTime;
                    EndTimeText = Resource.EndTime;
                    HuboText = Resource.HuboEquals;
                    AddBreakText = Resource.AddBreak;

                    EditingEndDrive = true;
                }

                EditingStartDrive = true;
                OnPropertyChanged("DriveStartTime");
                OnPropertyChanged("DriveStartDate");
                OnPropertyChanged("DriveEndTime");
                OnPropertyChanged("DriveEndDate");
                OnPropertyChanged("DriveStartHubo");
                OnPropertyChanged("DriveEndHubo");
                OnPropertyChanged("StartTimeText");
                OnPropertyChanged("EndTimeText");
                OnPropertyChanged("HuboText");
                OnPropertyChanged("EditingStartDrive");
                OnPropertyChanged("EditingEndDrive");
                OnPropertyChanged("EditingBreakList");
            }

            ShowSaveButton = true;

            OnPropertyChanged("ShowSaveButton");
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
            if (Instruction == "Breaks")
            {
                if (currentBreak != null)
                {
                    DateTime oldStartBreakDate = DateTime.Parse(currentBreak.StartDate).Date;
                    DateTime oldEndBreakDate = DateTime.Parse(currentBreak.EndDate).Date;

                    TimeSpan oldStartBreakTime = DateTime.Parse(currentBreak.StartDate).TimeOfDay;
                    TimeSpan oldEndBreakTime = DateTime.Parse(currentBreak.EndDate).TimeOfDay;

                    if ((BreakStartDate.Date != oldStartBreakDate) || (BreakStartTime != oldStartBreakTime))
                    {
                        AmendmentTable newAmendment = new AmendmentTable()
                        {
                            Field = "StartDate",
                            ShiftId = currentShift.Key,
                            Table = "BreakTable",
                            TimeStamp = DateTime.Now.ToString(Resource.DatabaseDateFormat),
                            BeforeValue = currentBreak.StartDate,
                            AfterValue = (BreakStartDate + BreakStartTime).ToString(Resource.DatabaseDateFormat)
                        };
                        currentBreak.StartDate = (BreakStartDate + BreakStartTime).ToString(Resource.DatabaseDateFormat);
                        listOfAmendments.Add(newAmendment);
                    }

                    if ((BreakEndDate != oldEndBreakDate) || (BreakEndTime != oldEndBreakTime))
                    {
                        AmendmentTable newAmendment = new AmendmentTable()
                        {
                            Field = "EndDate",
                            Table = "BreakTable",
                            DriveId = currentDrive.Key,
                            TimeStamp = DateTime.Now.ToString(Resource.DatabaseDateFormat),
                            BeforeValue = currentBreak.EndDate,
                            AfterValue = (BreakEndDate + BreakEndTime).ToString(Resource.DatabaseDateFormat)
                        };
                        currentBreak.EndDate = (BreakEndDate + BreakEndTime).ToString(Resource.DatabaseDateFormat);
                        listOfAmendments.Add(newAmendment);
                    }

                    if (BreakStartLocation != currentBreak.StartLocation)
                    {
                        AmendmentTable newAmendment = new AmendmentTable()
                        {
                            Field = "StartLocation",
                            Table = "BreakTable",
                            DriveId = currentDrive.Key,
                            TimeStamp = DateTime.Now.ToString(Resource.DatabaseDateFormat),
                            BeforeValue = currentBreak.StartLocation,
                            AfterValue = BreakStartLocation
                        };
                        currentBreak.EndDate = BreakStartLocation;
                        listOfAmendments.Add(newAmendment);
                    }

                    if (BreakEndLocation != currentBreak.EndLocation)
                    {
                        AmendmentTable newAmendment = new AmendmentTable()
                        {
                            Field = "EndLocation",
                            Table = "BreakTable",
                            DriveId = currentDrive.Key,
                            TimeStamp = DateTime.Now.ToString(Resource.DatabaseDateFormat),
                            BeforeValue = currentBreak.EndLocation,
                            AfterValue = BreakEndLocation
                        };
                        currentBreak.EndDate = BreakEndLocation;
                        listOfAmendments.Add(newAmendment);
                    }

                    if (listOfAmendments.Count > 0)
                    {
                        dbService.AddAmendments(listOfAmendments, null, null, currentBreak);
                    }
                }
                else
                {
                    BreakTable newBreak = new BreakTable()
                    {
                        ShiftKey = currentDrive.Key,
                        ActiveBreak = false,
                        StartDate = (BreakStartDate + BreakStartTime).ToString(Resource.DatabaseDateFormat),
                        EndDate = (BreakEndDate + BreakEndTime).ToString(Resource.DatabaseDateFormat),
                        StartLocation = BreakStartLocation,
                        EndLocation = BreakEndLocation
                    };
                    dbService.SaveBreak(newBreak);
                }

                Navigation.PopModalAsync();
            }
            else if (Instruction == "Notes")
            {
                if (currentNote != null)
                {
                    if (NoteEntry != currentNote.Note)
                    {
                        AmendmentTable newAmendment = new AmendmentTable()
                        {
                            Field = "Note",
                            ShiftId = currentShift.Key,
                            Table = "NoteTable",
                            TimeStamp = DateTime.Now.ToString(Resource.DatabaseDateFormat),
                            BeforeValue = currentNote.Note,
                            AfterValue = NoteEntry
                        };
                        currentNote.Note = NoteEntry;
                        listOfAmendments.Add(newAmendment);
                    }

                    DateTime oldDate = DateTime.Parse(currentNote.Date).Date;
                    TimeSpan oldTime = DateTime.Parse(currentNote.Date).TimeOfDay;

                    if ((NoteDate != oldDate) || (NoteTime != oldTime))
                    {
                        AmendmentTable newAmendment = new AmendmentTable()
                        {
                            Field = "Date",
                            ShiftId = currentShift.Key,
                            Table = "NoteTable",
                            TimeStamp = DateTime.Now.ToString(Resource.DatabaseDateFormat),
                            BeforeValue = currentNote.Date,
                            AfterValue = (NoteDate + NoteTime).ToString(Resource.DatabaseDateFormat)
                        };
                        currentNote.Date = (NoteDate + NoteTime).ToString(Resource.DatabaseDateFormat);
                        listOfAmendments.Add(newAmendment);
                    }

                    if (listOfAmendments.Count > 0)
                    {
                        dbService.AddAmendments(listOfAmendments, null, null, null, currentNote);
                    }
                }
                else
                {
                    NoteTable newNote = new NoteTable()
                    {
                        Date = (NoteDate + NoteTime).ToString(Resource.DatabaseDateFormat),
                        Note = NoteEntry,
                        ShiftKey = currentShift.Key
                    };
                    dbService.InsertNote(newNote);
                }

                Navigation.PopModalAsync();
            }
            else if (Instruction == "Drives")
            {
                if (CheckValidHuboEntry())
                {
                    if (currentDrive != null)
                    {
                        DateTime oldStartDriveDate = DateTime.Parse(currentDrive.StartDate).Date;
                        DateTime oldEndDriveDate = DateTime.Parse(currentDrive.EndDate).Date;

                        TimeSpan oldStartDriveTime = DateTime.Parse(currentDrive.StartDate).TimeOfDay;
                        TimeSpan oldEndDriveTime = DateTime.Parse(currentDrive.EndDate).TimeOfDay;

                        if ((DriveStartDate.Date != oldStartDriveDate) || (DriveStartTime != oldStartDriveTime))
                        {
                            AmendmentTable newAmendment = new AmendmentTable()
                            {
                                Field = "StartDate",
                                DriveId = currentDrive.Key,
                                Table = "DriveTable",
                                TimeStamp = DateTime.Now.ToString(Resource.DatabaseDateFormat),
                                BeforeValue = currentDrive.StartDate,
                                AfterValue = (DriveStartDate + DriveStartTime).ToString(Resource.DatabaseDateFormat)
                            };
                            currentDrive.StartDate = (DriveStartDate + DriveStartTime).ToString(Resource.DatabaseDateFormat);
                            listOfAmendments.Add(newAmendment);
                        }

                        if ((DriveEndDate != oldEndDriveDate) || (DriveEndTime != oldEndDriveTime))
                        {
                            AmendmentTable newAmendment = new AmendmentTable()
                            {
                                Field = "EndDate",
                                Table = "DriveTable",
                                DriveId = currentDrive.Key,
                                TimeStamp = DateTime.Now.ToString(Resource.DatabaseDateFormat),
                                BeforeValue = currentDrive.EndDate,
                                AfterValue = (DriveEndDate + DriveEndTime).ToString(Resource.DatabaseDateFormat)
                            };
                            currentDrive.EndDate = (DriveEndDate + DriveEndTime).ToString(Resource.DatabaseDateFormat);
                            listOfAmendments.Add(newAmendment);
                        }

                        if ((VehicleId - 1) != SelectedIndex)
                        {
                            AmendmentTable newAmendment = new AmendmentTable()
                            {
                                Field = "VehicleKey",
                                Table = "DriveTable",
                                DriveId = currentDrive.Key,
                                TimeStamp = DateTime.Now.ToString(Resource.DatabaseDateFormat),
                                BeforeValue = VehicleId.ToString(),
                                AfterValue = (SelectedIndex + 1).ToString()
                            };
                            currentDrive.StartHubo = SelectedIndex + 1;
                            listOfAmendments.Add(newAmendment);
                        }

                        if (int.Parse(DriveStartHubo) != currentDrive.StartHubo)
                        {
                            AmendmentTable newAmendment = new AmendmentTable()
                            {
                                Field = "StartHubo",
                                Table = "DriveTable",
                                DriveId = currentDrive.Key,
                                TimeStamp = DateTime.Now.ToString(Resource.DatabaseDateFormat),
                                BeforeValue = currentDrive.StartHubo.ToString(),
                                AfterValue = DriveStartHubo.ToString()
                            };
                            currentDrive.StartHubo = int.Parse(DriveStartHubo);
                            listOfAmendments.Add(newAmendment);
                        }

                        if (int.Parse(DriveEndHubo) != currentDrive.EndHubo)
                        {
                            AmendmentTable newAmendment = new AmendmentTable()
                            {
                                Field = "EndHubo",
                                Table = "DriveTable",
                                DriveId = currentDrive.Key,
                                TimeStamp = DateTime.Now.ToString(Resource.DatabaseDateFormat),
                                BeforeValue = currentDrive.EndHubo.ToString(),
                                AfterValue = DriveEndHubo.ToString()
                            };
                            currentDrive.EndHubo = int.Parse(DriveEndHubo);
                            listOfAmendments.Add(newAmendment);
                        }

                        if (listOfAmendments.Count > 0)
                        {
                            dbService.AddAmendments(listOfAmendments, null, currentDrive);
                        }
                    }
                    else
                    {
                        DriveTable newDrive = new DriveTable()
                        {
                            ShiftKey = currentShift.Key,
                            ActiveVehicle = false,
                            VehicleKey = SelectedIndex + 1,
                            StartDate = (DriveStartDate + DriveStartTime).ToString(Resource.DatabaseDateFormat),
                            EndDate = (DriveEndDate + DriveEndTime).ToString(Resource.DatabaseDateFormat),
                            StartHubo = int.Parse(DriveStartHubo),
                            EndHubo = int.Parse(DriveEndHubo)
                        };
                        dbService.InsertDrive(newDrive);
                    }

                    Navigation.PopModalAsync();
                }
            }
        }

        private void Cancel()
        {
            Navigation.PopModalAsync();
        }

        private bool CheckValidHuboEntry()
        {
            Regex regex = new Regex("^[0-9]+$");
            if ((DriveStartHubo.Length == 0) || (DriveEndHubo.Length == 0))
            {
                UserDialogs.Instance.ConfirmAsync(Resource.InvalidHubo, Resource.Alert, Resource.DisplayAlertOkay);
                return false;
            }

            if (!regex.IsMatch(DriveStartHubo) || !regex.IsMatch(DriveEndHubo))
            {
                UserDialogs.Instance.ConfirmAsync(Resource.InvalidHubo, Resource.Alert, Resource.DisplayAlertOkay);
                return false;
            }

            return true;
        }
    }
}