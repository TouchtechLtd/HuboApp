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
        public event PropertyChangedEventHandler PropertyChanged;

        public bool HamburgerSettings
        {
            get { return Settings.HamburgerSettings; }
            set
            {
                Settings.HamburgerSettings = value;
                OnPropertyChanged("HamburgerSettings");
            }
        }

        public bool DarkLightSetting
        {
            get { return Settings.DarkLightSetting; }
            set
            {
                Settings.DarkLightSetting = value;
                OnPropertyChanged("DarkLightSetting");
            }
        }

        public SettingsViewModel()
        {
            //HamburgerSettings = Settings.HamburgerSettings;

            
        }

        public async void Restart()
        {
            await Application.Current.MainPage.DisplayAlert("Layout Change", "For this setting to take affect, the app will now close", "OK");
            var closer = DependencyService.Get<ICloseApplication>();
            if (closer != null)
                closer.closeApplication();
        }

        

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
