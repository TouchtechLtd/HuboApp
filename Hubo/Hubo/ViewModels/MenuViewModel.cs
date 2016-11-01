using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    class MenuViewModel
    {
        public List<MenuItem> MenuPageList { get; set; }
        public MenuViewModel()
        {
            MenuPageList = PopulateMenuItems();
        }

        private List<MenuItem> PopulateMenuItems()
        {
            List<MenuItem> items = new List<MenuItem>();

            MenuItem settings = new MenuItem();
            settings.Title = Resource.SettingsText;
            settings.TargetType = "Settings";

            MenuItem profile = new MenuItem();
            profile.Title = Resource.ProfileText;
            profile.TargetType = "Profile";

            MenuItem vehicles = new MenuItem();
            vehicles.Title = Resource.VehiclesText;
            vehicles.TargetType = "Vehicles";

            MenuItem history = new MenuItem();
            history.Title = Resource.HistoryText;
            history.TargetType = "History";

            MenuItem addShift = new MenuItem();
            addShift.Title = Resource.AddShiftText;
            addShift.TargetType = "AddShift";

            MenuItem signOut = new MenuItem();
            signOut.Title = Resource.SignOutText;
            signOut.TargetType = "SignOut";

            items.Add(settings);
            items.Add(profile);
            items.Add(vehicles);
            items.Add(history);
            items.Add(addShift);
            items.Add(signOut);

            return items;
        }
    }
}
