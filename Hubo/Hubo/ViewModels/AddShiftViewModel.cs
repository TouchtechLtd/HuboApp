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
    class AddShiftViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<VehicleTable> vehicles { get; set; }

        public INavigation Navigation { get; set; }
        public ICommand AddButton { get; set; }
        public ICommand SaveButton { get; set; }

        public string StartShiftText { get; set; }
        public TimeSpan StartShift { get; set; }
        public TimeSpan EndShift { get; set; }
        public string DashIcon { get; set; }
        public string AddText { get; set; }
        public string SaveText { get; set; }
        public string DateText { get; set; }
        public DateTime Date { get; set; }
        public string Vehicle { get; set; }
        public int EndShiftRow { get; set; }
        public string LocationText { get; set; }
        public string LocationStartData { get; set; }
        public string LocationEndData { get; set; }
        public string DriveText { get; set; }
        public TimeSpan DriveStartData { get; set; }
        public TimeSpan DriveEndData { get; set; }
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
        public int selectedVehicle { get; set; }

        DatabaseService DbService = new DatabaseService();

        NoteTable AddNoteTable = new NoteTable();
        BreakTable AddBreakTable = new BreakTable();

        List<NoteTable> listOfNotes = new List<NoteTable>();
        List<BreakTable> listOfBreaks = new List<BreakTable>();

        public AddShiftViewModel()
        {
            StartShiftText = Resource.Shift;
            DashIcon = Resource.Dash;
            DriveText = Resource.Drive;
            LocationText = Resource.Location;
            HuboText = Resource.HuboEquals;
            AddText = Resource.Add;
            SaveText = Resource.Save;
            DateText = Resource.Date;
            Vehicle = Resource.Vehicle;
            AddButton = new Command(AddBreakNote);
            SaveButton = new Command(Save);
            EndShiftRow = 4;
            BreakDetails = false;
            NoteDetails = false;
            NumBreaks = 0;
            NumNotes = 0;
            Date = DateTime.Now.Date;
        }

        internal List<VehicleTable> GetVehicles()
        {
            vehicles = DbService.GetVehicles();
            return vehicles;
        }

        public void Save()
        {
            if (!CheckValidHuboEntry(HuboStartData))
                return;
            if (!CheckValidHuboEntry(HuboEndData))
                return;

            DateTime startDate = Date.Date + StartShift;
            DateTime endDate = Date.Date + EndShift;

            List<VehicleTable> vehicleKey = GetVehicles();

            int result = DbService.SaveShift(startDate, endDate, vehicleKey[selectedVehicle], HuboStartData, HuboEndData, LocationStartData, LocationEndData);

            if (result == -1)
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to save shift!", Resource.DisplayAlertOkay);
                return;
            }

            if (listOfBreaks.Count != 0)
            {
                List<int> breakStartNote = new List<int>();
                List<int> breakEndNote = new List<int>();
                List<int> notes = new List<int>();

                BreakTable breakDetails = new BreakTable();
                NoteTable noteDetails = new NoteTable();
                NoteTable noteEndDetails = new NoteTable();

                for (int i = 0; i < listOfBreaks.Count; i++)
                {
                    breakDetails = listOfBreaks[i];
                    for (int n = 0; n < listOfNotes.Count; n++)
                    {
                        noteDetails = listOfNotes[n];
                        if (noteDetails.Date == breakDetails.StartTime)
                        {
                            breakStartNote.Add(n);
                        }
                        else if (noteDetails.Date == breakDetails.EndTime)
                        {
                            breakEndNote.Add(n);
                        }
                        else if (noteDetails.StandAloneNote == true)
                        {
                            notes.Add(n);
                        }
                    }

                    for (int v = 0; v < breakEndNote.Count; v++)
                    {
                            noteDetails = listOfNotes[breakStartNote[v]];
                            noteEndDetails = listOfNotes[breakEndNote[v]];
                            DbService.SaveBreak(breakDetails.StartTime, breakDetails.EndTime, result, noteDetails.Note, noteDetails.Hubo, noteDetails.Location, noteEndDetails.Note, noteEndDetails.Hubo, noteEndDetails.Location);
                    }
                }

                if (notes.Count > 0)
                {
                    for (int v = 0; v < notes.Count; v++)
                    {
                            noteDetails = listOfNotes[notes[v]];
                            DbService.SaveManNote(noteDetails.Note, noteDetails.Date, result, noteDetails.Hubo, noteDetails.Location);
                    }
                }
            }
            else
            {
                if (listOfNotes.Count != 0)
                {
                    foreach (NoteTable note in listOfNotes)
                    {
                        DbService.SaveManNote(note.Note, note.Date, result, note.Hubo, note.Location);
                    }
                }
            }
            Navigation.PopAsync();
        }

        private void AddBreakNote()
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

            MessagingCenter.Subscribe<AddManBreakNoteViewModel, List<NoteTable>>(this, "Note_Added", (senderNotePage, noteList) =>
            {
                MessagingCenter.Unsubscribe<AddManBreakNoteViewModel, List<NoteTable>>(this, "Note_Added");

                if (Add == "Break")
                {
                    MessagingCenter.Subscribe<AddManBreakNoteViewModel, List<BreakTable>>(this, "Break_Added", (senderBreakPage, breakList) =>
                    {
                        MessagingCenter.Unsubscribe<AddManBreakNoteViewModel, List<BreakTable>>(this, "Break_Added");
                        //TODO: add note or break to page
                        if (breakList != null)
                        {
                            if (noteList != null && noteList != null)
                            {
                                BreakText = Resource.BreaksText;
                                BreakDetails = true;

                                string breakStartTime = breakList[0].StartTime;
                                string breakEndTime = breakList[0].EndTime;
                                string breakStartNote = noteList[0].Note;
                                string breakStartLocation = noteList[0].Location;
                                string breakEndNote = noteList[1].Note;
                                string breakEndLocation = noteList[1].Location;

                                if (String.Compare(breakStartTime, "12:00:00") > 0)
                                {
                                    TimeSpan temp = TimeSpan.Parse(breakStartTime);
                                    int temphour = temp.Hours - 12;
                                    int tempMin = temp.Minutes;

                                    if (temphour == 00)
                                        temphour = 12;

                                    if (tempMin < 10)
                                        breakStartTime = temphour.ToString() + ":" + "0" + tempMin.ToString() + " PM";
                                    else
                                        breakStartTime = temphour.ToString() + ":" + tempMin.ToString() + " PM";
                                }
                                else
                                {
                                    TimeSpan temp = TimeSpan.Parse(breakStartTime);
                                    int temphour = temp.Hours;
                                    int tempMin = temp.Minutes;

                                    if (temphour == 00)
                                        temphour = 12;

                                    if (tempMin < 10)
                                        breakStartTime = temphour.ToString() + ":" + "0" + tempMin.ToString() + " AM";
                                    else
                                        breakStartTime = temphour.ToString() + ":" + tempMin.ToString() + " AM";
                                }

                                if (String.Compare(breakEndTime, "12:00:00") > 0)
                                {
                                    TimeSpan temp = TimeSpan.Parse(breakEndTime);
                                    int temphour = temp.Hours - 12;
                                    int tempMin = temp.Minutes;

                                    if (temphour == 00)
                                        temphour = 12;

                                    if (tempMin < 10)
                                        breakEndTime = temphour.ToString() + ":" + "0" + tempMin.ToString() + " PM";
                                    else
                                        breakEndTime = temphour.ToString() + ":" + tempMin.ToString() + " PM";
                                }
                                else
                                {
                                    TimeSpan temp = TimeSpan.Parse(breakEndTime);
                                    int temphour = temp.Hours;
                                    int tempMin = temp.Minutes;

                                    if (temphour == 00)
                                        temphour = 12;

                                    if (tempMin < 10)
                                        breakEndTime = temphour.ToString() + ":" + "0" + tempMin.ToString() + " AM";
                                    else
                                        breakEndTime = temphour.ToString() + ":" + tempMin.ToString() + " AM";
                                }

                                BreakGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                                BreakGrid.Children.Add(new Label { Text = breakStartTime + ": " + breakStartNote + ", " + breakStartLocation + " - " + breakEndTime + ": " + breakEndNote + ", " + breakEndLocation }, 1, NumBreaks);
                                NumBreaks++;

                                OnPropertyChanged("BreakText");
                                OnPropertyChanged("BreakDetails");
                                OnPropertyChanged("BreakGrid");

                                listOfBreaks.AddRange(breakList);
                                listOfNotes.AddRange(noteList);
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
                    });
                    }
                    else if (Add == "Note")
                    {
                        if (noteList != null)
                        {
                            NoteText = Resource.NotesText;
                            NoteDetails = true;

                            string noteTime = noteList[0].Date;
                            string noteDetails = noteList[0].Note;
                            string noteLocation = noteList[0].Location;

                            if (String.Compare(noteTime, "12:00:00") > 0 )
                            {
                                TimeSpan temp = TimeSpan.Parse(noteTime);
                                int temphour = temp.Hours - 12;
                                int tempMin = temp.Minutes;

                                if (temphour == 00)
                                    temphour = 12;

                                if (tempMin < 10)
                                    noteTime = temphour.ToString() + ":" + "0" + tempMin.ToString() + " PM";
                                else
                                    noteTime = temphour.ToString() + ":" + tempMin.ToString() + " PM";
                            }
                            else
                            {
                                TimeSpan temp = TimeSpan.Parse(noteTime);
                                int temphour = temp.Hours;
                                int tempMin = temp.Minutes;

                                if (temphour == 00)
                                    temphour = 12;

                                if (tempMin < 10)
                                    noteTime = temphour.ToString() + ":" + "0" + tempMin.ToString() + " AM";
                                else
                                    noteTime = temphour.ToString() + ":" + tempMin.ToString() + " AM";
                            }

                            NoteGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                            NoteGrid.Children.Add(new Label { Text = noteTime + " - " + noteDetails + ", " + noteLocation }, 1, NumNotes);
                            NumNotes++;

                            OnPropertyChanged("NoteText");
                            OnPropertyChanged("NoteDetails");
                            OnPropertyChanged("NoteGrid");

                        listOfNotes.AddRange(noteList);
                        }
                        else
                        {
                            Application.Current.MainPage.DisplayAlert("Error Adding Note", "Unable to add note!", "OK");
                        }
                    }
            });  
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
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
