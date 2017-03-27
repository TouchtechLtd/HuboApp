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
                OnPropertyChangedAsync("HamburgerSettings");
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
                OnPropertyChangedAsync("DarkLightSetting");
            }
        }

        public async Task Restart()
        {
            await UserDialogs.Instance.ConfirmAsync(Resource.LayoutChange, Resource.LayoutChangeTitle, Resource.Okay);
            DependencyService.Get<ICloseApplication>().CloseApplication();
        }

        public async Task OnPropertyChangedAsync(string name)
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
