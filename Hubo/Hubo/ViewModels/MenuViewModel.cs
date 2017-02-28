// <copyright file="MenuViewModel.cs" company="Trio Technology LTD">
// Copyright (c) Trio Technology LTD. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Collections.Generic;
    using Hubo.Helpers;

    public class MenuViewModel
    {
        private readonly DatabaseService dbService = new DatabaseService();

        public MenuViewModel()
        {
            MenuPageList = PopulateMenuItems();
            Name = dbService.GetName();
        }

        public List<MenuItem> MenuPageList { get; set; }

        public string Name { get; set; }

        private List<MenuItem> PopulateMenuItems()
        {
            List<MenuItem> items = new List<MenuItem>();

            MenuItem home = new MenuItem();
            home.Title = Resource.HomeText;
            home.TargetType = "Home";
            home.ImageSource = "Home96.png";

            MenuItem profile = new MenuItem();
            profile.Title = Resource.ProfileText;
            profile.TargetType = "Profile";
            profile.ImageSource = "User96.png";

            MenuItem vehicles = new MenuItem();
            vehicles.Title = Resource.VehiclesText;
            vehicles.TargetType = "Vehicles";
            vehicles.ImageSource = "InterstateTruck96.png";

            MenuItem history = new MenuItem();
            history.Title = Resource.HistoryText;
            history.TargetType = "History";
            history.ImageSource = "Clock96.png";

            MenuItem addShift = new MenuItem();
            addShift.Title = Resource.AddShiftText;
            addShift.TargetType = "AddShift";
            addShift.ImageSource = "AddList96.png";

            MenuItem signOut = new MenuItem();
            signOut.Title = Resource.SignOutText;
            signOut.TargetType = "SignOut";
            signOut.ImageSource = "Exit96.png";

            if (Settings.HamburgerSettings != true)
            {
                items.Add(home);
            }

            items.Add(profile);
            items.Add(vehicles);
            items.Add(history);
            items.Add(addShift);
            items.Add(signOut);

            return items;
        }
    }
}
