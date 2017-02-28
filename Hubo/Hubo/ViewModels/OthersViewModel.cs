// <copyright file="OthersViewModel.cs" company="Trio Technology LTD">
// Copyright (c) Trio Technology LTD. All rights reserved.
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

            MenuItem profile = new MenuItem();
            profile.Title = Resource.ProfileText;
            profile.TargetType = "Profile";
            profile.ImageSource = "User96.png";

            MenuItem settings = new MenuItem();
            settings.Title = Resource.SettingsText;
            settings.TargetType = "Settings";
            settings.ImageSource = "Settings25.png";

            MenuItem signOut = new MenuItem();
            signOut.Title = Resource.SignOutText;
            signOut.TargetType = "SignOut";
            signOut.ImageSource = "Exit96.png";

            items.Add(profile);
            items.Add(settings);
            items.Add(signOut);

            return items;
        }
    }
}
