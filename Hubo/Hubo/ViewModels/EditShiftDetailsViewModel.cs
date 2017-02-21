using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class EditShiftDetailsViewModel : INotifyPropertyChanged
    {
        public string instruction { get; set; }
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

        public event PropertyChangedEventHandler PropertyChanged;



        ShiftTable currentShift = new ShiftTable();
        DriveTable currentDrive = new DriveTable();
        DatabaseService DbService = new DatabaseService();


        List<DriveTable> listUsedVehicles = new List<DriveTable>();
        DriveTable currentVehicleInUse = new DriveTable();

        List<VehicleTable> listOfRegisteredVehicles = new List<VehicleTable>();
        VehicleTable currentRegisteredVehicle = new VehicleTable();

        public ObservableCollection<BreakTable> listOfBreaks { get; set; }
        BreakTable currentBreak = new BreakTable();

        List<NoteTable> listOfNotes = new List<NoteTable>();
        NoteTable currentStartNote = new NoteTable();
        NoteTable currentEndNote = new NoteTable();
        NoteTable currentNote = new NoteTable();

        List<AmendmentTable> listOfAmendments = new List<AmendmentTable>();

        public EditShiftDetailsViewModel(string instructionCommand, DriveTable drive = null, ShiftTable shift = null, BreakTable breakItem = null)
        {
            DashText = Resource.Dash;
            VehicleStartHubo = "";
            VehicleEndHubo = "";
            currentDrive = drive;
            currentShift = shift;
            currentBreak = breakItem;
            instruction = instructionCommand;
            EditingVehicle = false;
            SaveText = Resource.Save;
            CancelText = Resource.Cancel;
            SaveCommand = new Command(Save);
            CancelCommand = new Command(Cancel);
            BreakCommand = new Command(EditBreakDetails);
            ShowSaveButton = false;
            EditingVehicle = false;
            EditingNote = false;
            EditingBreakList = false;
            SelectedBreak = -1;

            if (instruction == "Drives")
                DisplayDetails();
            else if (instruction == "Breaks")
                DisplayDetails();

            if (instruction == "Notes")
                EditNote = true;
            else
                EditNote = false;
        }

        private void Cancel()
        {
            Navigation.PopModalAsync();
        }

        public void EditBreakDetails(object obj)
        {
            if (SelectedBreak > -1)
            {
                Navigation.PushModalAsync(new EditShiftDetailsPage(obj.ToString(), null, null, listOfBreaks[SelectedBreak]));

                listOfBreaks = LoadBreaks();

                SelectedBreak = -1;

                OnPropertyChanged("listOfBreaks");
                OnPropertyChanged("SelectedBreak");
            }
            else if (SelectedBreak == -1)
                Navigation.PushModalAsync(new EditShiftDetailsPage(obj.ToString(), currentDrive));
        }

        private void Save()
        {
            if (instruction == "Breaks")
            {
                if (currentBreak != null)
                {
                    DateTime oldStartBreakDate = DateTime.Parse(currentBreak.StartDate).Date;
                    DateTime oldEndBreakDate = DateTime.Parse(currentBreak.EndDate).Date;

                    TimeSpan oldStartBreakTime = DateTime.Parse(currentBreak.StartDate).TimeOfDay;
                    TimeSpan oldEndBreakTime = DateTime.Parse(currentBreak.EndDate).TimeOfDay;

                    if ((BreakStartDate.Date != oldStartBreakDate) || (BreakStartTime != oldStartBreakTime))
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.Field = "StartDate";
                        newAmendment.DriveId = currentDrive.Key;
                        newAmendment.Table = "BreakTable";
                        newAmendment.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        newAmendment.BeforeValue = currentBreak.StartDate;
                        newAmendment.AfterValue = (BreakStartDate + BreakStartTime).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        currentBreak.StartDate = (BreakStartDate + BreakStartTime).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        listOfAmendments.Add(newAmendment);
                    }

                    if ((BreakEndDate != oldEndBreakDate) || (BreakEndTime != oldEndBreakTime))
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.Field = "EndDate";
                        newAmendment.Table = "BreakTable";
                        newAmendment.DriveId = currentDrive.Key;
                        newAmendment.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        newAmendment.BeforeValue = currentBreak.EndDate;
                        newAmendment.AfterValue = (BreakEndDate + BreakEndTime).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        currentBreak.EndDate = (BreakEndDate + BreakEndTime).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        listOfAmendments.Add(newAmendment);
                    }

                    if (BreakStartLocation != currentBreak.StartLocation)
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.Field = "StartLocation";
                        newAmendment.Table = "BreakTable";
                        newAmendment.DriveId = currentDrive.Key;
                        newAmendment.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        newAmendment.BeforeValue = currentBreak.StartLocation;
                        newAmendment.AfterValue = BreakStartLocation;
                        currentBreak.EndDate = BreakStartLocation;
                        listOfAmendments.Add(newAmendment);
                    }

                    if (BreakEndLocation != currentBreak.EndLocation)
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.Field = "EndLocation";
                        newAmendment.Table = "BreakTable";
                        newAmendment.DriveId = currentDrive.Key;
                        newAmendment.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        newAmendment.BeforeValue = currentBreak.EndLocation;
                        newAmendment.AfterValue = BreakEndLocation;
                        currentBreak.EndDate = BreakEndLocation;
                        listOfAmendments.Add(newAmendment);
                    }

                    if (listOfAmendments.Count > 0)
                    {
                        DbService.AddAmendments(listOfAmendments, null, null, currentBreak);
                    }
                }
                else
                {
                    BreakTable newBreak = new BreakTable();
                    newBreak.DriveKey = currentDrive.Key;
                    newBreak.ActiveBreak = false;
                    newBreak.StartDate = (BreakStartDate + BreakStartTime).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    newBreak.EndDate = (BreakEndDate + BreakEndTime).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    newBreak.StartLocation = BreakStartLocation;
                    newBreak.EndLocation = BreakEndLocation;

                    DbService.SaveBreak(newBreak);
                }

                Navigation.PopModalAsync();
            }
            else if (instruction == "Notes")
            {
                if (NoteEntry != currentNote.Note)
                {
                    AmendmentTable newAmendment = new AmendmentTable();
                    newAmendment.Field = "Note";
                    newAmendment.ShiftId = currentShift.Key;
                    newAmendment.Table = "NoteTable";
                    newAmendment.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    newAmendment.BeforeValue = currentNote.Note;
                    newAmendment.AfterValue = NoteEntry;
                    currentNote.Note = NoteEntry;
                    listOfAmendments.Add(newAmendment);
                }

                DateTime oldDate = DateTime.Parse(currentNote.Date).Date;
                TimeSpan oldTime = DateTime.Parse(currentNote.Date).TimeOfDay;


                if ((NoteDate != oldDate) || (NoteTime != oldTime))
                {
                    AmendmentTable newAmendment = new AmendmentTable();
                    newAmendment.Field = "Date";
                    newAmendment.ShiftId = currentShift.Key;
                    newAmendment.Table = "NoteTable";
                    newAmendment.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    newAmendment.BeforeValue = currentNote.Date;
                    newAmendment.AfterValue = (NoteDate + NoteTime).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    currentNote.Date = (NoteDate + NoteTime).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    listOfAmendments.Add(newAmendment);
                }

                if (listOfAmendments.Count > 0)
                {
                    DbService.AddAmendments(listOfAmendments, null, null, null, currentNote);
                }
                Navigation.PopModalAsync();
            }
            //else if (instruction == "Vehicles")
            //{
            //    if (CheckValidHuboEntry() && EditingVehicle)
            //    {
            //        if (VehicleStartHubo != currentVehicleInUse.StartHubo.ToString())
            //        {
            //            AmendmentTable newAmendment = new AmendmentTable();
            //            newAmendment.Field = "StartHubo";
            //            newAmendment.DriveId = currentDrive.Key;
            //            newAmendment.Table = "DriveTable";
            //            newAmendment.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            //            newAmendment.BeforeValue = currentVehicleInUse.StartHubo.ToString();
            //            newAmendment.AfterValue = VehicleStartHubo;
            //            currentVehicleInUse.StartHubo = int.Parse(VehicleStartHubo);
            //            listOfAmendments.Add(newAmendment);
            //        }
            //        if (VehicleEndHubo != currentVehicleInUse.EndHubo.ToString())
            //        {
            //            AmendmentTable newAmendment = new AmendmentTable();
            //            newAmendment.Field = "EndHubo";
            //            newAmendment.DriveId = currentDrive.Key;
            //            newAmendment.Table = "DriveTable";
            //            newAmendment.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            //            newAmendment.BeforeValue = currentVehicleInUse.EndHubo.ToString();
            //            newAmendment.AfterValue = VehicleEndHubo;
            //            currentVehicleInUse.EndHubo = int.Parse(VehicleEndHubo);
            //            listOfAmendments.Add(newAmendment);
            //        }

            //        if (listOfAmendments.Count > 0)
            //        {
            //            DbService.AddAmendments(listOfAmendments, null, currentVehicleInUse);
            //        }
            //        Navigation.PopAsync();
            //    }
            //}
            else if (instruction == "Drives")
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
                            AmendmentTable newAmendment = new AmendmentTable();
                            newAmendment.Field = "StartDate";
                            newAmendment.DriveId = currentDrive.Key;
                            newAmendment.Table = "DriveTable";
                            newAmendment.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            newAmendment.BeforeValue = currentDrive.StartDate;
                            newAmendment.AfterValue = (DriveStartDate + DriveStartTime).ToString("yyyy-MM-dd HH:mm:ss.fff");
                            currentDrive.StartDate = (DriveStartDate + DriveStartTime).ToString("yyyy-MM-dd HH:mm:ss.fff");
                            listOfAmendments.Add(newAmendment);
                        }

                        if ((DriveEndDate != oldEndDriveDate) || (DriveEndTime != oldEndDriveTime))
                        {
                            AmendmentTable newAmendment = new AmendmentTable();
                            newAmendment.Field = "EndDate";
                            newAmendment.Table = "DriveTable";
                            newAmendment.DriveId = currentDrive.Key;
                            newAmendment.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            newAmendment.BeforeValue = currentDrive.EndDate;
                            newAmendment.AfterValue = (DriveEndDate + DriveEndTime).ToString("yyyy-MM-dd HH:mm:ss.fff");
                            currentDrive.EndDate = (DriveEndDate + DriveEndTime).ToString("yyyy-MM-dd HH:mm:ss.fff");
                            listOfAmendments.Add(newAmendment);
                        }

                        if ((VehicleId - 1) != SelectedIndex)
                        {
                            AmendmentTable newAmendment = new AmendmentTable();
                            newAmendment.Field = "VehicleKey";
                            newAmendment.Table = "DriveTable";
                            newAmendment.DriveId = currentDrive.Key;
                            newAmendment.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            newAmendment.BeforeValue = VehicleId.ToString();
                            newAmendment.AfterValue = (SelectedIndex + 1).ToString();
                            currentDrive.StartHubo = (SelectedIndex + 1);
                            listOfAmendments.Add(newAmendment);
                        }

                        if (int.Parse(DriveStartHubo) != currentDrive.StartHubo)
                        {
                            AmendmentTable newAmendment = new AmendmentTable();
                            newAmendment.Field = "StartHubo";
                            newAmendment.Table = "DriveTable";
                            newAmendment.DriveId = currentDrive.Key;
                            newAmendment.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            newAmendment.BeforeValue = currentDrive.StartHubo.ToString();
                            newAmendment.AfterValue = DriveStartHubo.ToString();
                            currentDrive.StartHubo = int.Parse(DriveStartHubo);
                            listOfAmendments.Add(newAmendment);
                        }

                        if (int.Parse(DriveEndHubo) != currentDrive.EndHubo)
                        {
                            AmendmentTable newAmendment = new AmendmentTable();
                            newAmendment.Field = "EndHubo";
                            newAmendment.Table = "DriveTable";
                            newAmendment.DriveId = currentDrive.Key;
                            newAmendment.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            newAmendment.BeforeValue = currentDrive.EndHubo.ToString();
                            newAmendment.AfterValue = DriveEndHubo.ToString();
                            currentDrive.EndHubo = int.Parse(DriveEndHubo);
                            listOfAmendments.Add(newAmendment);
                        }

                        if (listOfAmendments.Count > 0)
                        {
                            DbService.AddAmendments(listOfAmendments, null, currentDrive);
                        }
                    }
                    else
                    {
                        DriveTable newDrive = new DriveTable();
                        newDrive.ShiftKey = currentShift.Key;
                        newDrive.ActiveVehicle = false;
                        newDrive.VehicleKey = SelectedIndex + 1;
                        newDrive.StartDate = (DriveStartDate + DriveStartTime).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        newDrive.EndDate = (DriveEndDate + DriveEndTime).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        newDrive.StartHubo = int.Parse(DriveStartHubo);
                        newDrive.EndHubo = int.Parse(DriveEndHubo);

                        DbService.InsertDrive(newDrive);
                    }
                    Navigation.PopModalAsync();
                }
            }
        }

        private bool CheckValidHuboEntry()
        {
            Regex regex = new Regex("^[0-9]+$");
            if ((DriveStartHubo.Length == 0) || (DriveEndHubo.Length == 0))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.InvalidHubo, Resource.DisplayAlertOkay);
                return false;
            }
            if (!(regex.IsMatch(DriveStartHubo)) || !(regex.IsMatch(DriveEndHubo)))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.InvalidHubo, Resource.DisplayAlertOkay);
                return false;
            }

            return true;
        }

        internal List<NoteTable> LoadNotes()
        {
            listOfNotes = DbService.GetNotes();
            return listOfNotes;
        }


        internal ObservableCollection<BreakTable> LoadBreaks()
        {
            List<BreakTable> breaks = new List<BreakTable>();
            breaks = DbService.GetBreaks(currentDrive);
            listOfBreaks = new ObservableCollection<BreakTable>(breaks);
            return listOfBreaks;
        }

        internal List<DriveTable> LoadDrives()
        {
            listUsedVehicles = DbService.GetDriveShifts(currentShift.Key);
            return listUsedVehicles;
        }

        internal List<VehicleTable> LoadVehicle()
        {
            return DbService.LoadVehicle();
        }

        internal void DisplayDetails(int selectedIndex = -1)
        {
            if (instruction == "Breaks")
            {
                if (selectedIndex > -1)
                {
                    BreakStartLabel = Resource.StartBreak;
                    BreakEndLabel = Resource.EndBreak;
                    currentBreak = listOfBreaks[selectedIndex];

                    BreakStartDate = DateTime.Parse(currentBreak.StartDate).Date;//DateTime.Parse("2016-10-21 10:22").Date;
                    BreakStartTime = DateTime.Parse(currentBreak.StartDate).TimeOfDay;//DateTime.Parse("2016-10-21 10:22").TimeOfDay;
                    BreakEndDate = DateTime.Parse(currentBreak.EndDate).Date;//DateTime.Parse("2016-10-21 14:32").Date;
                    BreakEndTime = DateTime.Parse(currentBreak.EndDate).TimeOfDay;//DateTime.Parse("2016-10-21 14:32").TimeOfDay;
                    BreakStartLocation = currentBreak.StartLocation;//"Auckland";
                    BreakEndLocation = currentBreak.EndLocation;//"Wellington";

                    StartTimeText = Resource.StartTime;
                    EndTimeText = Resource.EndTime;

                    if (currentBreak.EndDate != null)
                    {
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

            if (instruction == "Notes")
            {
                currentNote = listOfNotes[selectedIndex];
                NoteEntry = currentNote.Note;
                EditingNote = true;

                NoteDate = DateTime.Parse(currentNote.Date);//DateTime.Parse("2016-10-21 12:22").Date;
                NoteTime = NoteDate.TimeOfDay;

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

            if (instruction == "Vehicles")
            {
                currentVehicleInUse = listUsedVehicles[selectedIndex];
                VehicleStartHubo = currentVehicleInUse.StartHubo.ToString();//"125405";
                VehicleEndHubo = currentVehicleInUse.EndHubo.ToString();//"127022";

                HuboStartText = Resource.HuboStart;
                HuboEndText = Resource.HuboEnd;
                StartLocationText = Resource.StartLocation;
                EndLocationText = Resource.EndLocation;


                EditingVehicle = true;
                OnPropertyChanged("EditingVehicle");
                OnPropertyChanged("VehicleEndLocation");
                OnPropertyChanged("VehicleStartLocation");
                OnPropertyChanged("HuboStartText");
                OnPropertyChanged("HuboEndText");
                OnPropertyChanged("StartLocationText");
                OnPropertyChanged("EndLocationText");
                OnPropertyChanged("VehicleStartHubo");
                OnPropertyChanged("VehicleEndHubo");
            }
            if (instruction == "Drives")
            {
                if (currentDrive != null)
                {
                    DriveStartDate = DateTime.Parse(currentDrive.StartDate).Date;//DateTime.Parse("2016-10-21 10:22").Date;
                    DriveStartTime = DateTime.Parse(currentDrive.StartDate).TimeOfDay;//DateTime.Parse("2016-10-21 10:22").TimeOfDay;
                    DriveEndDate = DateTime.Parse(currentDrive.EndDate).Date;//DateTime.Parse("2016-10-21 14:32").Date;
                    DriveEndTime = DateTime.Parse(currentDrive.EndDate).TimeOfDay;//DateTime.Parse("2016-10-21 14:32").TimeOfDay;
                    DriveStartHubo = currentDrive.StartHubo.ToString();//"132135"
                    DriveEndHubo = currentDrive.EndHubo.ToString();//"135476"
                    VehicleId = currentDrive.VehicleKey;//2;

                    StartTimeText = Resource.StartTime;
                    EndTimeText = Resource.EndTime;
                    HuboText = Resource.HuboEquals;
                    AddBreakText = Resource.AddBreak;

                    listOfBreaks = LoadBreaks();
                    EditingBreakList = true;

                    if (currentDrive.EndDate != null)
                    {
                        EditingEndDrive = true;
                    }
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
    }
}