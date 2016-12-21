﻿using System;
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
        public Grid FullGrid { get; set; }
        public int NumBreaks { get; set; }
        public int NumNotes { get; set; }
        public int selectedVehicle { get; set; }
        public int ButtonRow { get; set; }
        public bool CreatedBreak { get; set; }
        public bool CreatedNote { get; set; }

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
            BreakDetails = false;
            NoteDetails = false;
            NumBreaks = 0;
            NumNotes = 0;
            Date = DateTime.Now.Date;
            ButtonRow = 7;
            CreatedBreak = false;
            CreatedNote = false;
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
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.ShiftAddError, Resource.DisplayAlertOkay);
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
                    }

                    for (int v = 0; v < breakEndNote.Count; v++)
                    {
                        noteDetails = listOfNotes[breakStartNote[v]];
                        noteEndDetails = listOfNotes[breakEndNote[v]];
                        DbService.SaveBreak(breakDetails.StartTime, breakDetails.EndTime, result, noteDetails.Note, noteDetails.Hubo, noteDetails.Location, noteEndDetails.Note, noteEndDetails.Hubo, noteEndDetails.Location);
                    }
                }

                foreach (NoteTable note in listOfNotes)
                {
                    if (note.StandAloneNote == true)
                    {
                        DbService.SaveManNote(note.Note, note.Date, result, note.Hubo, note.Location);
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
            MessagingCenter.Subscribe<AddManBreakNoteViewModel, List<NoteTable>>(this, "Note_Added", (senderNotePage, noteList) =>
            {
                MessagingCenter.Unsubscribe<AddManBreakNoteViewModel, List<NoteTable>>(this, "Note_Added");

                if (Add == "Break")
                {
                    MessagingCenter.Subscribe<AddManBreakNoteViewModel, List<BreakTable>>(this, "Break_Added", (senderBreakPage, breakList) =>
                    {
                        MessagingCenter.Unsubscribe<AddManBreakNoteViewModel, List<BreakTable>>(this, "Break_Added");
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

                                if (BreakDetails && !CreatedBreak)
                                {
                                    ScrollView scroll = new ScrollView { Orientation = ScrollOrientation.Vertical, HorizontalOptions = LayoutOptions.FillAndExpand };

                                    Grid newGrid = new Grid
                                    {
                                        ColumnDefinitions =
                                        {
                                            new ColumnDefinition { Width = new GridLength(.1, GridUnitType.Star)},
                                            new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                                            new ColumnDefinition { Width = new GridLength(.1, GridUnitType.Star) }
                                        }
                                    };

                                    BreakGrid = newGrid;

                                    scroll.Content = newGrid;

                                    FullGrid.RowDefinitions.Insert(ButtonRow - 1, new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                                    FullGrid.RowDefinitions.Insert(ButtonRow - 1, new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                                    FullGrid.Children.Add(new Label { Text = BreakText, VerticalTextAlignment = TextAlignment.Center }, 1, ButtonRow - 1);
                                    FullGrid.Children.Add(scroll, 1, 5, ButtonRow, ButtonRow + 1);

                                    ButtonRow = ButtonRow + 2;

                                    CreatedBreak = true;
                                }

                                BreakGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                                BreakGrid.Children.Add(new Label { Text = breakStartTime + ": " + breakStartNote + ", " + breakStartLocation + " - " + breakEndTime + ": " + breakEndNote + ", " + breakEndLocation }, 1, NumBreaks);
                                NumBreaks++;

                                OnPropertyChanged("BreakText");
                                OnPropertyChanged("BreakDetails");
                                OnPropertyChanged("BreakGrid");
                                OnPropertyChanged("FullGrid");
                                OnPropertyChanged("ButtonRow");

                                listOfBreaks.AddRange(breakList);
                                listOfNotes.AddRange(noteList);
                            }
                            else
                            {
                                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.BreakAddError, Resource.DisplayAlertOkay);
                            }
                        }
                        else
                        {
                            Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.BreakAddError, Resource.DisplayAlertOkay);
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

                        if (String.Compare(noteTime, "12:00:00") > 0)
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

                        if (NoteDetails && !CreatedNote)
                        {
                            ScrollView scroll = new ScrollView { Orientation = ScrollOrientation.Vertical, HorizontalOptions = LayoutOptions.FillAndExpand };

                            Grid newGrid = new Grid
                            {
                                ColumnDefinitions =
                                {
                                    new ColumnDefinition { Width = new GridLength(.1, GridUnitType.Star)},
                                    new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                                    new ColumnDefinition { Width = new GridLength(.1, GridUnitType.Star) }
                                }
                            };

                            NoteGrid = newGrid;

                            scroll.Content = newGrid;

                            FullGrid.RowDefinitions.Insert(ButtonRow - 1, new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                            FullGrid.RowDefinitions.Insert(ButtonRow - 1, new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                            FullGrid.Children.Add(new Label { Text = NoteText, VerticalTextAlignment = TextAlignment.Center }, 1, ButtonRow - 1);
                            FullGrid.Children.Add(scroll, 1, 5, ButtonRow, ButtonRow + 1);

                            ButtonRow = ButtonRow + 2;

                            CreatedNote = true;
                        }

                        NoteGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                        NoteGrid.Children.Add(new Label { Text = noteTime + " - " + noteDetails + ", " + noteLocation }, 1, NumNotes);
                        NumNotes++;

                        OnPropertyChanged("NoteText");
                        OnPropertyChanged("NoteDetails");
                        OnPropertyChanged("NoteGrid");
                        OnPropertyChanged("FullGrid");
                        OnPropertyChanged("ButtonRow");

                        listOfNotes.AddRange(noteList);
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.NoteAddError, Resource.DisplayAlertOkay);
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
