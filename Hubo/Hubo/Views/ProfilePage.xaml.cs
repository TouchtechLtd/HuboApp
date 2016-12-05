﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class ProfilePage : TabbedPage
    {

        ProfileViewModel profileVM = new ProfileViewModel();

        public ProfilePage()
        {
            InitializeComponent();
            profileVM.Navigation = Navigation;
            BindingContext = profileVM;
            ToolbarItem Done = new ToolbarItem();
            ToolbarItem Cancel = new ToolbarItem();
            Done.Text = Resource.Save;
            Cancel.Text = Resource.Cancel;
            Done.Command = profileVM.SaveAndExit;
            Cancel.Command = profileVM.CancelAndExit;
            ToolbarItems.Add(Done);
            ToolbarItems.Add(Cancel);
            Title = Resource.ProfileText;
        }

    }
}
