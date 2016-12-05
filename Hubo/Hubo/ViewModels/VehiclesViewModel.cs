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
        public List<VehicleTable> listOfVehicles;
        public List<string> listOfVehicleRegistrations;
        public VehicleTable currentVehicle;
        DatabaseService DbService = new DatabaseService();

        public INavigation Navigation { get; set; }
        public string SearchVehiclesPlaceholder { get; set; }
        public bool VehicleActive { get; set; }
        public string RegistrationText { get; set; }
        public string RegistrationEntry { get; set; }
        public string AddRegistrationEntry { get; set; }
        public string MakeText { get; set; }
        public string MakeEntry { get; set; }
        public string AddMakeEntry { get; set; }
        public string ModelText { get; set; }
        public string ModelEntry { get; set; }
        public string AddModelEntry { get; set; }
        public string CompanyText { get; set; }
        public string CompanyEntry { get; set; }
        public string AddCompanyEntry { get; set; }
        public string SwitchText { get; set; }
        public string EditVehicleText { get; set; }
        public string AddVehicleText { get; set; }
        public string SaveText { get; set; }
        public string CancelText { get; set; }
        public string UseOrStopVehicleText { get; set; }
        public ICommand EditVehicleCommand { get; set; }
        public ICommand AddVehicleCommand { get; set; }
        public ICommand SearchVehiclesCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ToggleVehicleUseCommand { get; set; }
        public Color UseVehicleColor { get; set; }
        public bool UseVehicleButtonVisible { get; set; }
        public bool VehiclesPageFromMenu { get; set; }
        public bool VehicleInUse { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        

        public VehiclesViewModel(int instruction = 0)
        {
            UpdateLabels();
            GetVehicles();

            currentVehicle = new VehicleTable();

            EditVehicleCommand = new Command(EditVehicle);

            CancelText = Resource.Cancel;
            SaveText = Resource.Save;

            SaveCommand = new Command(SaveVehicleDetails);
            AddVehicleCommand = new Command(AddVehicle);
            AddCommand = new Command(InsertVehicle);
            CancelCommand = new Command(Cancel);

            ToggleVehicleUseCommand = new Command(ToggleVehicleUse);

            if (instruction == 1)
            {
                VehiclesPageFromMenu = true;
                UseVehicleButtonVisible = false;
            }
            else if (instruction == 2)
            {
                VehiclesPageFromMenu = false;
                UseVehicleButtonVisible = true;
            }
            OnPropertyChanged("VehiclesPageFromMenu");
            OnPropertyChanged("UseVehicleButtonVisible");

            ToggleVehicleInUseVisuals();

            if (DbService.VehicleActive())
            {
                VehicleActive = true;
                currentVehicle = DbService.GetCurrentVehicle();
                RegistrationText = "Registration: " + currentVehicle.Registration;
                MakeText = "Make: " + currentVehicle.Make;
                ModelText = "Model: " + currentVehicle.Model;
                CompanyText = "Company: " + currentVehicle.Company;
                SwitchText = Resource.SwitchTextActive;
                OnPropertyChanged("RegistrationText");
                OnPropertyChanged("MakeText");
                OnPropertyChanged("ModelText");
                OnPropertyChanged("CompanyText");
            }
            MessagingCenter.Subscribe<string>("UpdateVehicleInUse", "UpdateVehicleInUse", (sender) =>
            {
                ToggleVehicleInUseVisuals();
            });
        }

        private void ToggleVehicleInUseVisuals()
        {
            if (DbService.VehicleInUse())
            {
                UseVehicleColor = Color.Red;
                UseOrStopVehicleText = Resource.StopUsingVehicle;
                VehicleInUse = true;
            }
            else
            {
                UseVehicleColor = Color.Green;
                UseOrStopVehicleText = Resource.UseVehicle;
                VehicleInUse = false;
            }
            OnPropertyChanged("VehicleInUse");
            OnPropertyChanged("UseOrStopVehicleText");
            OnPropertyChanged("UseVehicleColor");
        }

        private void ToggleVehicleUse(object obj)
        {
            if(currentVehicle.Registration!=null)
            {
                if (VehicleInUse)
                {
                    // Code to switch used vehicle off. 1) change visual elements, 2) code to toggle active off, 3)Code to open new page to input rego information
                    Navigation.PushAsync(new AddNotePage(4, currentVehicle.Key));
                }
                else
                {
                    //Code to switch vehicle on Reverse of previous TODO
                    //Navigation.PushAsync(new VehicleChecklistPage(1, true,currentVehicle.Key));
                    Navigation.PushAsync(new AddNotePage(4, currentVehicle.Key));
                }
            }
            else
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "PLEASE SELECT A VEHICLE", Resource.DisplayAlertOkay);
            }
            
        }

        private void Cancel()
        {
            MessagingCenter.Send<string>("UpdateVehicles", "UpdateVehicles");
        }

        public void InsertVehicle()
        {
            VehicleTable VehicleToAdd = new VehicleTable();
            VehicleToAdd = BindXAMLToVehicle();
            DbService.InsertVehicle(VehicleToAdd);
        }

        private void AddVehicle()
        {
            Navigation.PushAsync(new AddVehiclePage());
        }

        public List<VehicleTable> GetVehicles()
        {
            listOfVehicles = new List<VehicleTable>();
            listOfVehicles = DbService.GetVehicles();
            UpdateLabels();
            return listOfVehicles;
        }

        private VehicleTable BindXAMLToVehicle()
        {
            VehicleTable editedVehicle = new VehicleTable();
            editedVehicle.Company = CompanyEntry;
            editedVehicle.Make = MakeEntry;
            editedVehicle.Model = ModelEntry;
            editedVehicle.Registration = RegistrationEntry;
            return editedVehicle;
        }

        public void SaveVehicleDetails()
        {
            VehicleTable editedVehicle = new VehicleTable();
            editedVehicle = BindXAMLToVehicle();
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
            if (currentVehicle.Registration == null)
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.ChooseVehicleToEdit, Resource.DisplayAlertOkay);
            }
            else
            {
                Navigation.PushAsync(new EditVehiclePage(currentVehicle));
            }
        }

        internal void UpdatePage(int selectedVehicle)
        {
            VehicleTable vehicle = listOfVehicles[selectedVehicle];
            RegistrationText = "Registration: " + vehicle.Registration;
            MakeText = "Make: " + vehicle.Make;
            ModelText = "Model: " + vehicle.Model;
            CompanyText = "Company: " + vehicle.Company;
            VehicleActive = false;
            SwitchText = Resource.SwitchTextInActive;
            OnPropertyChanged("RegistrationText");
            OnPropertyChanged("MakeText");
            OnPropertyChanged("ModelText");
            OnPropertyChanged("CompanyText");
            OnPropertyChanged("VehicleActive");
            OnPropertyChanged("SwitchText");
            currentVehicle = vehicle;
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
