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
    class AddNoteViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public string SaveText { get; set; }
        public string CancelText { get; set; }
        public DateTime Date { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public string Note { get; set; }
        public string NoteText { get; set; }
        public string HuboLabel { get; set; }
        public string HuboEntry { get; set; }
        public string Location { get; set; }
        public string LocationText { get; set; }
        public string LocationGeoText { get; set; }
        public string LocationDashText { get; set; }
        public string LocationLat { get; set; }
        public string LocationLong { get; set; }
        public int CurrentVehicleKey { get; set; }
        public bool DriveActive { get; set; }
        public bool HuboVisible { get; set; }
        public bool NoteVisible { get; set; }
        public bool LocationVisible { get; set; }

        DatabaseService DbService = new DatabaseService();

        public event PropertyChangedEventHandler PropertyChanged;

        public AddNoteViewModel()
        {
            SaveText = Resource.Save;
            CancelText = Resource.Cancel;
            Date = DateTime.Now;
            HuboEntry = "";
            Note = "";
            Location = "";
            LocationLat = "";
            LocationLong = "";
            NoteText = Resource.Note;
            LocationText = Resource.Location;
            LocationGeoText = Resource.LocationGeo;
            LocationDashText = Resource.Dash;
            HuboVisible = false;
            NoteVisible = false;
            LocationVisible = false;
        }

        private void CancelFromBreak()
        {
            MessagingCenter.Send<string>("AddBreak", "Failure");
            Navigation.PopModalAsync();
        }

        private void SaveNote()
        {
            DbService.SaveNote(Note, Date);
            Navigation.PopAsync();
        }
        public void Load(int instruction, int vehicleKey = 0, bool driveActive = false)
        {
            //Add Note button clicked, hubo not required
            if (instruction == 1)
            {
                SaveCommand = new Command(SaveNote);
                HuboLabel = Resource.HuboNotRequired;
                CancelCommand = new Command(Cancel);
            }
            else if (instruction == 2)
            {
                SaveCommand = new Command(StartBreak);
                HuboLabel = Resource.Hubo;
                CancelCommand = new Command(CancelFromBreak);
                LocationVisible = true;
                HuboVisible = true;
            }
            else if (instruction == 3)
            {
                SaveCommand = new Command(EndBreak);
                HuboLabel = Resource.Hubo;
                CancelCommand = new Command(CancelFromBreak);
                LocationVisible = true;
                HuboVisible = true;
            }
            //Load details for attaching note to currentvehicle
            else if (instruction == 4)
            {
                CurrentVehicleKey = vehicleKey;
                DriveActive = driveActive;
                HuboLabel = Resource.Hubo;
                CancelCommand = new Command(Cancel);
                SaveCommand = new Command(SaveDrive);
                HuboVisible = true;
                LocationVisible = true;
            }

            else if (instruction == 5)
            {
                SaveCommand = new Command(StartShift);
                CancelCommand = new Command(Cancel);
                HuboLabel = Resource.Hubo;
                LocationVisible = true;
            }
            else if (instruction == 6)
            {
                SaveCommand = new Command(EndShift);
                CancelCommand = new Command(Cancel);
                HuboLabel = Resource.Hubo;
                LocationVisible = true;
            }


            OnPropertyChanged("SaveCommand");
            OnPropertyChanged("CancelCommand");
            OnPropertyChanged("HuboLabel");
            OnPropertyChanged("HuboVisible");
            OnPropertyChanged("NoteVisible");
            OnPropertyChanged("LocationVisible");
        }

        private void EndShift()
        {
            DbService.StopShift(Date, double.Parse(LocationLat), double.Parse(LocationLong));
            Navigation.PopAsync();
        }

        private void StartShift()
        {
            DbService.StartShift(Date, double.Parse(LocationLat), double.Parse(LocationLong));
            Navigation.PopAsync();
        }

        private void SaveDrive()
        {
            if (CheckValidEntry())
            {
                DbService.SaveDrive(DriveActive, Date, Int32.Parse(HuboEntry), CurrentVehicleKey);
                Navigation.PopAsync();
            }
        }

        private void Cancel()
        {
            Navigation.PopModalAsync();
        }

        private async void StartBreak()
        {
            if (CheckValidEntry())
            {
                await DbService.StartBreak(Location);
                MessagingCenter.Send<string>("Success", "AddBreak");
                await Navigation.PopModalAsync();
            }
        }

        private async void EndBreak()
        {
            if (CheckValidEntry())
            {
                await DbService.StopBreak(Location);
                MessagingCenter.Send<string>("Success", "AddBreak");
                await Navigation.PopModalAsync();
            }
        }

        private bool CheckValidEntry()
        {
            Regex regex = new Regex("^[0-9]+$");
            if (HuboEntry.Length == 0)
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.InvalidHubo, Resource.DisplayAlertOkay);
                return false;
            }
            if (!(regex.IsMatch(HuboEntry)))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.InvalidHubo, Resource.DisplayAlertOkay);
                return false;
            }

            if (Location.Length == 0)
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.InputLocation, Resource.DisplayAlertOkay);
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
