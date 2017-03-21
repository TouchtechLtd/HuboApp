// <copyright file="BottomNavBar.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using Xamarin.Forms;

    public partial class BottomNavBar : ContentPage
    {
        public BottomNavBar()
        {
            InitializeComponent();
            Title = "BottomBar";
        }

        public BottomBarPage GetBottomBar()
        {
            BottomBarPage bottomBarPage = new BottomBarPage()
            {
                FixedMode = true,
                BarTextColor = Color.White,
                BarTheme = BottomBarPage.BarThemeTypes.DarkWithoutAlpha
            };

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
                        VehiclesPage vehicle = new VehiclesPage();
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
