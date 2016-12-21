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

        public bool AddingBreak { get; set; }
        public bool AddingNote { get; set; }

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
        public string NoteLocationText { get; set; }
        public string NoteHuboText { get; set; }
        public string BreakStartNoteText { get; set; }
        public string BreakEndNoteText { get; set; }

        public TimeSpan BreakStart { get; set; }
        public TimeSpan BreakEnd { get; set; }
        public string LocationStart { get; set; }
        public string LocationEnd { get; set; }
        public string HuboStart { get; set; }
        public string HuboEnd { get; set; }
        public TimeSpan NoteTime { get; set; }
        public string NoteDetail { get; set; }
        public string NoteLocation { get; set; }
        public string NoteHubo { get; set; }
        public string BreakStartNote { get; set; }
        public string BreakEndNote { get; set; }

        public ICommand AddCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public string AddText { get; set; }
        public string CancelText { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public AddManBreakNoteViewModel(string instructionCommand)
        {
            instruction = instructionCommand;
            AddingBreak = false;
            AddingNote = false;
            CancelText = Resource.Cancel;
            AddCommand = new Command(Add);
            CancelCommand = new Command(Cancel);
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

                NoteTable noteTableStart = new NoteTable();
                NoteTable noteTableEnd = new NoteTable();
                BreakTable breakTable = new BreakTable();

                breakTable.StartTime = BreakStart.ToString();
                breakTable.EndTime = BreakEnd.ToString();

                noteTableStart.Date = BreakStart.ToString();
                noteTableStart.Note = BreakStartNote;
                noteTableStart.Location = LocationStart;
                noteTableStart.Hubo = int.Parse(HuboStart);
                noteTableStart.StandAloneNote = false;

                noteTableEnd.Date = BreakEnd.ToString();
                noteTableEnd.Note = BreakEndNote;
                noteTableEnd.Location = LocationEnd;
                noteTableEnd.Hubo = int.Parse(HuboEnd);
                noteTableEnd.StandAloneNote = false;

                List<NoteTable> noteList = new List<NoteTable>();
                List<BreakTable> breakList = new List<BreakTable>();

                breakList.Add(breakTable);
                noteList.Add(noteTableStart);
                noteList.Add(noteTableEnd);

                MessagingCenter.Send(this, "Note_Added", noteList);
                MessagingCenter.Send(this, "Break_Added", breakList);
            }
            else if (instruction == "Note")
            {
                NoteTable noteTable = new NoteTable();
                noteTable.Date = NoteTime.ToString();
                noteTable.Note = NoteDetail;
                noteTable.Location = NoteLocation;
                if (NoteHubo != null)
                {
                    if (!CheckValidHuboEntry(NoteHubo))
                        return;
                    noteTable.Hubo = int.Parse(NoteHubo);
                }
                noteTable.StandAloneNote = true;

                List<NoteTable> noteList = new List<NoteTable>();
                noteList.Add(noteTable);

                MessagingCenter.Send(this, "Note_Added", noteList);
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
                BreakStartNoteText = Resource.StartNote;
                LocationStartText = Resource.StartLocation;
                HuboStartText = Resource.StartHubo;
                BreakEndText = Resource.EndBreak;
                BreakEndTimeText = Resource.EndTime;
                BreakEndNoteText = Resource.EndNote;
                LocationEndText = Resource.EndLocation;
                HuboEndText = Resource.EndHubo;

                OnPropertyChanged("AddingBreak");
                OnPropertyChanged("AddText");
                OnPropertyChanged("BreakStartText");
                OnPropertyChanged("BreakStartTimeText");
                OnPropertyChanged("BreakStartNoteText");
                OnPropertyChanged("LocationStartText");
                OnPropertyChanged("HuboStartText");
                OnPropertyChanged("BreakEndText");
                OnPropertyChanged("BreakEndTimeText");
                OnPropertyChanged("BreakEndNoteText");
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
                NoteLocationText = Resource.Location;
                NoteHuboText = Resource.HuboNotRequired;

                OnPropertyChanged("AddingNote");
                OnPropertyChanged("NoteText");
                OnPropertyChanged("NoteTimeText");
                OnPropertyChanged("NoteDetailText");
                OnPropertyChanged("NoteLocationText");
                OnPropertyChanged("NoteHuboText");
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
