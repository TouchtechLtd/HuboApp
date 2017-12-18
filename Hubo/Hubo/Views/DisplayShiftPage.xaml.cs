// <copyright file="DisplayShiftPage.xaml.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using Xamarin.Forms;
    using XFShapeView;

    public partial class DisplayShiftPage : ContentPage
    {
        private DisplayShiftViewModel displayVM;
        private List<ShiftTable> shifts = new List<ShiftTable>();

        public DisplayShiftPage(DateTime selectedDate)
        {
            InitializeComponent();
            displayVM = new DisplayShiftViewModel(selectedDate)
            {
                Navigation = Navigation
            };
            BindingContext = displayVM;
            shiftPicker.Title = Resource.ShiftPickerTitle;

            shifts = displayVM.ShiftList;

            foreach (ShiftTable shift in shifts)
            {
                // Format and add shifts to picker
                if (shift.EndDate == null)
                {
                    shift.EndDate = "Current";
                }

                DateTime shiftStart = DateTime.Parse(shift.StartDate);
                DateTime shiftEnd = DateTime.Parse(shift.EndDate);

                shiftPicker.Items.Add(string.Format("{0:dd/MM}", shiftStart) + ") " + string.Format("{0:hh:mm tt}", shiftStart) + " - " + string.Format("{0:hh:mm tt}", shiftEnd));
            }

            var leftArrow = new ShapeView
            {
                ShapeType = ShapeType.Triangle,
                Color = Color.Gray,
                HeightRequest = 30,
                WidthRequest = 30,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Rotation = -90
            };
            leftArrow.SetBinding(ShapeView.IsVisibleProperty, "CanExecuteLeft");

            var leftTapGesture = new TapGestureRecognizer()
            {
                CommandParameter = "Left"
            };
            leftTapGesture.SetBinding(TapGestureRecognizer.CommandProperty, "ChangeShiftLeftCommand");
            leftArrow.GestureRecognizers.Add(leftTapGesture);
            leftBox.GestureRecognizers.Add(leftTapGesture);

            var rightArrow = new ShapeView
            {
                ShapeType = ShapeType.Triangle,
                Color = Color.Gray,
                HeightRequest = 30,
                WidthRequest = 30,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Rotation = 90
            };
            rightArrow.SetBinding(ShapeView.IsVisibleProperty, "CanExecuteRight");

            var rightTapGesture = new TapGestureRecognizer()
            {
                CommandParameter = "Right"
            };
            rightTapGesture.SetBinding(TapGestureRecognizer.CommandProperty, "ChangeShiftRightCommand");
            rightArrow.GestureRecognizers.Add(rightTapGesture);
            rightBox.GestureRecognizers.Add(rightTapGesture);

            grid.Children.Add(leftArrow, 0, 0);
            grid.Children.Add(rightArrow, 2, 0);

            shiftPicker.SelectedIndexChanged += ShiftPicker_SelectedIndexChanged;

            MessagingCenter.Subscribe<string, int>("ChangeShift", "ChangeShift", (s, index) =>
            {
                shiftPicker.SelectedIndex = index;
            });
        }

        private void ShiftPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (shifts.Count > 0)
            {
                displayVM.LoadShiftDetails(shifts[shiftPicker.SelectedIndex]);
            }
        }

        private void DisableItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (((ListView)sender).SelectedItem == null)
            {
                return;
            }

            ((ListView)sender).SelectedItem = null;
        }
    }
}
