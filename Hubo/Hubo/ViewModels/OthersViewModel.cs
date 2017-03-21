// <copyright file="OthersViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Collections.Generic;

    public class OthersViewModel
    {
        private readonly DatabaseService dbService = new DatabaseService();

        public OthersViewModel()
        {
            OthersPageList = PopulateMenuItems();
            Name = dbService.GetName();
        }

        public List<MenuItem> OthersPageList { get; set; }

        public string Name { get; set; }

        private List<MenuItem> PopulateMenuItems()
        {
            List<MenuItem> items = new List<MenuItem>();

            MenuItem profile = new MenuItem()
            {
                Title = Resource.ProfileText,
                TargetType = "Profile",
                ImageSource = "User96.png"
            };

            MenuItem settings = new MenuItem()
            {
                Title = Resource.SettingsText,
                TargetType = "Settings",
                ImageSource = "Settings25.png"
            };

            MenuItem signOut = new MenuItem()
            {
                Title = Resource.SignOutText,
                TargetType = "SignOut",
                ImageSource = "Exit96.png"
            };
            items.Add(profile);
            items.Add(settings);
            items.Add(signOut);

            return items;
        }
    }
}
