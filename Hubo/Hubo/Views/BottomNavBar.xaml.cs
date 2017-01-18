using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hubo
{
    public partial class BottomNavBar : ContentPage
    {
        public BottomNavBar()
        {
            InitializeComponent();
            Title = "Test";

        }

        public BottomBarPage GetBottomBar()
        {
            BottomBarPage bottomBarPage = new BottomBarPage();
            bottomBarPage.BarTextColor = Color.Black;
            bottomBarPage.FixedMode = false;

            MenuViewModel menuVM = new MenuViewModel();

            foreach (MenuItem item in menuVM.MenuPageList)
            {
                switch (item.Title)
                {
                    case "Home":
                        HomePage home = new HomePage();

                        FileImageSource homeIcon = (FileImageSource)FileImageSource.FromFile(string.Format(item.ImageSource, item.Title.ToLowerInvariant()));

                        home.Title = item.Title;
                        home.Icon = homeIcon;

                        bottomBarPage.Children.Add(home);
                        break;
                    case "Vehicles":
                        VehiclesPage vehicle = new VehiclesPage(1);
                        vehicle.AddToolBar();
                        FileImageSource vehicleIcon = (FileImageSource)FileImageSource.FromFile(string.Format(item.ImageSource, item.Title.ToLowerInvariant()));

                        vehicle.Title = item.Title;
                        vehicle.Icon = vehicleIcon;

                        bottomBarPage.Children.Add(vehicle);
                        break;
                    case "History":
                        HistoryPage history = new HistoryPage();

                        FileImageSource historyIcon = (FileImageSource)FileImageSource.FromFile(string.Format(item.ImageSource, item.Title.ToLowerInvariant()));

                        history.Title = item.Title;
                        history.Icon = historyIcon;

                        bottomBarPage.Children.Add(history);
                        break;
                    case "Add Shift":
                        AddShiftPage addShift = new AddShiftPage();

                        FileImageSource addShiftIcon = (FileImageSource)FileImageSource.FromFile(string.Format(item.ImageSource, item.Title.ToLowerInvariant()));

                        addShift.Title = item.Title;
                        addShift.Icon = addShiftIcon;

                        bottomBarPage.Children.Add(addShift);
                        break;
                }
            }

            OthersPage other = new OthersPage();
            bottomBarPage.Children.Add(other);

            return bottomBarPage;
        }
    }
}
