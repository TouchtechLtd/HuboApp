// <copyright file="SettingsViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Acr.UserDialogs;
    using Hubo.Helpers;
    using Xamarin.Forms;

    internal class SettingsViewModel : INotifyPropertyChanged
    {
        public SettingsViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool HamburgerSettings
        {
            get
            {
                return Settings.HamburgerSettings;
            }

            set
            {
                if (Settings.HamburgerSettings == value)
                {
                    return;
                }

                Settings.HamburgerSettings = value;
                OnPropertyChanged("HamburgerSettings");
            }
        }

        public bool DarkLightSetting
        {
            get
            {
                return Settings.DarkLightSetting;
            }

            set
            {
                if (Settings.DarkLightSetting == value)
                {
                    return;
                }

                Settings.DarkLightSetting = value;
                OnPropertyChanged("DarkLightSetting");
            }
        }

        public async Task Restart()
        {
            await UserDialogs.Instance.ConfirmAsync(Resource.LayoutSettingText, Resource.LayoutSettingTitle, Resource.DisplayAlertOkay);
            var closer = DependencyService.Get<ICloseApplication>();
            if (closer != null)
            {
                closer.CloseApplication();
            }
        }

        public async void OnPropertyChanged(string name)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
                if (name == "HamburgerSettings")
                {
                    await Restart();
                }
            }
        }
    }
}
