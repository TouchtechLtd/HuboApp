﻿// <copyright file="ProfileViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Xamarin.Forms;
    using XLabs;

    public class ProfileViewModel : INotifyPropertyChanged
    {
        private DatabaseService dbService = new DatabaseService();

        private UserTable user = new UserTable();

        public ProfileViewModel()
        {
            SaveAndExit = new RelayCommand(() => SaveAndPop());
            CancelAndExit = new RelayCommand(async () => await CancelAndPop());

            UserNameText = Resource.UserName;
            FirstNameText = Resource.FirstName;
            LastNameText = Resource.LastName;
            EmailText = Resource.Email;
            PasswordText = Resource.Password;
            AddressText = Resource.Address;
            PhoneText = Resource.Phone;
            PostCodeText = Resource.PostCode;

            LicenseNumberText = Resource.LicenseNumber;
            LicenseVersionText = Resource.LicenseVersion;
            EndorsementsText = Resource.Endorsements;

            SuburbText = Resource.SuburbText;
            CityText = Resource.CityText;
            CountryText = Resource.CountryText;

            GetUserInfo();
            Companies = dbService.GetCompanyInfo(user.DriverId);
            Licences = dbService.GetLicenceInfo(user.DriverId);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SaveAndExit { get; set; }

        public ICommand CancelAndExit { get; set; }

        public INavigation Navigation { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string Phone { get; set; }

        public string FirstNameText { get; set; }

        public string LastNameText { get; set; }

        public string EmailText { get; set; }

        public string PasswordText { get; set; }

        public string LicenseNumberText { get; set; }

        public string LicenseVersionText { get; set; }

        public string EndorsementsText { get; set; }

        public string CompanyNameText { get; set; }

        public string AddressText { get; set; }

        public string CompanyEmailText { get; set; }

        public string PhoneText { get; set; }

        public string UserName { get; set; }

        public string PostCode { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string SuburbText { get; set; }

        public string CityText { get; set; }

        public string CountryText { get; set; }

        public string UserNameText { get; set; }

        public string PostCodeText { get; set; }

        public List<CompanyTable> Companies { get; set; }

        public List<LicenceTable> Licences { get; set; }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void GetUserInfo()
        {
            dbService = new DatabaseService();
            user = dbService.GetUserInfo();

            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Address1 = user.Address1;
            Address2 = user.Address2;
            Address3 = user.Address3;
            PostCode = user.PostCode;
            City = user.City;
            Country = user.Country;
            Phone = user.Phone.ToString();

            OnPropertyChanged("UserName");
            OnPropertyChanged("FirstName");
            OnPropertyChanged("LastName");
            OnPropertyChanged("Email");
            OnPropertyChanged("Address1");
            OnPropertyChanged("Address2");
            OnPropertyChanged("Address3");
            OnPropertyChanged("PostCode");
            OnPropertyChanged("City");
            OnPropertyChanged("Country");
            OnPropertyChanged("Phone");
        }

        private void SaveAndPop()
        {
            UserTable userChanges = new UserTable();

            userChanges = user;

            userChanges.UserName = UserName.Trim();
            userChanges.FirstName = FirstName.Trim();
            userChanges.LastName = LastName.Trim();
            userChanges.Email = Email.Trim();
            userChanges.Address1 = Address1.Trim();
            if (Address2 != null)
            {
                userChanges.Address2 = Address2.Trim();
            }

            if (Address3 != null)
            {
                userChanges.Address3 = Address3.Trim();
            }

            userChanges.PostCode = PostCode.Trim();
            userChanges.City = City.Trim();
            userChanges.Country = Country.Trim();
            userChanges.Phone = int.Parse(Phone.Trim());
        }

        private async Task CancelAndPop()
        {
            await Navigation.PopModalAsync();
        }
    }
}
