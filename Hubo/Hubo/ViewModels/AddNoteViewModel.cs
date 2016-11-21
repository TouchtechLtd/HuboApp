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
    class AddNoteViewModel :INotifyPropertyChanged
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

        DatabaseService DbService = new DatabaseService();

        public event PropertyChangedEventHandler PropertyChanged;

        public AddNoteViewModel()
        {
            SaveText = Resource.Save;
            Date = DateTime.Now;
            HuboEntry = "";
            Note = "";
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
            DbService.SaveNote(Note, Date);
            Navigation.PopAsync();                        
        }
        public void Load(int instruction)
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
            else
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "ERROR: WRONG INSTRUCTION NUMBER IDENTIFIED", Resource.DisplayAlertOkay);
            }
            OnPropertyChanged("SaveCommand");
            OnPropertyChanged("CancelCommand");
            OnPropertyChanged("HuboLabel");
        }

        private void Cancel()
        {
            Navigation.PopAsync();
        }

        private void SaveNoteWithHubo()
        {
            if (CheckValidEntry())
            {
                DbService.SaveNoteFromBreak(Note, Date, Int32.Parse(HuboEntry));
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
            if(Note.Length==0)
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.EmptyNote, Resource.DisplayAlertOkay);
                return false;
            }
            if (!(regex.IsMatch(HuboEntry)))
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
