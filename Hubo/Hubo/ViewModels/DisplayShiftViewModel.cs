// <copyright file="DisplayShiftViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Xamarin.Forms;

    internal class DisplayShiftViewModel : INotifyPropertyChanged
    {
        private DatabaseService db = new DatabaseService();

        private ObservableCollection<CarouselViewModel> pages;
        private CarouselViewModel currentPage;

        private bool canExecuteRight = false;
        private bool canExecuteLeft = false;
        private bool extraDetails;
        private List<DriveTable> drives;
        private List<BreakTable> breaks;
        private List<NoteTable> notes;
        private int currentShiftIndex;
        private ICommand changeShiftLeftCommand;
        private ICommand changeShiftRightCommand;

        public DisplayShiftViewModel()
        {
            DateText = Resource.DateEquals;
            HuboText = Resource.HuboEquals;
            Dash = Resource.Dash;
            LocationText = Resource.Location;
        }

        public DisplayShiftViewModel(DateTime date)
        {
            ShiftSelected = false;
            SelectedDate = date;
            DateText = Resource.DateEquals;
            HuboText = Resource.HuboEquals;
            Dash = Resource.Dash;
            DrivesText = Resource.Drive;
            LocationText = Resource.Location;
            NotesText = Resource.NotesText;
            BreaksText = Resource.BreaksText;
            CloseCommand = new Command(Close);
            ChangeShiftLeftCommand = new Command(ChangeShift, (s) => canExecuteLeft);
            ChangeShiftRightCommand = new Command(ChangeShift, (s) => canExecuteRight);
            CloseText = Resource.CloseText;
            ShiftList = db.GetDayShifts(date);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<ShiftTable> ShiftList { get; set; }

        public DateTime SelectedDate { get; set; }

        public bool ShiftSelected { get; set; }

        public string ShiftDate { get; set; }

        public string ShiftLocation { get; set; }

        public string Dash { get; set; }

        public string CloseText { get; set; }

        public string DateText { get; set; }

        public string LocationText { get; set; }

        public string HuboText { get; set; }

        public string DrivesText { get; set; }

        public string NotesText { get; set; }

        public string BreaksText { get; set; }

        public List<DriveTable> Drives
        {
            get
            {
                return drives;
            }

            set
            {
                drives = value;
                OnPropertyChanged("Drives");
            }
        }

        public List<BreakTable> Breaks
        {
            get
            {
                return breaks;
            }

            set
            {
                breaks = value;
                OnPropertyChanged("Breaks");
            }
        }

        public List<NoteTable> Notes
        {
            get
            {
                return notes;
            }

            set
            {
                notes = value;
                OnPropertyChanged("Notes");
            }
        }

        public int CurrentShiftIndex
        {
            get
            {
                return currentShiftIndex;
            }

            set
            {
                currentShiftIndex = value;
                OnPropertyChanged("Notes");
            }
        }

        public ICommand CloseCommand { get; set; }

        public ICommand ChangeShiftLeftCommand
        {
            get
            {
                return changeShiftLeftCommand;
            }

            set
            {
                changeShiftLeftCommand = value;
                OnPropertyChanged("ChangeShiftLeftCommand");
            }
        }

        public ICommand ChangeShiftRightCommand
        {
            get
            {
                return changeShiftRightCommand;
            }

            set
            {
                changeShiftRightCommand = value;
                OnPropertyChanged("ChangeShiftRightCommand");
            }
        }

        public INavigation Navigation { get; set; }

        public ObservableCollection<CarouselViewModel> Pages
        {
            get
            {
                return pages;
            }

            set
            {
                pages = value;
                OnPropertyChanged("Pages");
            }
        }

        public CarouselViewModel CurrentPage
        {
            get
            {
                return currentPage;
            }

            set
            {
                currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }

        public bool ExtraDetails
        {
            get
            {
                return extraDetails;
            }

            set
            {
                extraDetails = value;
                OnPropertyChanged("ExtraDetails");
            }
        }

        public bool CanExecuteRight
        {
            get
            {
                return canExecuteRight;
            }

            set
            {
                canExecuteRight = value;
                OnPropertyChanged("CanExecuteRight");
            }
        }

        public bool CanExecuteLeft
        {
            get
            {
                return canExecuteLeft;
            }

            set
            {
                canExecuteLeft = value;
                OnPropertyChanged("CanExecuteLeft");
            }
        }

        public void Close()
        {
            Navigation.PopModalAsync();
        }

        public void LoadShiftDetails(ShiftTable shift)
        {
            Pages = new ObservableCollection<CarouselViewModel>();

            Drives = new List<DriveTable>();
            Notes = new List<NoteTable>();
            Breaks = new List<BreakTable>();

            Drives = db.GetDriveShifts(shift.Key);
            Notes = db.GetNotes(shift.Key);
            Breaks = db.GetBreaks(shift);

            if (Drives.Count != 0 || Breaks.Count != 0 || Notes.Count != 0)
            {
                ExtraDetails = true;
            }
            else
            {
                ExtraDetails = false;
            }

            if (Drives.Count != 0)
            {
                Pages.Add(new CarouselViewModel { Title = "Drives", Page = "DriveView", ImageSource = "icon.png", Drives = Drives });
            }

            if (Notes.Count != 0)
            {
                Pages.Add(new CarouselViewModel { Title = "Notes", Page = "NoteView", ImageSource = "icon.png", Notes = Notes });
            }

            if (Breaks.Count != 0)
            {
                Pages.Add(new CarouselViewModel { Title = "Breaks", Page = "BreakView", ImageSource = "icon.png", Breaks = Breaks });
            }

            CurrentPage = Pages.FirstOrDefault();

            if (shift.EndDate != string.Empty)
            {
                ShiftDate = DateTime.Parse(shift.StartDate).ToString("g") + " - " + DateTime.Parse(shift.EndDate).ToString("g");
            }
            else
            {
                ShiftDate = DateTime.Parse(shift.StartDate).ToString("g") + " - Current";
            }

            if (shift.StartLocation != null)
            {
                string startLocation = shift.StartLocation.Split(',')[1];

                if (shift.EndLocation != null)
                {
                    string endLocation = shift.EndLocation.Split(',')[1];

                    ShiftLocation = startLocation.Trim() + " - " + endLocation.Trim();
                }
                else
                {
                    ShiftLocation = startLocation.Trim() + " -";
                }
            }
            else
            {
                ShiftLocation = "Unknown - Unknown";
            }

            ShiftSelected = true;

            CurrentShiftIndex = ShiftList.IndexOf(shift);

            if (CurrentShiftIndex == 0)
            {
                CanExecuteChangeShift(false, true);
            }
            else if (CurrentShiftIndex == (ShiftList.Count - 1))
            {
                CanExecuteChangeShift(true, false);
            }
            else
            {
                CanExecuteChangeShift(true, true);
            }

            OnPropertyChanged("Drives");
            OnPropertyChanged("Notes");
            OnPropertyChanged("Breaks");
            OnPropertyChanged("ShiftDate");
            OnPropertyChanged("ShiftLocation");
            OnPropertyChanged("ShiftSelected");
            OnPropertyChanged("DrivesAvailable");
            OnPropertyChanged("BreaksAvailable");
            OnPropertyChanged("NotesAvailable");
        }

        public void ChangeShift(object direction)
        {
            if ((string)direction == "Left")
            {
                MessagingCenter.Send<string, int>("ChangeShift", "ChangeShift", CurrentShiftIndex - 1);
            }
            else if ((string)direction == "Right")
            {
                MessagingCenter.Send<string, int>("ChangeShift", "ChangeShift", CurrentShiftIndex + 1);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CanExecuteChangeShift(bool left, bool right)
        {
            CanExecuteRight = right;
            ((Command)ChangeShiftRightCommand).ChangeCanExecute();

            CanExecuteLeft = left;
            ((Command)ChangeShiftLeftCommand).ChangeCanExecute();
        }
    }

    internal class CarouselViewModel : ITabProvider
    {
        public CarouselViewModel()
        {
        }

        public string Title { get; set; }

        public string Page { get; set; }

        public string ImageSource { get; set; }

        public List<DriveTable> Drives { get; set; }

        public List<BreakTable> Breaks { get; set; }

        public List<NoteTable> Notes { get; set; }
    }
}