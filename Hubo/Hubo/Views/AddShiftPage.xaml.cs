// <copyright file="AddShiftPage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using Xamarin.Forms;

    public partial class AddShiftPage : ContentPage
    {
        private readonly AddShiftViewModel addShiftVM = new AddShiftViewModel();

        public AddShiftPage()
        {
            InitializeComponent();
            BindingContext = addShiftVM;
            addShiftVM.Navigation = Navigation;
            ToolbarItem topLeftText = new ToolbarItem()
            {
                Text = "Add Shift"
            };
            ToolbarItems.Add(topLeftText);
            addButton.Clicked += AddButton_Clicked;
            Title = Resource.AddShiftText;
            addShiftVM.FullGrid = grid;

            startLocation.ReturnType = ReturnType.Next;
            startLocation.Next = endLocation;

            endLocation.ReturnType = ReturnType.Done;
            endLocation.Completed += EndLocation_Completed;
        }

        private void EndLocation_Completed(object sender, EventArgs e)
        {
            addShiftVM.SaveButton.Execute(null);
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            string[] buttons = new string[] { Resource.Break, Resource.NoteText, Resource.DriveText };

            var action = await DisplayActionSheet(Resource.AddBreakNote, Resource.Cancel, null, buttons);

            if (action != null && action != "Cancel")
            {
                addShiftVM.Add = action;
                await Navigation.PushModalAsync(new NavigationPage(new AddManBreakNotePage(action)));
            }
        }
    }
}
