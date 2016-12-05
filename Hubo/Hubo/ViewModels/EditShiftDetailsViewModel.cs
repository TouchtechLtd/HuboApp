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
        public int instruction { get; set; }
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
        DatabaseService DbService = new DatabaseService();


        List<VehicleInUseTable> listUsedVehicles = new List<VehicleInUseTable>();
        VehicleInUseTable currentVehicleInUse = new VehicleInUseTable();

        List<VehicleTable> listOfRegisteredVehicles = new List<VehicleTable>();
        VehicleTable currentRegisteredVehicle = new VehicleTable();

        List<BreakTable> listOfBreaks = new List<BreakTable>();
        BreakTable currentBreak = new BreakTable();

        List<NoteTable> listOfNotes = new List<NoteTable>();
        NoteTable currentStartNote = new NoteTable();
        NoteTable currentEndNote = new NoteTable();
        NoteTable currentNote = new NoteTable();

        List<AmendmentTable> listOfAmendments = new List<AmendmentTable>();
        
        public EditShiftDetailsViewModel(int instructionNo, ShiftTable shift)
        {
            VehicleStartHubo = "";
            VehicleEndHubo = "";
            currentShift = shift;
            instruction = instructionNo;
            EditingVehicle = false;
            SaveText = Resource.Save;
            CancelText = Resource.Cancel;
            SaveCommand = new Command(Save);
            CancelCommand = new Command(Cancel);
            ShowSaveButton = false;
            StartNoteChanged = false;
            EndNoteChanged = false;
            BreakInfoChanged = false;
            EditingVehicle = false;
            EditingNote = false;
        }

        private void Cancel()
        {
            Navigation.PopAsync();
        }

        private void Save()
        {
            if(instruction==1)
            {
                if(CheckValidHuboEntry())
                {
                    //TODO: Code for checking for changes and saving as amendments
                    listOfNotes = new List<NoteTable>();
                    //Hubo Break start
                    if (VehicleStartHubo != currentStartNote.Hubo.ToString())
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.BeforeValue = currentStartNote.Hubo.ToString();
                        newAmendment.ShiftId = currentStartNote.ShiftKey;
                        newAmendment.Table = "NoteTable";
                        newAmendment.Field = "Hubo";
                        newAmendment.TimeStamp = DateTime.Now.ToString();
                        currentStartNote.Hubo = Int32.Parse(VehicleStartHubo);
                        listOfAmendments.Add(newAmendment);
                        StartNoteChanged = true;
                    }

                    //Hubo Break end
                    if(VehicleEndHubo != currentEndNote.Hubo.ToString())
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.BeforeValue = currentEndNote.Hubo.ToString();
                        newAmendment.ShiftId = currentEndNote.ShiftKey;
                        newAmendment.Table = "NoteTable";
                        newAmendment.Field = "Hubo";
                        newAmendment.TimeStamp = DateTime.Now.ToString();
                        currentEndNote.Hubo = Int32.Parse(VehicleEndHubo);
                        listOfAmendments.Add(newAmendment);
                        EndNoteChanged = true;
                    }

                    //Check if start note has changed
                    if (BreakStartNote!=currentStartNote.Note)
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.BeforeValue = currentStartNote.Note;
                        newAmendment.ShiftId = currentStartNote.ShiftKey;
                        newAmendment.Table = "NoteTable";
                        newAmendment.Field = "Note";
                        newAmendment.TimeStamp = DateTime.Now.ToString();
                        currentStartNote.Note = BreakStartNote;
                        listOfAmendments.Add(newAmendment);
                        StartNoteChanged = true;
                    }

                    //Check if end note has changed
                    if (BreakEndNote!=currentEndNote.Note)
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.BeforeValue = currentEndNote.Note;
                        newAmendment.ShiftId = currentEndNote.ShiftKey;
                        newAmendment.Table = "NoteTable";
                        newAmendment.Field = "Note";
                        newAmendment.TimeStamp = DateTime.Now.ToString();
                        currentEndNote.Note = BreakEndNote;
                        listOfAmendments.Add(newAmendment);
                        EndNoteChanged = true;
                    }


                    DateTime oldStartBreakDate = DateTime.Parse(currentBreak.StartTime).Date;
                    DateTime oldEndBreakDate = DateTime.Parse(currentBreak.EndTime).Date;

                    TimeSpan oldStartBreakTime = DateTime.Parse(currentBreak.StartTime).TimeOfDay;
                    TimeSpan oldEndBreakTime = DateTime.Parse(currentBreak.EndTime).TimeOfDay;

                    if ((BreakStartDate.Date!=oldStartBreakDate) || (BreakStartTime!=oldStartBreakTime))
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.BeforeValue = oldStartBreakDate.ToString("dd/MM/yyyy") + " " + oldStartBreakTime;
                        newAmendment.Field = "StartTime";
                        newAmendment.ShiftId = currentShift.Key;
                        newAmendment.Table = "BreakTable";
                        newAmendment.TimeStamp = DateTime.Now.ToString();
                        listOfAmendments.Add(newAmendment);
                        //currentBreak.StartTime = newStartBreakDate + " " + newStartBreakTime;
                        currentBreak.StartTime = BreakStartDate.Date.ToString("dd/MM/yyyy") + " " + BreakStartTime;
                        BreakInfoChanged = true;
                    }

                    if((BreakEndDate!=oldEndBreakDate) || (BreakEndTime!=oldEndBreakTime))
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.BeforeValue = oldEndBreakDate.ToString("dd/MM/yyyy") + " " + oldEndBreakTime;
                        newAmendment.Field = "EndTime";
                        newAmendment.Table = "BreakTable";
                        newAmendment.ShiftId = currentShift.Key;
                        newAmendment.TimeStamp = DateTime.Now.ToString();
                        listOfAmendments.Add(newAmendment);
                        //currentBreak.EndTime = newEndBreakDate + " " + newEndBreakTime;
                        currentBreak.EndTime = BreakEndDate.Date.ToString("dd/MM/yyyy") + " " + BreakEndTime.ToString();
                        BreakInfoChanged = true;
                    }

                    if (StartNoteChanged)
                    {
                        listOfNotes.Add(currentStartNote);
                    }

                    if (EndNoteChanged)
                    {
                        listOfNotes.Add(currentEndNote);
                    }

                    if (listOfAmendments.Count>0)
                    {
                        DbService.AddAmendments(listOfAmendments, null, null, listOfNotes, currentBreak);
                    }
                    Navigation.PopAsync();
                }
            }

            else if(instruction==2)
            {
                Regex regex = new Regex("^[0-9]+$");
                if (regex.IsMatch(HuboEntry))
                {

                    if (NoteEntry != currentNote.Note)
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.BeforeValue = currentNote.Note;
                        newAmendment.Field = "Note";
                        newAmendment.ShiftId = currentShift.Key;
                        newAmendment.Table = "NoteTable";
                        newAmendment.TimeStamp = DateTime.Now.ToString();
                        currentNote.Note = NoteEntry;
                        listOfAmendments.Add(newAmendment);
                    }

                    if(LocationEntry != currentNote.Location)
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.BeforeValue = currentNote.Location;
                        newAmendment.Field = "Location";
                        newAmendment.ShiftId = currentShift.Key;
                        newAmendment.Table = "NoteTable";
                        newAmendment.TimeStamp = DateTime.Now.ToString();
                        currentNote.Location = LocationEntry;
                        listOfAmendments.Add(newAmendment);
                    }

                    if (HuboEntry != currentNote.Hubo.ToString())
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.BeforeValue = currentNote.Hubo.ToString();
                        newAmendment.Field = "Hubo";
                        newAmendment.ShiftId = currentShift.Key;
                        newAmendment.Table = "NoteTable";
                        newAmendment.TimeStamp = DateTime.Now.ToString();
                        currentNote.Hubo = Int32.Parse(HuboEntry);
                        listOfAmendments.Add(newAmendment);
                    }


                    DateTime oldDate = DateTime.Parse(currentNote.Date).Date;
                    TimeSpan oldTime = DateTime.Parse(currentNote.Date).TimeOfDay;


                    if ((NoteDate!=oldDate) || (NoteTime!=oldTime))
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.BeforeValue = oldDate.Date.ToString("dd/MM/yyyy") + " " + oldTime;
                        newAmendment.Field = "Date";
                        newAmendment.ShiftId = currentShift.Key;
                        newAmendment.Table = "NoteTable";
                        newAmendment.TimeStamp = DateTime.Now.ToString();
                        currentNote.Date = NoteDate.Date.ToString("dd/MM/yyyy") + " " + NoteTime;
                        listOfAmendments.Add(newAmendment);
                    }

                    if (listOfAmendments.Count > 0)
                    {
                        DbService.AddAmendments(listOfAmendments, null, null, null, null, currentNote);
                    }

                }
                else
                {
                    Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.InvalidHubo, Resource.DisplayAlertOkay);
                }
            }

            else if(instruction==3)
            {
                if(CheckValidHuboEntry() && EditingVehicle)
                {
                    if (VehicleStartHubo != currentVehicleInUse.HuboStart.ToString())
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.BeforeValue = currentVehicleInUse.HuboStart.ToString();
                        newAmendment.Field = "HuboStart";
                        newAmendment.ShiftId = currentShift.Key;
                        newAmendment.Table = "VehicleInUseTable";
                        newAmendment.TimeStamp = DateTime.Now.ToString();
                        currentVehicleInUse.HuboStart = Int32.Parse(VehicleStartHubo);
                        listOfAmendments.Add(newAmendment);
                    }
                    if (VehicleEndHubo != currentVehicleInUse.HuboEnd.ToString())
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.BeforeValue = currentVehicleInUse.HuboEnd.ToString();
                        newAmendment.Field = "HuboEnd";
                        newAmendment.ShiftId = currentShift.Key;
                        newAmendment.Table = "VehicleInUseTable";
                        newAmendment.TimeStamp = DateTime.Now.ToString();
                        currentVehicleInUse.HuboEnd = Int32.Parse(VehicleEndHubo);
                        listOfAmendments.Add(newAmendment);
                    }

                    if (listOfAmendments.Count > 0)
                    {
                        DbService.AddAmendments(listOfAmendments, null, currentVehicleInUse);
                    }
                    Navigation.PopAsync();
                }
            }
        }

        private bool CheckValidHuboEntry()
        {
            Regex regex = new Regex("^[0-9]+$");
            if ((VehicleStartHubo.Length == 0)||(VehicleEndHubo.Length==0))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.InvalidHubo, Resource.DisplayAlertOkay);
                return false;
            }
            if (!(regex.IsMatch(VehicleStartHubo))||!(regex.IsMatch(VehicleEndHubo)))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.InvalidHubo, Resource.DisplayAlertOkay);
                return false;
            }

            return true;
        }

        internal List<NoteTable> LoadNotes()
        {
            //Load notes in relation to break
            if(instruction==1)
            {
                listOfNotes = DbService.GetNotes(listOfBreaks);
            }

            ////Load all notes (Maybe not in relation to breaks)
            else if(instruction==2)
            {
                listOfNotes = DbService.GetNotes();
            }

            return listOfNotes;

        }


        internal List<BreakTable> LoadBreaks()
        {
            listOfBreaks = DbService.GetBreaks(currentShift);
            return listOfBreaks;
        }

        internal List<VehicleInUseTable> LoadVehicles()
        {
            listUsedVehicles = DbService.GetUsedVehicles(currentShift);
            return listUsedVehicles;
        }

        internal VehicleTable LoadVehicleInfo(VehicleInUseTable vehicle)
        {
            return DbService.LoadVehicleInfo(vehicle);
        }

        internal void DisplayDetails(int selectedIndex)
        {
            //Breaks
            if(instruction==1)
            {
                BreakStartLabel = Resource.StartBreak;
                BreakEndLabel = Resource.EndBreak;
                currentBreak = listOfBreaks[selectedIndex];
                
                //TODO: Code to load the notes for this break
                foreach(NoteTable note in listOfNotes)
                {
                    if(note.Key==currentBreak.StartNoteKey)
                    {
                        //TODO: Code to load details into start note
                        BreakStartNote = note.Note;
                        VehicleStartHubo = note.Hubo.ToString();
                        BreakStartDate = DateTime.Parse(currentBreak.StartTime);
                        BreakStartTime = BreakStartDate.TimeOfDay;
                        BreakStartLocation = note.Location;
                        currentStartNote = note;
                    }
                    else if(note.Key==currentBreak.StopNoteKey)
                    {
                        //TODO: Code to load details into stop note
                        BreakEndNote = note.Note;
                        VehicleEndHubo = note.Hubo.ToString();
                        BreakEndDate = DateTime.Parse(currentBreak.EndTime);
                        BreakEndTime = BreakEndDate.TimeOfDay;
                        currentEndNote = note;
                        BreakEndLocation = note.Location;
                    }
                }

                StartTimeText = Resource.StartTime;
                EndTimeText = Resource.EndTime;

                EditingStartBreak = true;
                if(currentBreak.EndTime!=null)
                {
                    EditingEndBreak = true;
                }
                OnPropertyChanged("EditingStartBreak");
                OnPropertyChanged("EditingEndBreak");
                OnPropertyChanged("BreakStartNote");
                OnPropertyChanged("BreakStartDate");
                OnPropertyChanged("BreakStartTime");
                OnPropertyChanged("BreakEndNote");
                OnPropertyChanged("BreakEndDate");
                OnPropertyChanged("BreakEndTime");
                OnPropertyChanged("BreakStartLabel");
                OnPropertyChanged("BreakEndLabel");
                OnPropertyChanged("BreakStartLocation");
                OnPropertyChanged("BreakEndLocation");
                OnPropertyChanged("StartTimeText");
                OnPropertyChanged("EndTimeText");
            }

            if(instruction==2)
            {
                currentNote = listOfNotes[selectedIndex];
                NoteEntry = currentNote.Note;
                LocationEntry = currentNote.Location;
                HuboEntry = currentNote.Hubo.ToString();
                EditingNote = true;

                NoteDate = DateTime.Parse(currentNote.Date);
                NoteTime = NoteDate.TimeOfDay;

                NoteText = Resource.Note;
                LocationText = Resource.Location;
                HuboText = Resource.HuboEquals;
                DateText = Resource.DateEquals;

                OnPropertyChanged("NoteEntry");
                OnPropertyChanged("NoteDate");
                OnPropertyChanged("NoteTime");
                OnPropertyChanged("LocationEntry");
                OnPropertyChanged("HuboEntry");
                OnPropertyChanged("EditingNote");
                OnPropertyChanged("NoteText");
                OnPropertyChanged("LocationText");
                OnPropertyChanged("HuboText");
                OnPropertyChanged("DateText");
            }
            //Vehicle
            if (instruction==3)
            {
                currentVehicleInUse = listUsedVehicles[selectedIndex];
                VehicleStartHubo = currentVehicleInUse.HuboStart.ToString();
                VehicleEndHubo = currentVehicleInUse.HuboEnd.ToString();

                List<NoteTable> vehicleNotes = new List<NoteTable>();
                vehicleNotes = DbService.GetNotesFromVehicle(currentVehicleInUse);

                foreach(NoteTable note in vehicleNotes)
                {
                    if(note.Key == currentVehicleInUse.StartNoteKey)
                    {
                        VehicleStartLocation = note.Location;
                        VehicleStartNote = note.Note;
                    }
                    else if(note.Key == currentVehicleInUse.EndNoteKey)
                    {
                        VehicleEndLocation = note.Location;
                        VehicleEndNote = note.Note;
                    }
                }

                HuboStartText = Resource.HuboStart;
                HuboEndText = Resource.HuboEnd;
                StartLocationText = Resource.StartLocation;
                EndLocationText = Resource.EndLocation;
                StartNoteText = Resource.StartNote;
                EndNoteText = Resource.EndNote;

 
                EditingVehicle = true;                
                OnPropertyChanged("EditingVehicle");
                OnPropertyChanged("VehicleEndLocation");
                OnPropertyChanged("VehicleEndNote");
                OnPropertyChanged("VehicleStartNote");
                OnPropertyChanged("VehicleStartLocation");
                OnPropertyChanged("HuboStartText");
                OnPropertyChanged("HuboEndText");
                OnPropertyChanged("StartLocationText");
                OnPropertyChanged("EndLocationText");
                OnPropertyChanged("StartNoteText");
                OnPropertyChanged("EndNoteText");
            }

            ShowSaveButton = true;

            OnPropertyChanged("ShowSaveButton");
            OnPropertyChanged("VehicleStartHubo");
            OnPropertyChanged("VehicleEndHubo");
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
