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
        public ObservableCollection<Vehicle> listOfVehicles;
        public List<string> listOfVehicleRegistrations;
        public Vehicle currentVehicle;

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
            listOfVehicles = new ObservableCollection<Vehicle>();
            listOfVehicleRegistrations = new List<string>();
            Vehicle test1 = new Vehicle();
            Vehicle test2 = new Vehicle();
            Vehicle test3 = new Vehicle();
            Vehicle test4 = new Vehicle();
            test1.Company = "BD14";
            test2.Company = "BD14";
            test3.Company = "BD14";
            test4.Company = "BD14";

            test1.Make = "Mack";
            test2.Make = "Freightliner";
            test3.Make = "Western Star";
            test4.Make = "Kenworth";

            test1.Model = "Example 1";
            test2.Model = "Example 2";
            test3.Model = "Example 3";
            test4.Model = "Example 4";

            test1.Registration = "BSK474";
            test2.Registration = "YHG072";
            test3.Registration = "HED889";
            test4.Registration = "LWP127";

            listOfVehicleRegistrations.Add(test1.Registration);
            listOfVehicleRegistrations.Add(test2.Registration);
            listOfVehicleRegistrations.Add(test3.Registration);
            listOfVehicleRegistrations.Add(test4.Registration);

            listOfVehicles.Add(test1);
            listOfVehicles.Add(test2);
            listOfVehicles.Add(test3);
            listOfVehicles.Add(test4);

            currentVehicle = new Vehicle();

            EditVehicleCommand = new Command(EditVehicle);

            CancelText = Resource.Cancel;
            SaveText = Resource.Save;
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
            foreach(Vehicle vehicle in listOfVehicles)
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
            foreach (Vehicle vehicle in listOfVehicles)
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
