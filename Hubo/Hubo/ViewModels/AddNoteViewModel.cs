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
        public string HuboLabel { get; set; }
        public string HuboEntry { get; set; }
        public string Location { get; set; }
        public int CurrentVehicleKey { get; set; }

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
        }

        private void CancelFromBreak()
        {
            MessagingCenter.Send<string>("AddBreak", "Failure");
            Navigation.PopModalAsync();
        }

        private void SaveNoteWithoutHubo()
        {
            if(HuboEntry!="")
            {
                if(!CheckValidEntry())
                {
                    return;
                }
            }
            if (Location.Length == 0)
            {
                Application.Current.MainPage.DisplayActionSheet(Resource.DisplayAlertTitle, "Please input a location", Resource.DisplayAlertOkay);
                return;
            }
            DbService.SaveNote(Note, Date, Location, 0);
            Navigation.PopAsync();                        
        }
        public void Load(int instruction, int vehicleKey = 0)
        {
            //Add Note button clicked, hubo not required
            if(instruction==1)
            {
                SaveCommand = new Command(SaveNoteWithoutHubo);
                HuboLabel = Resource.Hubo + "(Not Required)";
                CancelCommand = new Command(Cancel);
            }
            else if(instruction==2)
            {
                SaveCommand = new Command(SaveNoteWithHubo);
                HuboLabel = Resource.Hubo;
                CancelCommand = new Command(CancelFromBreak);
            }

            //Load details for attaching note to currentvehicle
            else if(instruction==4)
            {
                CurrentVehicleKey = vehicleKey;
                HuboLabel = Resource.Hubo;
                CancelCommand = new Command(Cancel);
                SaveCommand = new Command(SaveNoteFromVehicle);
            }
            else
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "ERROR: WRONG INSTRUCTION NUMBER IDENTIFIED", Resource.DisplayAlertOkay);
            }
            OnPropertyChanged("SaveCommand");
            OnPropertyChanged("CancelCommand");
            OnPropertyChanged("HuboLabel");
        }

        private void SaveNoteFromVehicle()
        {
            if(CheckValidEntry())
            {
                DbService.SaveNoteFromVehicle(Note, Date, Location, Int32.Parse(HuboEntry), CurrentVehicleKey);
                Navigation.PopAsync();
            }
        }

        private void Cancel()
        {
            Navigation.PopAsync();
        }

        private void SaveNoteWithHubo()
        {
            if (CheckValidEntry())
            {
                DbService.SaveNoteFromBreak(Note, Date, Location, Int32.Parse(HuboEntry));
                MessagingCenter.Send<string>("Success", "AddBreak");
                Navigation.PopModalAsync();
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
                Application.Current.MainPage.DisplayActionSheet(Resource.DisplayAlertTitle, "Please input a location", Resource.DisplayAlertOkay);
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
