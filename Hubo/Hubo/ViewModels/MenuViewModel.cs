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
        public string Name { get; set; }
        DatabaseService DbService = new DatabaseService();
        public MenuViewModel()
        {
            MenuPageList = PopulateMenuItems();
            Name = DbService.GetName();
        }

        private List<MenuItem> PopulateMenuItems()
        {
            List<MenuItem> items = new List<MenuItem>();

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

            //items.Add(settings);
            items.Add(profile);
            items.Add(vehicles);
            items.Add(history);
            items.Add(addShift);
            items.Add(signOut);

            return items;
        }
    }
}
