using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Hubo.Helpers;

namespace Hubo
{
    class SettingsViewModel : INotifyPropertyChanged
    {
        public bool HamburgerSettings
        {
            get { return Settings.HamburgerSettings; }
            set
            {
                Settings.HamburgerSettings = value;
                OnPropertyChanged("HamburgerSettings");
            }
        }

        public SettingsViewModel()
        {
            HamburgerSettings = Settings.HamburgerSettings;
            
        }

        public void Restart()
        {
            Application.Current.MainPage.DisplayAlert("Layout Change", "For this setting to take affect please restart the App", "OK");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
                if (name == "HamburgerSettings")
                    Restart();
            }
        }
    }
}
