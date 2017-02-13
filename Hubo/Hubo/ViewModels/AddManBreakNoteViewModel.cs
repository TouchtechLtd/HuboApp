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
    class AddManBreakNoteViewModel : INotifyPropertyChanged
    {
        public string instruction { get; set; }
        public INavigation Navigation { get; set; }
        public List<VehicleTable> vehicles { get; set; }

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
        public int selectedVehicle { get; set; }
        public bool AddingBreak { get; set; }
        public bool AddingNote { get; set; }
        public bool AddingDrive { get; set; }
        public string Vehicle { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        DatabaseService DbService = new DatabaseService();

        public AddManBreakNoteViewModel(string instructionCommand)
        {
            instruction = instructionCommand;
            CancelText = Resource.Cancel;
            AddCommand = new Command(Add);
            CancelCommand = new Command(Cancel);
            Vehicle = Resource.Vehicle;
            AddingBreak = false;
            AddingNote = false;
            AddingDrive = false;
        }

        internal List<VehicleTable> GetVehicles()
        {
            vehicles = DbService.GetVehicles();
            return vehicles;
        }

        private async void Cancel()
        {
            await Navigation.PopModalAsync();
        }

        private void Add()
        {
            if (instruction == "Break")
            {
                if (!CheckValidHuboEntry(HuboStart))
                    return;
                if (!CheckValidHuboEntry(HuboEnd))
                    return;

                BreakTable breakAdd = new BreakTable();

                breakAdd.StartDate = BreakStart.ToString();
                breakAdd.EndDate = BreakEnd.ToString();
                breakAdd.StartLocation = LocationStart;
                breakAdd.EndLocation = LocationEnd;

                MessagingCenter.Send(this, "Break_Added", breakAdd);
            }
            else if (instruction == "Note")
            {
                NoteTable note = new NoteTable();
                note.Date = NoteTime.ToString();
                note.Note = NoteDetail;

                MessagingCenter.Send(this, "Note_Added", note);
            }
            else if (instruction == "Drive Shift")
            {
                if (!CheckValidHuboEntry(HuboStart))
                    return;
                if (!CheckValidHuboEntry(HuboEnd))
                    return;
                List<VehicleTable> vehicleKey = GetVehicles();

                DriveTable drive = new DriveTable();
                drive.StartDate = DriveStartTime.ToString();
                drive.EndDate = DriveEndTime.ToString();
                drive.StartHubo = int.Parse(HuboStart);
                drive.EndHubo = int.Parse(HuboEnd);
                drive.ActiveVehicle = false;
                drive.VehicleKey = vehicleKey[selectedVehicle].Key;

                MessagingCenter.Send(this, "Drive_Added", drive);
            }

            Navigation.PopModalAsync();
        }

        internal void InflatePage()
        {
            if (instruction == "Break")
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
            else if (instruction == "Note")
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
            else if (instruction == "Drive Shift")
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

        private bool CheckValidHuboEntry(string huboValue)
        {
            Regex regex = new Regex("^[0-9]+$");
            if ((huboValue.Length == 0) || (huboValue.Length == 0))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.InvalidHubo, Resource.DisplayAlertOkay);
                return false;
            }
            if (!(regex.IsMatch(huboValue)) || !(regex.IsMatch(huboValue)))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.InvalidHubo, Resource.DisplayAlertOkay);
                return false;
            }

            return true;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
