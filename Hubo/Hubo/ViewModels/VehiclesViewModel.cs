using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hubo
{
    class VehiclesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Vehicle> listOfVehicles;
        public string SearchVehiclesPlaceholder { get; set; }
        public string RegistrationText { get; set; }
        public string MakeText { get; set; }
        public string ModelText { get; set; }
        public string CompanyText { get; set; }
        public string SwitchText { get; set; }
        public string EditVehicleText { get; set; }
        public string AddVehicleText { get; set; }
        public ICommand EditVehicleCommand { get; set; }
        public ICommand AddVehicleCommand { get; set; }
        public ICommand SearchVehiclesCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        public VehiclesViewModel()
        {
            SearchVehiclesPlaceholder = Resource.SearchVehiclesPlaceholder;
            RegistrationText = Resource.RegistrationText;
            MakeText = Resource.MakeText;
            ModelText = Resource.ModelText;
            CompanyText = Resource.CompanyText;
            SwitchText = Resource.SwitchTextInActive;
            EditVehicleText = Resource.EditVehicleText;
            AddVehicleText = Resource.AddVehicleText;
            listOfVehicles = new ObservableCollection<Vehicle>();
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
            listOfVehicles.Add(test1);
            listOfVehicles.Add(test2);
            listOfVehicles.Add(test3);
            listOfVehicles.Add(test4);
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
