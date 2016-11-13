using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class VehiclesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<VehicleTable> listOfVehicles;
        public List<string> listOfVehicleRegistrations;
        public VehicleTable currentVehicle;
        DatabaseService DbService = new DatabaseService();

        public INavigation Navigation { get; set; }
        public string SearchVehiclesPlaceholder { get; set; }
        public string RegistrationText { get; set; }
        public string RegistrationEntry { get; set; }
        public string MakeText { get; set; }
        public string MakeEntry { get; set; }
        public string ModelText { get; set; }
        public string ModelEntry { get; set; }
        public string CompanyText { get; set; }
        public string CompanyEntry { get; set; }
        public string SwitchText { get; set; }
        public string EditVehicleText { get; set; }
        public string AddVehicleText { get; set; }
        public string SaveText { get; set; }
        public string CancelText { get; set; }
        public ICommand EditVehicleCommand { get; set; }
        public ICommand AddVehicleCommand { get; set; }
        public ICommand SearchVehiclesCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        public VehiclesViewModel()
        {
            UpdateLabels();
            GetVehicles();

            currentVehicle = new VehicleTable();

            EditVehicleCommand = new Command(EditVehicle);

            CancelText = Resource.Cancel;
            SaveText = Resource.Save;

            SaveCommand = new Command(SaveVehicleDetails);

            
        }

        public List<string> GetVehicles()
        {
            listOfVehicles = new ObservableCollection<VehicleTable>();
            listOfVehicleRegistrations = new List<string>();

            listOfVehicles = DbService.GetVehicles();

            foreach (VehicleTable vehicle in listOfVehicles)
            {
                listOfVehicleRegistrations.Add(vehicle.Registration);
            }
            UpdateLabels();
            return listOfVehicleRegistrations;
                
        }

        private void SaveVehicleDetails()
        {
            VehicleTable editedVehicle = new VehicleTable();
            editedVehicle.Company = CompanyEntry;
            editedVehicle.Make = MakeEntry;
            editedVehicle.Model = ModelEntry;
            editedVehicle.Registration = RegistrationEntry;
            editedVehicle.Key = currentVehicle.Key;
            DbService.UpdateVehicleInfo(editedVehicle);
        }

        private void UpdateLabels()
        {
            SearchVehiclesPlaceholder = Resource.SearchVehiclesPlaceholder;
            RegistrationText = Resource.RegistrationText;
            MakeText = Resource.MakeText;
            ModelText = Resource.ModelText;
            CompanyText = Resource.CompanyText;
            SwitchText = Resource.SwitchTextInActive;
            EditVehicleText = Resource.EditVehicleText;
            AddVehicleText = Resource.AddVehicleText;
            OnPropertyChanged("RegistrationText");
            OnPropertyChanged("MakeText");
            OnPropertyChanged("ModelText");
            OnPropertyChanged("CompanyText");
        }

        private void EditVehicle()
        {
            if (currentVehicle == null)
            {

            }
            else
            {
                Navigation.PushAsync(new EditVehiclePage(currentVehicle));
            }
        }

        public void ToggleSwitch(bool toggle)
        {
            if(toggle)
            {
                SwitchText = Resource.SwitchTextActive;
            }
            else
            {
                SwitchText = Resource.SwitchTextInActive;
            }
            OnPropertyChanged("SwitchText");
        }

        internal void UpdatePage(string rego)
        {
            foreach(VehicleTable vehicle in listOfVehicles)
            {
                if(rego==vehicle.Registration)
                {
                    RegistrationText = "Registration: " + vehicle.Registration;
                    MakeText = "Make: " + vehicle.Make;
                    ModelText = "Model: " + vehicle.Model;
                    CompanyText = "Company: " + vehicle.Company;
                    OnPropertyChanged("RegistrationText");
                    OnPropertyChanged("MakeText");
                    OnPropertyChanged("ModelText");
                    OnPropertyChanged("CompanyText");
                    currentVehicle = vehicle;
                }
            }
        }


        internal void UpdateEditPage(string registration)
        {
            UpdateLabels();
            foreach (VehicleTable vehicle in listOfVehicles)
            {
                if (registration == vehicle.Registration)
                {
                    RegistrationEntry = vehicle.Registration;
                    MakeEntry = vehicle.Make;
                    ModelEntry = vehicle.Model;
                    CompanyEntry = vehicle.Company;
                    OnPropertyChanged("RegistrationEntry");
                    OnPropertyChanged("MakeEntry");
                    OnPropertyChanged("ModelEntry");
                    OnPropertyChanged("CompanyEntry");
                    currentVehicle = vehicle;
                }
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
