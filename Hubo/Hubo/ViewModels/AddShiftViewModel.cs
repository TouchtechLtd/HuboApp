// <copyright file="AddShiftViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using Acr.UserDialogs;
    using Xamarin.Forms;

    internal class AddShiftViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService dbService = new DatabaseService();

        private readonly List<NoteTable> listOfNotes = new List<NoteTable>();
        private readonly List<BreakTable> listOfBreaks = new List<BreakTable>();
        private readonly List<DriveTable> listOfDrives = new List<DriveTable>();

        public AddShiftViewModel()
        {
            StartShiftText = Resource.Shift;
            DashIcon = Resource.Dash;
            LocationText = Resource.Location;
            AddText = Resource.Add;
            SaveText = Resource.Save;
            DateText = Resource.Date;
            AddButton = new Command(AddBreakNote);
            SaveButton = new Command(Save);
            BreakDetails = false;
            NoteDetails = false;
            DriveDetails = false;
            NumBreaks = 0;
            NumNotes = 0;
            NumDrives = 0;
            Date = DateTime.Now.Date;
            ButtonRow = 4;
            CreatedBreak = false;
            CreatedNote = false;
            CreatedDrive = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public string LocationText { get; set; }

        public string LocationStartData { get; set; }

        public string LocationEndData { get; set; }

        public string DriveText { get; set; }

        public TimeSpan DriveStart { get; set; }

        public TimeSpan DriveEnd { get; set; }

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

        public int ButtonRow { get; set; }

        public bool CreatedBreak { get; set; }

        public bool CreatedNote { get; set; }

        public bool CreatedDrive { get; set; }

        public bool DriveDetails { get; set; }

        public Grid DriveGrid { get; set; }

        public int NumDrives { get; set; }

        public async void Save()
        {
            PromptConfig huboPrompt = new PromptConfig();
            huboPrompt.IsCancellable = true;
            huboPrompt.Title = "Shift Add Reason:";
            huboPrompt.SetInputMode(InputType.Default);
            PromptResult promptResult = await UserDialogs.Instance.PromptAsync(huboPrompt);

            if (promptResult.Ok && promptResult.Text != string.Empty)
            {
                DateTime startShift = Date.Date + StartShift;
                DateTime endShift = Date.Date + EndShift;

                int result = dbService.SaveShift(startShift, endShift, listOfDrives);

                if (result == -1)
                {
                    await UserDialogs.Instance.ConfirmAsync(Resource.ShiftAddError, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                    return;
                }

                if (listOfBreaks.Count != 0)
                {
                    foreach (BreakTable breakItem in listOfBreaks)
                    {
                        breakItem.ShiftKey = result;
                        breakItem.ActiveBreak = false;

                        dbService.SaveBreak(breakItem);
                    }
                }
                else
                {
                    if (listOfNotes.Count != 0)
                    {
                        foreach (NoteTable note in listOfNotes)
                        {
                            note.ShiftKey = result;

                            await dbService.SaveNote(note.Note, DateTime.Parse(note.Date));
                        }
                    }
                }

                await Navigation.PopModalAsync();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void AddBreakNote()
        {
            if (Add == "Break")
            {
                MessagingCenter.Subscribe<AddManBreakNoteViewModel, BreakTable>(this, "Break_Added", (senderBreakPage, breakAdd) =>
                {
                    MessagingCenter.Unsubscribe<AddManBreakNoteViewModel, BreakTable>(this, "Break_Added");
                    if (breakAdd != null)
                    {
                        BreakText = Resource.BreaksText;
                        BreakDetails = true;

                        string breakStartTime = breakAdd.StartDate;
                        string breakEndTime = breakAdd.EndDate;
                        string breakStartLocation = breakAdd.StartLocation;
                        string breakEndLocation = breakAdd.EndLocation;

                        if (string.Compare(breakStartTime, "12:00:00") > 0)
                        {
                            TimeSpan temp = TimeSpan.Parse(breakStartTime);
                            int temphour = temp.Hours - 12;
                            int tempMin = temp.Minutes;

                            if (temphour == 00)
                            {
                                temphour = 12;
                            }

                            if (tempMin < 10)
                            {
                                breakStartTime = temphour.ToString() + ":" + "0" + tempMin.ToString() + " PM";
                            }
                            else
                            {
                                breakStartTime = temphour.ToString() + ":" + tempMin.ToString() + " PM";
                            }
                        }
                        else
                        {
                            TimeSpan temp = TimeSpan.Parse(breakStartTime);
                            int temphour = temp.Hours;
                            int tempMin = temp.Minutes;

                            if (temphour == 00)
                            {
                                temphour = 12;
                            }

                            if (tempMin < 10)
                            {
                                breakStartTime = temphour.ToString() + ":" + "0" + tempMin.ToString() + " AM";
                            }
                            else
                            {
                                breakStartTime = temphour.ToString() + ":" + tempMin.ToString() + " AM";
                            }
                        }

                        if (string.Compare(breakEndTime, "12:00:00") > 0)
                        {
                            TimeSpan temp = TimeSpan.Parse(breakEndTime);
                            int temphour = temp.Hours - 12;
                            int tempMin = temp.Minutes;

                            if (temphour == 00)
                            {
                                temphour = 12;
                            }

                            if (tempMin < 10)
                            {
                                breakEndTime = temphour.ToString() + ":" + "0" + tempMin.ToString() + " PM";
                            }
                            else
                            {
                                breakEndTime = temphour.ToString() + ":" + tempMin.ToString() + " PM";
                            }
                        }
                        else
                        {
                            TimeSpan temp = TimeSpan.Parse(breakEndTime);
                            int temphour = temp.Hours;
                            int tempMin = temp.Minutes;

                            if (temphour == 00)
                            {
                                temphour = 12;
                            }

                            if (tempMin < 10)
                            {
                                breakEndTime = temphour.ToString() + ":" + "0" + tempMin.ToString() + " AM";
                            }
                            else
                            {
                                breakEndTime = temphour.ToString() + ":" + tempMin.ToString() + " AM";
                            }
                        }

                        if (BreakDetails && !CreatedBreak)
                        {
                            ScrollView scroll = new ScrollView { Orientation = ScrollOrientation.Vertical, HorizontalOptions = LayoutOptions.FillAndExpand };

                            Grid newGrid = new Grid
                            {
                                ColumnDefinitions =
                                        {
                                            new ColumnDefinition { Width = new GridLength(.1, GridUnitType.Star) },
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
                        BreakGrid.Children.Add(new Label { Text = breakStartTime + ": " + breakStartLocation + " - " + breakEndTime + ": " + breakEndLocation }, 1, NumBreaks);
                        NumBreaks++;

                        OnPropertyChanged("BreakText");
                        OnPropertyChanged("BreakDetails");
                        OnPropertyChanged("BreakGrid");
                        OnPropertyChanged("FullGrid");
                        OnPropertyChanged("ButtonRow");

                        listOfBreaks.Add(breakAdd);
                    }
                    else
                    {
                        UserDialogs.Instance.ConfirmAsync(Resource.BreakAddError, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                    }
                });
            }
            else if (Add == "Note")
            {
                MessagingCenter.Subscribe<AddManBreakNoteViewModel, NoteTable>(this, "Note_Added", (senderNotePage, note) =>
                    {
                        MessagingCenter.Unsubscribe<AddManBreakNoteViewModel, NoteTable>(this, "Note_Added");
                        if (note != null)
                        {
                            NoteText = Resource.NotesText;
                            NoteDetails = true;

                            string noteTime = note.Date;
                            string noteDetails = note.Note;

                            if (string.Compare(noteTime, "12:00:00") > 0)
                            {
                                TimeSpan temp = TimeSpan.Parse(noteTime);
                                int temphour = temp.Hours - 12;
                                int tempMin = temp.Minutes;

                                if (temphour == 00)
                                {
                                    temphour = 12;
                                }

                                if (tempMin < 10)
                                {
                                    noteTime = temphour.ToString() + ":" + "0" + tempMin.ToString() + " PM";
                                }
                                else
                                {
                                    noteTime = temphour.ToString() + ":" + tempMin.ToString() + " PM";
                                }
                            }
                            else
                            {
                                TimeSpan temp = TimeSpan.Parse(noteTime);
                                int temphour = temp.Hours;
                                int tempMin = temp.Minutes;

                                if (temphour == 00)
                                {
                                    temphour = 12;
                                }

                                if (tempMin < 10)
                                {
                                    noteTime = temphour.ToString() + ":" + "0" + tempMin.ToString() + " AM";
                                }
                                else
                                {
                                    noteTime = temphour.ToString() + ":" + tempMin.ToString() + " AM";
                                }
                            }

                            if (NoteDetails && !CreatedNote)
                            {
                                ScrollView scroll = new ScrollView { Orientation = ScrollOrientation.Vertical, HorizontalOptions = LayoutOptions.FillAndExpand };

                                Grid newGrid = new Grid
                                {
                                    ColumnDefinitions =
                                {
                                    new ColumnDefinition { Width = new GridLength(.1, GridUnitType.Star) },
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
                            NoteGrid.Children.Add(new Label { Text = noteTime + " - " + noteDetails }, 1, NumNotes);
                            NumNotes++;

                            OnPropertyChanged("NoteText");
                            OnPropertyChanged("NoteDetails");
                            OnPropertyChanged("NoteGrid");
                            OnPropertyChanged("FullGrid");
                            OnPropertyChanged("ButtonRow");

                            listOfNotes.Add(note);
                        }
                        else
                        {
                            UserDialogs.Instance.ConfirmAsync(Resource.NoteAddError, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                        }
                    });
            }
            else if (Add == "Drive Shift")
            {
                MessagingCenter.Subscribe<AddManBreakNoteViewModel, DriveTable>(this, "Drive_Added", (senderDrivePage, drive) =>
                {
                    MessagingCenter.Unsubscribe<AddManBreakNoteViewModel, DriveTable>(this, "Drive_Added");

                    if (drive != null)
                    {
                        DriveText = Resource.Drive;
                        DriveDetails = true;

                        string driveStart = drive.StartDate.ToString();
                        string driveEnd = drive.EndDate.ToString();
                        string driveStartHubo = drive.StartHubo.ToString();
                        string driveEndHubo = drive.EndHubo.ToString();

                        if (string.Compare(driveStart, "12:00:00") > 0)
                        {
                            TimeSpan temp = TimeSpan.Parse(driveStart);
                            int temphour = temp.Hours - 12;
                            int tempMin = temp.Minutes;

                            if (temphour == 00)
                            {
                                temphour = 12;
                            }

                            if (tempMin < 10)
                            {
                                driveStart = temphour.ToString() + ":" + "0" + tempMin.ToString() + " PM";
                            }
                            else
                            {
                                driveStart = temphour.ToString() + ":" + tempMin.ToString() + " PM";
                            }
                        }
                        else
                        {
                            TimeSpan temp = TimeSpan.Parse(driveStart);
                            int temphour = temp.Hours;
                            int tempMin = temp.Minutes;

                            if (temphour == 00)
                            {
                                temphour = 12;
                            }

                            if (tempMin < 10)
                            {
                                driveStart = temphour.ToString() + ":" + "0" + tempMin.ToString() + " AM";
                            }
                            else
                            {
                                driveStart = temphour.ToString() + ":" + tempMin.ToString() + " AM";
                            }
                        }

                        if (string.Compare(driveEnd, "12:00:00") > 0)
                        {
                            TimeSpan temp = TimeSpan.Parse(driveEnd);
                            int temphour = temp.Hours - 12;
                            int tempMin = temp.Minutes;

                            if (temphour == 00)
                            {
                                temphour = 12;
                            }

                            if (tempMin < 10)
                            {
                                driveEnd = temphour.ToString() + ":" + "0" + tempMin.ToString() + " PM";
                            }
                            else
                            {
                                driveEnd = temphour.ToString() + ":" + tempMin.ToString() + " PM";
                            }
                        }
                        else
                        {
                            TimeSpan temp = TimeSpan.Parse(driveEnd);
                            int temphour = temp.Hours;
                            int tempMin = temp.Minutes;

                            if (temphour == 00)
                            {
                                temphour = 12;
                            }

                            if (tempMin < 10)
                            {
                                driveEnd = temphour.ToString() + ":" + "0" + tempMin.ToString() + " AM";
                            }
                            else
                            {
                                driveEnd = temphour.ToString() + ":" + tempMin.ToString() + " AM";
                            }
                        }

                        if (DriveDetails && !CreatedDrive)
                        {
                            ScrollView scroll = new ScrollView { Orientation = ScrollOrientation.Vertical, HorizontalOptions = LayoutOptions.FillAndExpand };

                            Grid newGrid = new Grid
                            {
                                ColumnDefinitions =
                                {
                                    new ColumnDefinition { Width = new GridLength(.1, GridUnitType.Star) },
                                    new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                                    new ColumnDefinition { Width = new GridLength(.1, GridUnitType.Star) }
                                }
                            };

                            DriveGrid = newGrid;

                            scroll.Content = newGrid;

                            FullGrid.RowDefinitions.Insert(ButtonRow - 1, new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                            FullGrid.RowDefinitions.Insert(ButtonRow - 1, new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                            FullGrid.Children.Add(new Label { Text = DriveText, VerticalTextAlignment = TextAlignment.Center }, 1, ButtonRow - 1);
                            FullGrid.Children.Add(scroll, 1, 5, ButtonRow, ButtonRow + 1);

                            ButtonRow = ButtonRow + 2;

                            CreatedDrive = true;
                        }

                        DriveGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                        DriveGrid.Children.Add(new Label { Text = driveStart + " (Hubo: " + driveStartHubo + ") - " + driveEnd + "(Hubo: " + driveEndHubo + ")" }, 1, NumDrives);
                        NumDrives++;

                        OnPropertyChanged("DriveText");
                        OnPropertyChanged("DriveDetails");
                        OnPropertyChanged("DriveGrid");
                        OnPropertyChanged("FullGrid");
                        OnPropertyChanged("ButtonRow");

                        listOfDrives.Add(drive);
                    }
                });
            }
        }

        private bool CheckValidHuboEntry(string huboValue)
        {
            Regex regex = new Regex("^[0-9]+$");
            if (huboValue.Length == 0)
            {
                UserDialogs.Instance.ConfirmAsync(Resource.InvalidHubo, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                return false;
            }

            if (!regex.IsMatch(huboValue))
            {
                UserDialogs.Instance.ConfirmAsync(Resource.InvalidHubo, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                return false;
            }

            return true;
        }
    }
}
