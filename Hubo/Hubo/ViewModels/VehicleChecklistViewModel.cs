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
    class VehicleChecklistViewModel : INotifyPropertyChanged
    {
        DatabaseService DbService = new DatabaseService();

        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }

        public string HuboText { get; set; }
        public string HuboEntry { get; set; }
        public string ContinueText { get; set; }
        public ICommand ContinueCommand { get; set; }
        public int CurrentVehicleKey { get; set; }
        public VehicleChecklistViewModel()
        {
            HuboText = Resource.Hubo;
            ContinueText = Resource.Continue;
            HuboEntry = "";
        }

        internal void Load(int instruction)
        {
            if(instruction==1)
            {
                //Start Hubo
                ContinueCommand = new Command(Continue);
                OnPropertyChanged("ContinueCommand");
            }
            else if(instruction==2)
            {
                //End Hubo
                ContinueCommand = new Command(Finish);
                OnPropertyChanged("ContinueCommand");
            }
            else if(instruction==3)
            {
                ContinueCommand = new Command(FinishAndMessage);
                OnPropertyChanged("ContinueCommand");
            }
        }

        private void FinishAndMessage()
        {
            if (CheckValidEntry())
            {
                DbService.StopVehicleInUse(HuboEntry);
                MessagingCenter.Send<string>("Success", "EndShiftRegoEntered");
                Navigation.PopModalAsync();

            }

        }
        private void Finish()
        {
            if(CheckValidEntry())
            {
                DbService.StopVehicleInUse(HuboEntry);
                Navigation.PopAsync();
            }

        }

        private void Continue()
        {
            if(CheckValidEntry())
            {
                DbService.SetVehicleInUse(CurrentVehicleKey, HuboEntry);
                Navigation.PopAsync();
            }

        }

        private bool CheckValidEntry()
        {
            Regex regex = new Regex("^[0-9]+$");
            if (HuboEntry.Length == 0)
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "PLEASE INPUT A VALID REGO", Resource.DisplayAlertOkay);
                return false;
            }
            if (!(regex.IsMatch(HuboEntry)))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "PLEASE INPUT A VALID REGO", Resource.DisplayAlertOkay);
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
