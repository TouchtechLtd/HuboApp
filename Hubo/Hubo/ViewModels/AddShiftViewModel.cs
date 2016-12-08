using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class AddShiftViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<VehicleTable> vehicles { get; set; }

        public INavigation Navigation { get; set; }
        public ICommand AddButton { get; set; }

        public string StartShift { get; set; }
        public string DashIcon { get; set; }
        public string AddText { get; set; }
        public string SaveText { get; set; }
        public string Date { get; set; }
        public string Vehicle { get; set; }
        public int EndShiftRow { get; set; }
        public string LocationText { get; set; }
        public string LocationStartData { get; set; }
        public string LocationEndData { get; set; }
        public string DriveText { get; set; }
        public string DriveStartData { get; set; }
        public string DriveEndData { get; set; }
        public string HuboText { get; set; }
        public string HuboStartData { get; set; }
        public string HuboEndData { get; set; }
        public string Add { get; set; }
        public string NoteText { get; set; }
        public string BreakText { get; set; }
        public bool BreakDetails { get; set; }
        public bool NoteDetails { get; set; }
        public Grid BreakGrid { get; set; }
        public Grid NoteGrid { get; set; }
        public int NumBreaks { get; set; }
        public int NumNotes { get; set; }

        DatabaseService DbService = new DatabaseService();

        NoteTable AddNoteTable = new NoteTable();
        BreakTable AddBreakTable = new BreakTable();

        List<NoteTable> listOfNotes = new List<NoteTable>();
        List<BreakTable> listOfBreaks = new List<BreakTable>();
        List<NoteTable> listOfStartNotes = new List<NoteTable>();
        List<NoteTable> listOfEndNotes = new List<NoteTable>();

        public AddShiftViewModel()
        {
            StartShift = Resource.Shift;
            DashIcon = Resource.Dash;
            DriveText = Resource.Drive;
            LocationText = Resource.Location;
            HuboText = Resource.HuboEquals;
            AddText = Resource.Add;
            SaveText = Resource.Save;
            Date = Resource.Date;
            Vehicle = Resource.Vehicle;
            AddButton = new Command(AddBreak);
            EndShiftRow = 4;
            BreakDetails = false;
            NoteDetails = false;
            NumBreaks = 0;
            NumNotes = 0;
        }

        internal List<VehicleTable> GetVehicles()
        {
            vehicles = DbService.GetVehicles();
            return vehicles;
        }

        public void Save()
        {

        }

        private void AddBreak()
        {
            //if(EndShiftRow==4)
            //{
            //    EndShiftRow = 6;
            //}
            //else if(EndShiftRow==6)
            //{
            //    EndShiftRow = 8;
            //}
            //OnPropertyChanged("EndShiftRow");

            MessagingCenter.Subscribe<AddManBreakNoteViewModel>(this, "Note_Break_Added", (senderPage) =>
            {
                MessagingCenter.Unsubscribe<AddManBreakNoteViewModel>(this, "Note_Break_Added");
                //TODO: add note or break to page
                if (Add == "Break")
                {
                    listOfBreaks = DbService.GetRecentBreak();

                    if (listOfBreaks != null)
                    {
                        listOfStartNotes = DbService.GetSelectedNote(listOfBreaks[0].StartNoteKey);
                        listOfEndNotes = DbService.GetSelectedNote(listOfBreaks[0].StopNoteKey);

                        if (listOfStartNotes != null && listOfEndNotes != null)
                        {
                            BreakText = Resource.BreaksText;
                            BreakDetails = true;

                            string[] breakStartTime = listOfBreaks[0].StartTime.Split(' ');
                            string[] breakEndTime = listOfBreaks[0].EndTime.Split(' ');
                            string breakStartNote = listOfStartNotes[0].Note;
                            string breakStartLocation = listOfStartNotes[0].Location;
                            string breakEndNote = listOfEndNotes[0].Note;
                            string breakEndLocation = listOfStartNotes[0].Location;

                            BreakGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                            BreakGrid.Children.Add(new Label { Text = breakStartTime[1] + " " + breakStartTime[2] + ": " + breakStartNote + ", " + breakStartLocation + " - " + breakEndTime[1] + " " + breakEndTime[2] + ": " + breakEndNote + ", " + breakEndLocation }, 1, NumBreaks);
                            NumBreaks++;

                            OnPropertyChanged("BreakText");
                            OnPropertyChanged("BreakDetails");
                            OnPropertyChanged("BreakGrid");
                        }
                        else
                        {
                            Application.Current.MainPage.DisplayAlert("Error Adding Break", "Unable to add break!", "OK");
                        }
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("Error Adding Break", "Unable to add break!", "OK");
                    }
                }
                else if (Add == "Note")
                {
                    listOfNotes = DbService.GetRecentNote();

                    if (listOfNotes != null)
                    {
                        NoteText = Resource.NotesText;
                        NoteDetails = true;

                        string[] noteTime = listOfNotes[0].Date.Split(' ');
                        string noteDetails = listOfNotes[0].Note;
                        string noteLocation = listOfNotes[0].Location;

                        NoteGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                        NoteGrid.Children.Add(new Label { Text = noteTime[1] + " " + noteTime[2] + " - " + noteDetails + ", " + noteLocation }, 1, NumNotes);
                        NumNotes++;

                        OnPropertyChanged("NoteText");
                        OnPropertyChanged("NoteDetails");
                        OnPropertyChanged("NoteGrid");
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("Error Adding Note", "Unable to add note!", "OK");
                    }
                }
            });  
        }

        //internal List<NoteTable> LoadNote(int selectedIndex)
        //{
        //    listOfNotes = DbService.GetRecentNote(selectedIndex);
        //    return listOfNotes;
        //}

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
