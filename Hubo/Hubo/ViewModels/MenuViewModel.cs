// <copyright file="MenuViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
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

            MenuItem home = new MenuItem()
            {
                Title = Resource.HomeText,
                TargetType = "Home",
                ImageSource = "Home96.png"
            };

            MenuItem profile = new MenuItem()
            {
                Title = Resource.ProfileText,
                TargetType = "Profile",
                ImageSource = "User96.png"
            };

            MenuItem vehicles = new MenuItem()
            {
                Title = Resource.VehiclesText,
                TargetType = "Vehicles",
                ImageSource = "InterstateTruck96.png"
            };

            MenuItem history = new MenuItem()
            {
                Title = Resource.HistoryText,
                TargetType = "History",
                ImageSource = "Clock96.png"
            };

            MenuItem addShift = new MenuItem()
            {
                Title = Resource.AddShiftText,
                TargetType = "AddShift",
                ImageSource = "AddList96.png"
            };

            MenuItem signOut = new MenuItem()
            {
                Title = Resource.SignOutText,
                TargetType = "SignOut",
                ImageSource = "Exit96.png"
            };

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
