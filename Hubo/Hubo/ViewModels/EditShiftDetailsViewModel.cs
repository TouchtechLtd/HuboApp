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
    class EditShiftDetailsViewModel : INotifyPropertyChanged
    {
        public int instruction { get; set; }
        public INavigation Navigation { get; set; }
        public string VehicleStartHubo { get; set; }
        public string VehicleEndHubo { get; set; }
        public bool EditingVehicle { get; set; }
        public string SaveText { get; set; }
        public string CancelText { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        ShiftTable currentShift = new ShiftTable();
        DatabaseService DbService = new DatabaseService();

        List<VehicleInUseTable> listUsedVehicles = new List<VehicleInUseTable>();
        VehicleInUseTable currentVehicleInUse = new VehicleInUseTable();

        List<VehicleTable> listOfRegisteredVehicles = new List<VehicleTable>();
        VehicleTable currentRegisteredVehicle = new VehicleTable();

        List<AmendmentTable> listOfAmendments = new List<AmendmentTable>();
        
        public EditShiftDetailsViewModel()
        {
            VehicleStartHubo = "";
            VehicleEndHubo = "";
            EditingVehicle = false;
        }

        internal void Load(int instructionNo, ShiftTable shift)
        {
            currentShift = shift;
            instruction = instructionNo;
            EditingVehicle = false;
            SaveText = Resource.Save;
            CancelText = Resource.Cancel;
            SaveCommand = new Command(Save);
            CancelCommand = new Command(Cancel);
            OnPropertyChanged("SaveText");
            OnPropertyChanged("CancelText");
            OnPropertyChanged("SaveCommand");
            OnPropertyChanged("CancelCommand");
        }

        private void Cancel()
        {
            Navigation.PopAsync();
        }

        private void Save()
        {
            if(instruction==3)
            {
                if(CheckValidHuboEntry() && EditingVehicle)
                {
                    if (VehicleStartHubo != currentVehicleInUse.HuboStart.ToString())
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.BeforeValue = currentVehicleInUse.HuboStart.ToString();
                        newAmendment.Field = "HuboStart";
                        newAmendment.ShiftId = currentShift.Key;
                        newAmendment.Table = "VehicleInUseTable";
                        newAmendment.TimeStamp = DateTime.Now.ToString();
                        currentVehicleInUse.HuboStart = Int32.Parse(VehicleStartHubo);
                        listOfAmendments.Add(newAmendment);
                    }
                    if (VehicleEndHubo != currentVehicleInUse.HuboEnd.ToString())
                    {
                        AmendmentTable newAmendment = new AmendmentTable();
                        newAmendment.BeforeValue = currentVehicleInUse.HuboEnd.ToString();
                        newAmendment.Field = "HuboEnd";
                        newAmendment.ShiftId = currentShift.Key;
                        newAmendment.Table = "VehicleInUseTable";
                        newAmendment.TimeStamp = DateTime.Now.ToString();
                        currentVehicleInUse.HuboEnd = Int32.Parse(VehicleEndHubo);
                        listOfAmendments.Add(newAmendment);
                    }
                    if (listOfAmendments.Count > 0)
                    {
                        DbService.AddAmendments(listOfAmendments, null, currentVehicleInUse);
                    }
                    Navigation.PopAsync();
                }
            }
        }

        private bool CheckValidHuboEntry()
        {
            Regex regex = new Regex("^[0-9]+$");
            if ((VehicleStartHubo.Length == 0)||(VehicleEndHubo.Length==0))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.InvalidHubo, Resource.DisplayAlertOkay);
                return false;
            }
            if (!(regex.IsMatch(VehicleStartHubo))||!(regex.IsMatch(VehicleEndHubo)))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.InvalidHubo, Resource.DisplayAlertOkay);
                return false;
            }

            return true;
        }

        internal List<VehicleInUseTable> LoadVehicles()
        {
            listUsedVehicles = DbService.GetUsedVehicles(currentShift);
            return listUsedVehicles;
        }

        internal VehicleTable LoadVehicleInfo(VehicleInUseTable vehicle)
        {
            return DbService.LoadVehicleInfo(vehicle);
        }

        internal void DisplayDetails(int selectedIndex)
        {
            if(instruction==3)
            {
                currentVehicleInUse = listUsedVehicles[selectedIndex];
                VehicleStartHubo = currentVehicleInUse.HuboStart.ToString();
                VehicleEndHubo = currentVehicleInUse.HuboEnd.ToString();
                EditingVehicle = true;
                OnPropertyChanged("VehicleStartHubo");
                OnPropertyChanged("VehicleEndHubo");
                OnPropertyChanged("EditingVehicle");
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
    }
}
