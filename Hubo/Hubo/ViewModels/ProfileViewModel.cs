using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class ProfileViewModel : INotifyPropertyChanged
    {
        public ICommand SaveAndExit { get; set; }
        public ICommand CancelAndExit { get; set; }
        public INavigation Navigation { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseVersion { get; set; }
        public string Endorsements { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string CompanyEmail { get; set; }
        public string Phone { get; set; }
        public string FirstNameText { get; set; }
        public string LastNameText { get; set; }
        public string EmailText { get; set; }
        public string PasswordText { get; set; }
        public string LicenseNumberText { get; set; }
        public string LicenseVersionText { get; set; }
        public string EndorsementsText { get; set; }
        public string NameText { get; set; }
        public string AddressText { get; set; }
        public string CompanyEmailText { get; set; }
        public string PhoneText { get; set; }



        DatabaseService dbService;

        RestService restService;

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

        UserTable user;

        CompanyTable company;

        public ProfileViewModel()
        {
            SaveAndExit = new Command(SaveAndPop);
            CancelAndExit = new Command(CancelAndPop);
            FirstNameText = Resource.FirstName;
            LastNameText = Resource.LastName;
            EmailText = Resource.Email;
            PasswordText = Resource.Password;
            LicenseNumberText = Resource.LicenseNumber;
            LicenseVersionText = Resource.LicenseVersion;
            EndorsementsText = Resource.Endorsements;
            NameText = Resource.Name;
            AddressText = Resource.Address;
            CompanyEmailText = Resource.Email;
            PhoneText = Resource.Phone;
            GetUserInfo();
        }

        private void GetUserInfo()
        {

            dbService = new DatabaseService();
            user = new UserTable();
            company = new CompanyTable();
            user = dbService.GetUserInfo();
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            LicenseNumber = user.License;
            LicenseVersion = user.LicenseVersion;
            Endorsements = user.Endorsements;
            Name = company.Name;
            Address = user.Address;
            CompanyEmail = user.Email;
            Phone = user.Phone;
            OnPropertyChanged("FirstName");
            OnPropertyChanged("LastName");
            OnPropertyChanged("Email");
            OnPropertyChanged("LicenseNumber");
            OnPropertyChanged("LicenseVersion");
            OnPropertyChanged("Endorsements");
            OnPropertyChanged("Name");
            OnPropertyChanged("Address");
            OnPropertyChanged("CompanyEmail");
            OnPropertyChanged("Phone");
        }

        private void CancelAndPop(object obj)
        {
            Navigation.PopModalAsync();
        }

        private async void SaveAndPop(object obj)
        {
            user = new UserTable();
            user.FirstName = FirstName.Trim();
            user.LastName = LastName.Trim();
            user.Email = Email.Trim();
            user.License = LicenseNumber.Trim();
            user.LicenseVersion = LicenseVersion.Trim();
            user.Endorsements = Endorsements.Trim();
            company.Name = Name.Trim();
            user.Address = Address.Trim();
            user.Email = CompanyEmail.Trim();
            user.Phone = Phone.Trim();

            restService = new RestService();
            IsBusy = true;
            if (await restService.QueryUpdateUserInfo(user))
            {
                IsBusy = false;
                await Navigation.PopModalAsync();
            }
            else
            {
                IsBusy = false;
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
