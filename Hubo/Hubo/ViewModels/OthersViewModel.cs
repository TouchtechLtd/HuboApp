// <copyright file="OthersViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Xamarin.Forms;

    public class OthersViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService dbService = new DatabaseService();
        private string name;

        public OthersViewModel()
        {
            OthersPageList = PopulateMenuItems();
            Name = dbService.GetName();
            MessagingCenter.Subscribe<string>("ReloadOthersPage", "ReloadOthersPage", (s) =>
            {
                Name = dbService.GetName();
                MessagingCenter.Unsubscribe<string>("ReloadOthersPage", "ReloadOthersPage");
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<MenuItem> OthersPageList { get; set; }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private List<MenuItem> PopulateMenuItems()
        {
            List<MenuItem> items = new List<MenuItem>();

            MenuItem profile = new MenuItem()
            {
                Title = Resource.ProfileText,
                TargetType = "Profile",
                ImageSource = "UserWhite.png"
            };

            MenuItem settings = new MenuItem()
            {
                Title = Resource.SettingsText,
                TargetType = "Settings",
                ImageSource = "SettingsWhite.png"
            };

            MenuItem signOut = new MenuItem()
            {
                Title = Resource.SignOutText,
                TargetType = "SignOut",
                ImageSource = "ExitWhite.png"
            };

            items.Add(profile);
            items.Add(settings);
            items.Add(signOut);

            return items;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
