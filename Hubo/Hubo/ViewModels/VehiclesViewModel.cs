// <copyright file="VehiclesViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Acr.UserDialogs;
    using Xamarin.Forms;
    using XLabs;

    public class VehiclesViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService dbService = new DatabaseService();
        private readonly RestService restAPI = new RestService();
        private List<VehicleTable> listOfVehicles;
        private List<CompanyTable> listOfCompanies;
        private VehicleTable currentVehicle;

        public VehiclesViewModel()
        {
            VehicleSelected = false;
            VehicleAddSelected = false;
            VehicleEditSelected = true;
            UpdateLabels();
            GetVehicles();
            currentVehicle = new VehicleTable();
            EditVehicleCommand = new Command(EditVehicle);
            AddVehicleCommand = new RelayCommand(async () => await InsertVehicle());
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public async Task InsertVehicle()
        {
            if (VehicleAddSelected)
            {
                VehicleTable vehicleToAdd = BindXAMLToVehicle();

                if (vehicleToAdd.Registration == string.Empty)
                {
                    return;
                }

                if (vehicleToAdd.MakeModel == string.Empty)
                {
                    return;
                }

                if (vehicleToAdd.FleetNumber == string.Empty)
                {
                    return;
                }

                if (await restAPI.QueryAddVehicle(vehicleToAdd))
                {
                    vehicleToAdd = dbService.InsertVehicle(vehicleToAdd);
                    GetVehicles();
                    MessagingCenter.Send<string, int>("UpdateVehicles", "UpdateVehicles", vehicleToAdd.Key);
                }
                else
                {
                    vehicleToAdd = dbService.InsertVehicle(vehicleToAdd);
                    dbService.VehicleOffine(vehicleToAdd);
                    GetVehicles();
                    MessagingCenter.Send<string, int>("UpdateVehicles", "UpdateVehicles", vehicleToAdd.Key);
                }
            }
        }

        public void UpdatePageAdd()
        {
            RegistrationEntry = string.Empty;
            MakeModelEntry = string.Empty;
            FleetEntry = string.Empty;
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
            listOfVehicles = dbService.GetVehicles();

            UpdateLabels();
            return listOfVehicles;
        }

        public List<CompanyTable> GetCompanies()
        {
            listOfCompanies = dbService.GetCompanies();

            return listOfCompanies;
        }

        public void SaveVehicleDetails()
        {
            if (VehicleEditSelected)
            {
                VehicleTable editedVehicle = BindXAMLToVehicle();
                editedVehicle.Key = currentVehicle.Key;

                if (editedVehicle.Registration != currentVehicle.Registration || editedVehicle.MakeModel != currentVehicle.MakeModel || editedVehicle.CompanyId != currentVehicle.CompanyId || editedVehicle.FleetNumber != currentVehicle.FleetNumber)
                {
                    dbService.UpdateVehicleInfo(editedVehicle);
                    MessagingCenter.Send<string, int>("UpdateVehicles", "UpdateVehicles", editedVehicle.Key);
                }
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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private VehicleTable BindXAMLToVehicle()
        {
            VehicleTable editedVehicle = new VehicleTable()
            {
                CompanyId = SelectedCompany + 1,
                MakeModel = MakeModelEntry,
                Registration = RegistrationEntry,
                FleetNumber = FleetEntry
            };
            return editedVehicle;
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
                UserDialogs.Instance.ConfirmAsync(Resource.ChooseVehicleToEdit, Resource.Alert, Resource.DisplayAlertOkay);
            }
            else
            {
                SaveVehicleDetails();
            }
        }
    }
}