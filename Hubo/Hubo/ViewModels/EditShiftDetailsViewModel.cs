using System;
using System.Collections.Generic;
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

        public event PropertyChangedEventHandler PropertyChanged;



        ShiftTable currentShift = new ShiftTable();
        DriveTable currentDrive = new DriveTable();
        DatabaseService DbService = new DatabaseService();


        List<DriveTable> listUsedVehicles = new List<DriveTable>();
        DriveTable currentVehicleInUse = new DriveTable();

        List<VehicleTable> listOfRegisteredVehicles = new List<VehicleTable>();
        VehicleTable currentRegisteredVehicle = new VehicleTable();

        List<BreakTable> listOfBreaks = new List<BreakTable>();
        BreakTable currentBreak = new BreakTable();

        List<NoteTable> listOfNotes = new List<NoteTable>();
        NoteTable currentStartNote = new NoteTable();
        NoteTable currentEndNote = new NoteTable();
        NoteTable currentNote = new NoteTable();

        List<AmendmentTable> listOfAmendments = new List<AmendmentTable>();

        public EditShiftDetailsViewModel(string instructionCommand, ShiftTable shift = null, DriveTable drive = null)
        {
            VehicleStartHubo = "";
            VehicleEndHubo = "";
            currentShift = shift;
            currentDrive = drive;
            instruction = instructionCommand;
            EditingVehicle = false;
            SaveText = Resource.Save;
            CancelText = Resource.Cancel;
            SaveCommand = new Command(Save);
            CancelCommand = new Command(Cancel);
            ShowSaveButton = false;
            EditingVehicle = false;
            EditingNote = false;
        }

        private void Cancel()
        {
            Navigation.PopAsync();
        }

        private void Save()
        {
            //if (instruction == "Breaks")
            //{
            //    if (CheckValidHuboEntry())
            //    {

            //        DateTime oldStartBreakDate = DateTime.Parse(currentBreak.StartDate).Date;
            //        DateTime oldEndBreakDate = DateTime.Parse(currentBreak.EndDate).Date;

            //        TimeSpan oldStartBreakTime = DateTime.Parse(currentBreak.StartDate).TimeOfDay;
            //        TimeSpan oldEndBreakTime = DateTime.Parse(currentBreak.EndDate).TimeOfDay;

            //        if ((BreakStartDate.Date != oldStartBreakDate) || (BreakStartTime != oldStartBreakTime))
            //        {
            //            AmendmentTable newAmendment = new AmendmentTable();
            //            newAmendment.Field = "StartDate";
            //            newAmendment.DriveId = currentDrive.Key;
            //            newAmendment.Table = "BreakTable";
            //            newAmendment.TimeStamp = DateTime.Now.ToString();
            //            newAmendment.BeforeValue = currentBreak.StartDate;
            //            newAmendment.AfterValue = (BreakStartDate + BreakStartTime).ToString();
            //            currentBreak.StartDate = (BreakStartDate + BreakStartTime).ToString();
            //            listOfAmendments.Add(newAmendment);
            //        }

            //        if ((BreakEndDate != oldEndBreakDate) || (BreakEndTime != oldEndBreakTime))
            //        {
            //            AmendmentTable newAmendment = new AmendmentTable();
            //            newAmendment.Field = "EndDate";
            //            newAmendment.Table = "BreakTable";
            //            newAmendment.DriveId = currentDrive.Key;
            //            newAmendment.TimeStamp = DateTime.Now.ToString();
            //            listOfAmendments.Add(newAmendment);
            //            newAmendment.BeforeValue = currentBreak.EndDate;
            //            newAmendment.AfterValue = (BreakEndDate + BreakEndTime).ToString();
            //            currentBreak.EndDate = (BreakEndDate + BreakEndTime).ToString();
            //        }

            //        if (listOfAmendments.Count > 0)
            //        {
            //            DbService.AddAmendments(listOfAmendments, null, currentDrive, currentBreak);
            //        }
            //        Navigation.PopAsync();
            //    }
            //}
            //else if (instruction == "Notes")
            //{
            //    Regex regex = new Regex("^[0-9]+$");
            //    if (regex.IsMatch(HuboEntry))
            //    {
            //        if (NoteEntry != currentNote.Note)
            //        {
            //            AmendmentTable newAmendment = new AmendmentTable();
            //            newAmendment.Field = "Note";
            //            newAmendment.ShiftId = currentShift.Key;
            //            newAmendment.Table = "NoteTable";
            //            newAmendment.TimeStamp = DateTime.Now.ToString();
            //            newAmendment.BeforeValue = currentNote.Note;
            //            newAmendment.AfterValue = NoteEntry;
            //            currentNote.Note = NoteEntry;
            //            listOfAmendments.Add(newAmendment);
            //        }

            //        DateTime oldDate = DateTime.Parse(currentNote.Date).Date;
            //        TimeSpan oldTime = DateTime.Parse(currentNote.Date).TimeOfDay;


            //        if ((NoteDate != oldDate) || (NoteTime != oldTime))
            //        {
            //            AmendmentTable newAmendment = new AmendmentTable();
            //            newAmendment.Field = "Date";
            //            newAmendment.ShiftId = currentShift.Key;
            //            newAmendment.Table = "NoteTable";
            //            newAmendment.TimeStamp = DateTime.Now.ToString();
            //            newAmendment.BeforeValue = currentNote.Date;
            //            newAmendment.AfterValue = (NoteDate + NoteTime).ToString();
            //            currentNote.Date = (NoteDate + NoteTime).ToString();
            //            listOfAmendments.Add(newAmendment);
            //        }

            //        if (listOfAmendments.Count > 0)
            //        {
            //            DbService.AddAmendments(listOfAmendments, currentShift, null, null, currentNote);
            //        }
            //    }
            //    else
            //    {
            //        Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.InvalidHubo, Resource.DisplayAlertOkay);
            //    }
            //}
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
            //            newAmendment.TimeStamp = DateTime.Now.ToString();
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
            //            newAmendment.TimeStamp = DateTime.Now.ToString();
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
        }

        private bool CheckValidHuboEntry()
        {
            Regex regex = new Regex("^[0-9]+$");
            if ((VehicleStartHubo.Length == 0) || (VehicleEndHubo.Length == 0))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.InvalidHubo, Resource.DisplayAlertOkay);
                return false;
            }
            if (!(regex.IsMatch(VehicleStartHubo)) || !(regex.IsMatch(VehicleEndHubo)))
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


        internal List<BreakTable> LoadBreaks()
        {
            listOfBreaks = DbService.GetBreaks(currentDrive);
            return listOfBreaks;
        }

        internal List<DriveTable> LoadVehicles()
        {
            listUsedVehicles = DbService.GetUsedVehicles(currentShift);
            return listUsedVehicles;
        }

        internal VehicleTable LoadVehicleInfo(DriveTable vehicle)
        {
            return DbService.LoadVehicleInfo(vehicle);
        }

        internal void DisplayDetails(int selectedIndex)
        {

            if (instruction == "Breaks")
            {
                BreakStartLabel = Resource.StartBreak;
                BreakEndLabel = Resource.EndBreak;
                //currentBreak = listOfBreaks[selectedIndex];

                BreakStartDate = DateTime.Parse("2016-10-21 10:22").Date;//DateTime.Parse(currentBreak.StartDate).Date;
                BreakStartTime = DateTime.Parse("2016-10-21 10:22").TimeOfDay;//DateTime.Parse(currentBreak.StartDate).TimeOfDay;
                BreakEndDate = DateTime.Parse("2016-10-21 10:22").Date;//DateTime.Parse(currentBreak.EndDate).Date;
                BreakEndTime = DateTime.Parse("2016-10-21 14:32").TimeOfDay;//DateTime.Parse(currentBreak.EndDate).TimeOfDay;
                BreakStartLocation = "Auckland";//currentBreak.StartLocation;
                BreakEndLocation = "Wellington";//currentBreak.EndLocation;

                StartTimeText = Resource.StartTime;
                EndTimeText = Resource.EndTime;

                EditingStartBreak = true;
                //if (currentBreak.EndDate != null)
                //{
                    EditingEndBreak = true;
                //}
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

                NoteDate = DateTime.Parse("2016-10-21 12:22").Date;//DateTime.Parse(currentNote.Date);
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
                VehicleStartHubo = "125405";//currentVehicleInUse.StartHubo.ToString();
                VehicleEndHubo = "127022";//currentVehicleInUse.EndHubo.ToString();

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
