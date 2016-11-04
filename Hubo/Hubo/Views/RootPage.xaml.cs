﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class RootPage : MasterDetailPage
    {
        public RootPage()
        {
            var menuPage = new MenuPage();
            menuPage.CopyList.ItemSelected += (sender, e) =>
            {
                if (((ListView)sender).SelectedItem == null)
                {
                    return;
                }
                ((ListView)sender).SelectedItem = null;
                NavigateTo(e.SelectedItem as MenuItem);
            };
            Master = menuPage;
            Detail = new NavigationPage(new HomePage())
            {
                BarTextColor = Color.White
            };
            this.PropertyChanged += RootPage_PropertyChanged;
        }

        private void RootPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IsGestureEnabled = true;
        }

        async void NavigateTo(MenuItem menu)
        {
            if (menu.TargetType == "Settings")
            {
                Detail.Navigation.PushAsync(new SettingsPage());
            }
            else if(menu.TargetType == "Profile")
            {
                Detail.Navigation.PushAsync(new ProfilePage());
            }
            else if (menu.TargetType == "Vehicles")
            {
                Detail.Navigation.PushAsync(new VehiclesPage());
            }
            else if (menu.TargetType == "History")
            {
                Detail.Navigation.PushAsync(new HistoryPage());
            }
            else if (menu.TargetType == "AddShift")
            {
                Detail.Navigation.PushAsync(new AddShiftPage());
            }
            else if (menu.TargetType == "SignOut")
            {
                Detail.Navigation.PushAsync(new SignOutPage());
            }
                IsPresented = false;
                IsGestureEnabled = false;
        }
    }
}