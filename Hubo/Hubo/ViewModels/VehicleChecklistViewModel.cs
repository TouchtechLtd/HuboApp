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
        public string LocationText { get; set; }
        public string LocationEntry { get; set; }
        public string ContinueText { get; set; }
        public ICommand ContinueCommand { get; set; }
        public int CurrentVehicleKey { get; set; }
        public VehicleChecklistViewModel(int instruction)
        {
            HuboText = Resource.Hubo;
            LocationText = Resource.Location;
            ContinueText = Resource.Continue;
            HuboEntry = "";
            if (instruction == 1)
            {
                //Start Hubo
                ContinueCommand = new Command(Continue);
            }
            else if (instruction == 2)
            {
                //End Hubo
                ContinueCommand = new Command(Finish);
            }
            else if (instruction == 3)
            {
                ContinueCommand = new Command(FinishAndMessage);
            }
        }

        private void FinishAndMessage()
        {
            if (CheckValidEntry())
            {

                Navigation.PopModalAsync();
                DbService.SaveDrive(true, DateTime.Now, int.Parse(HuboEntry));
                MessagingCenter.Send<string>("Success", "EndShiftRegoEntered");
            }

        }
        private void Finish()
        {
            if(CheckValidEntry())
            {
                DbService.SaveDrive(true, DateTime.Now, int.Parse(HuboEntry));
                Navigation.PopAsync();
            }

        }

        private void Continue()
        {
            if(CheckValidEntry())
            {
                DriveTable drive = new DriveTable();
                drive.VehicleKey = CurrentVehicleKey;
                drive.StartDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                drive.StartHubo = int.Parse(HuboEntry);

                DbService.SaveDrive(false, DateTime.Now, int.Parse(HuboEntry), CurrentVehicleKey);
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
