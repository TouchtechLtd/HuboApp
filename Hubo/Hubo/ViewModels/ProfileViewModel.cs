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
        public string Endorsements { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string CompanyEmail { get; set; }
        public string Phone { get; set; }

        DatabaseService dbService;

        public ProfileViewModel()
        {
            SaveAndExit = new Command(SaveAndPop);
            CancelAndExit = new Command(CancelAndPop);
            GetUserInfo();
        }

        private void GetUserInfo()
        {
            dbService = new DatabaseService();
            UserTable user = new UserTable();
            user = dbService.GetUserInfo();
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            OnPropertyChanged("FirstName");
            OnPropertyChanged("LastName");
            OnPropertyChanged("Email");
        }

        private void CancelAndPop(object obj)
        {
            Navigation.PopModalAsync();
        }

        private void SaveAndPop(object obj)
        {
            //TODO: Implement save of details written in
            Navigation.PopModalAsync();
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
