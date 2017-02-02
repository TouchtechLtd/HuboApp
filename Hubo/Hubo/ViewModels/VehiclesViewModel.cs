﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class VehiclesViewModel : INotifyPropertyChanged
    {
        public List<VehicleTable> listOfVehicles;
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
        public string SearchVehiclesPlaceholder { get; set; }
        public bool VehicleActive { get; set; }
        public string RegistrationText { get; set; }
        public string RegistrationEntry { get; set; }
        public string AddRegistrationEntry { get; set; }
        public string HuboText { get; set; }
        public string HuboEntry { get; set; }
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
                CompanyText = "Company: " + currentVehicle.CompanyId;
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

            IsBusy = false;
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
            if (currentVehicle.Registration != null)
            {
                if (VehicleInUse)
                {
                    // Code to switch used vehicle off. 1) change visual elements, 2) code to toggle active off, 3)Code to open new page to input rego information
                    Navigation.PushAsync(new AddNotePage(4, currentVehicle.Key));
                }
                else
                {
                    //Code to switch vehicle on Reverse of previous TODO
                    Navigation.PushAsync(new AddNotePage(4, currentVehicle.Key));
                }
            }
            else
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "PLEASE SELECT A VEHICLE", Resource.DisplayAlertOkay);
            }

        }

        private async void Cancel()
        {
            await Navigation.PopAsync();
        }

        public async void InsertVehicle()
        {
            VehicleTable VehicleToAdd = new VehicleTable();
            VehicleToAdd = BindXAMLToVehicle();

            IsBusy = true;
            SetLoadingText();
            if (await RestAPI.QueryAddVehicle(VehicleToAdd))
            {
                DbService.InsertVehicle(VehicleToAdd);
                GetVehicles();
                IsBusy = false;
                await Navigation.PopAsync();
            }
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
            editedVehicle.CompanyId = CompanyEntry;
            editedVehicle.Make = MakeEntry;
            editedVehicle.Model = ModelEntry;
            editedVehicle.Registration = RegistrationEntry;
            editedVehicle.StartingOdometer = HuboEntry;
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
            HuboText = Resource.HuboEquals;
            OnPropertyChanged("HuboText");
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
            CompanyText = "Company: " + vehicle.CompanyId;
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
                    CompanyEntry = vehicle.CompanyId;
                    OnPropertyChanged("RegistrationEntry");
                    OnPropertyChanged("MakeEntry");
                    OnPropertyChanged("ModelEntry");
                    OnPropertyChanged("CompanyEntry");
                    currentVehicle = vehicle;
                }
            }
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
