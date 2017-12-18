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

        private string company;

        public VehiclesViewModel()
        {
            VehicleSelected = false;
            RegistrationText = Resource.RegistrationText;
            MakeModelText = Resource.MakeModelText;
            CompanyText = Resource.CompanyText;
            FleetText = Resource.FleetText;
            EditVehicleText = Resource.EditVehicleText;
            AddVehicleText = Resource.AddVehicleText;
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

        public bool VehicleSelected { get; set; }

        public int SelectedCompany { get; set; }

        public string FleetEntry { get; set; }

        public string FleetText { get; set; }

        public string Company
        {
            get
            {
                return company;
            }

            set
            {
                company = value;
                OnPropertyChanged("Company");
            }
        }

        public List<VehicleTable> GetVehicles()
        {
            listOfVehicles = new List<VehicleTable>();
            listOfVehicles = dbService.GetVehicles();

            return listOfVehicles;
        }

        public CompanyTable GetCompany(int id)
        {
            List<CompanyTable> listOfCompanies = new List<CompanyTable>();
            listOfCompanies = dbService.GetCompanies(id);

            if (listOfCompanies.Count != 1)
            {
                return null;
            }

            CompanyTable companyDetails = new CompanyTable();
            companyDetails = listOfCompanies[0];

            return companyDetails;
        }

        internal void UpdatePage(int selectedVehicle)
        {
            VehicleTable vehicle = new VehicleTable();
            vehicle = listOfVehicles[selectedVehicle];

            CompanyTable companyDetails = new CompanyTable();
            companyDetails = GetCompany(vehicle.CompanyId);

            RegistrationEntry = vehicle.Registration;
            MakeModelEntry = vehicle.MakeModel;
            FleetEntry = vehicle.FleetNumber;
            Company = companyDetails.Name;

            VehicleSelected = true;

            OnPropertyChanged("RegistrationEntry");
            OnPropertyChanged("MakeModelEntry");
            OnPropertyChanged("FleetEntry");
            OnPropertyChanged("VehicleActive");
            OnPropertyChanged("VehicleSelected");
            OnPropertyChanged("SwitchText");
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}