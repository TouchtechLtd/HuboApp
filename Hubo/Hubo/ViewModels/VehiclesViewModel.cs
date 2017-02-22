using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class VehiclesViewModel : INotifyPropertyChanged
    {
        public List<VehicleTable> listOfVehicles;
        public List<CompanyTable> listOfCompanies;
        public VehicleTable currentVehicle;
        DatabaseService DbService = new DatabaseService();
        RestService RestAPI = new RestService();

        private bool _isBusy;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }
        private string _loadingText;
        public string LoadingText
        {
            get
            {
                return _loadingText;
            }

            set
            {
                _loadingText = value;
                OnPropertyChanged("LoadingText");
            }
        }

        public INavigation Navigation { get; set; }
        public string RegistrationText { get; set; }
        public string RegistrationEntry { get; set; }
        public string MakeModelText { get; set; }
        public string MakeModelEntry { get; set; }
        public string CompanyText { get; set; }
        public string EditVehicleText { get; set; }
        public string AddVehicleText { get; set; }
        public ICommand EditVehicleCommand { get; set; }
        public ICommand AddVehicleCommand { get; set; }
        public bool VehicleSelected { get; set; }
        public bool VehicleAddSelected { get; set; }
        public bool VehicleEditSelected { get; set; }
        public int SelectedCompany { get; set; }
        public string FleetEntry { get; set; }
        public string FleetText { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        public VehiclesViewModel()
        {
            VehicleSelected = false;
            VehicleAddSelected = false;
            VehicleEditSelected = true;
            UpdateLabels();
            GetVehicles();

            currentVehicle = new VehicleTable();

            EditVehicleCommand = new Command(EditVehicle);
            AddVehicleCommand = new Command(InsertVehicle);

            OnPropertyChanged("VehiclesPageFromMenu");
            OnPropertyChanged("UseVehicleButtonVisible");

            IsBusy = false;
        }

        public async void InsertVehicle()
        {
            if (VehicleAddSelected)
            {
                VehicleTable VehicleToAdd = new VehicleTable();
                VehicleToAdd = BindXAMLToVehicle();

                if (VehicleToAdd.Registration == "")
                    return;

                if (VehicleToAdd.MakeModel == "")
                    return;

                if (VehicleToAdd.FleetNumber == "")
                    return;

                IsBusy = true;
                SetLoadingText();
                if (await RestAPI.QueryAddVehicle(VehicleToAdd))
                {
                    VehicleToAdd = DbService.InsertVehicle(VehicleToAdd);
                    GetVehicles();
                    IsBusy = false;
                    MessagingCenter.Send<string, int>("UpdateVehicles", "UpdateVehicles", VehicleToAdd.Key);
                }
                else
                {
                    VehicleToAdd = DbService.InsertVehicle(VehicleToAdd);
                    DbService.VehicleOffine(VehicleToAdd);
                    GetVehicles();
                    IsBusy = false;
                    MessagingCenter.Send<string, int>("UpdateVehicles", "UpdateVehicles", VehicleToAdd.Key);
                }
            }
        }

        public void UpdatePageAdd()
        {
            RegistrationEntry = "";
            MakeModelEntry = "";
            FleetEntry = "";
            VehicleSelected = true;
            VehicleAddSelected = true;
            VehicleEditSelected = false;

            OnPropertyChanged("VehicleSelected");
            OnPropertyChanged("VehicleAddSelected");
            OnPropertyChanged("VehicleEditSelected");
            OnPropertyChanged("RegistrationEntry");
            OnPropertyChanged("MakeModelEntry");
            OnPropertyChanged("FleetEntry");
        }

        public List<VehicleTable> GetVehicles()
        {
            listOfVehicles = DbService.GetVehicles();

            UpdateLabels();
            return listOfVehicles;
        }

        public List<CompanyTable> GetCompanies()
        {
            listOfCompanies = DbService.GetCompanies();

            return listOfCompanies;
        }

        private VehicleTable BindXAMLToVehicle()
        {
            VehicleTable editedVehicle = new VehicleTable();
            editedVehicle.CompanyId = SelectedCompany + 1;
            editedVehicle.MakeModel = MakeModelEntry;
            editedVehicle.Registration = RegistrationEntry;
            editedVehicle.FleetNumber = FleetEntry;
            return editedVehicle;
        }

        public void SaveVehicleDetails()
        {
            if (VehicleEditSelected)
            {
                VehicleTable editedVehicle = new VehicleTable();
                editedVehicle = BindXAMLToVehicle();
                editedVehicle.Key = currentVehicle.Key;

                if (editedVehicle.Registration != currentVehicle.Registration || editedVehicle.MakeModel != currentVehicle.MakeModel || editedVehicle.CompanyId != currentVehicle.CompanyId || editedVehicle.FleetNumber != currentVehicle.FleetNumber)
                {
                    DbService.UpdateVehicleInfo(editedVehicle);
                    MessagingCenter.Send<string, int>("UpdateVehicles", "UpdateVehicles", editedVehicle.Key);
                }
            }
        }

        private void UpdateLabels()
        {
            RegistrationText = Resource.RegistrationText;
            MakeModelText = Resource.MakeModelText;
            CompanyText = Resource.CompanyText;
            FleetText = Resource.FleetText;
            EditVehicleText = Resource.EditVehicleText;
            AddVehicleText = Resource.AddVehicleText;

            OnPropertyChanged("HuboText");
            OnPropertyChanged("RegistrationText");
            OnPropertyChanged("MakeText");
            OnPropertyChanged("ModelText");
            OnPropertyChanged("CompanyText");
            OnPropertyChanged("FleetText");
        }

        private void EditVehicle()
        {
            if (currentVehicle.Registration == null)
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.ChooseVehicleToEdit, Resource.DisplayAlertOkay);
            }
            else
            {
                SaveVehicleDetails();
                Application.Current.MainPage.DisplayAlert("Saved", "Changes have been saved", "Gotcha");
            }
        }

        internal int UpdatePage(int selectedVehicle)
        {
            VehicleTable vehicle = listOfVehicles[selectedVehicle];

            RegistrationEntry = vehicle.Registration;
            MakeModelEntry = vehicle.MakeModel;
            FleetEntry = vehicle.FleetNumber;

            VehicleSelected = true;
            VehicleAddSelected = false;
            VehicleEditSelected = true;

            OnPropertyChanged("RegistrationEntry");
            OnPropertyChanged("MakeModelEntry");
            OnPropertyChanged("FleetEntry");
            OnPropertyChanged("VehicleActive");
            OnPropertyChanged("VehicleSelected");
            OnPropertyChanged("SwitchText");
            currentVehicle = vehicle;

            return vehicle.CompanyId;
        }

        private void SetLoadingText()
        {
            List<LoadTextTable> loadText = new List<LoadTextTable>();
            loadText = DbService.GetLoadingText();

            Random random = new Random();

            int id = random.Next(1, loadText.Count);

            LoadingText = loadText[id - 1].LoadText;

            var sec = TimeSpan.FromSeconds(5);

            Device.StartTimer(sec, () =>
            {
                id = random.Next(1, loadText.Count);

                LoadingText = loadText[id - 1].LoadText;

                return IsBusy;
            });
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