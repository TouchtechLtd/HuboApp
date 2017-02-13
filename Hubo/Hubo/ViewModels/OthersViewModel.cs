using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class OthersViewModel
    {
        public List<MenuItem> OthersPageList { get; set; }
        public string Name { get; set; }
        DatabaseService DbService = new DatabaseService();
        
        public OthersViewModel()
        {
            OthersPageList = PopulateMenuItems();
            Name = DbService.GetName();
        }

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
