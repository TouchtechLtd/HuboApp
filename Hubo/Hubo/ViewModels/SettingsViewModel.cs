// <copyright file="SettingsViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.ComponentModel;

    internal class SettingsViewModel : INotifyPropertyChanged
    {
        public SettingsViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool DarkLightSetting
        {
            get
            {
                return Settings.DarkLightSetting;
            }

            set
            {
                if (Settings.DarkLightSetting != value)
                {
                    Settings.DarkLightSetting = value;
                    OnPropertyChanged("DarkLightSetting");
                }
            }
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
