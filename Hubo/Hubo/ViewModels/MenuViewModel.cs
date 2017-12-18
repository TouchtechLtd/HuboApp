// <copyright file="MenuViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Collections.Generic;

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

            items.Add(home);
            items.Add(vehicles);
            items.Add(history);
            items.Add(addShift);

            return items;
        }
    }
}
