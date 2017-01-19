using Syncfusion.SfChart.XForms;
using Syncfusion.SfGauge.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hubo
{
    public partial class HomePage : ContentPage
    {
        HomeViewModel homeVM = new HomeViewModel();

        new Scale Scale = new Scale();
        ObservableCollection<Scale> Scales = new ObservableCollection<Scale>();
        List<VehicleTable> vehicles = new List<VehicleTable>();

        public HomePage()
        {
            InitializeComponent();
            BindingContext = homeVM;
            homeVM.Navigation = Navigation;
            BackgroundColor = Color.FromHex("#FCFFF5");
            Title = Resource.Hubo;
            UpdateList();
            vehiclePicker.SelectedIndexChanged += VehiclePicker_SelectedIndexChanged;

            ToolbarItem topLeftText = new ToolbarItem();
            topLeftText.Text = "Home";
            ToolbarItems.Add(topLeftText);

            Range rangeBlue = new Range();
            rangeBlue.BindingContext = homeVM;
            rangeBlue.StartValue = 0;
            rangeBlue.SetBinding(Range.EndValueProperty, new Binding("CompletedJourney"));
            rangeBlue.Color = Color.FromHex("#0000cc");
            rangeBlue.Thickness = 30;
            Scale.Ranges.Add(rangeBlue);

            Range rangeGreen = new Range();
            rangeGreen.BindingContext = homeVM;
            rangeGreen.SetBinding(Range.StartValueProperty, new Binding("CompletedJourney"));
            rangeGreen.EndValue = 8;
            rangeGreen.Color = Color.FromHex("#009900");
            rangeGreen.Thickness = 30;
            Scale.Ranges.Add(rangeGreen);

            Range rangeRed = new Range();
            rangeRed.StartValue = 8;
            rangeRed.EndValue = 14;
            rangeRed.Color = Color.FromHex("#cc0000");
            rangeRed.Thickness = 30;
            Scale.Ranges.Add(rangeRed);

            Scale.Interval = 14;
            Scale.RimThickness = 30;
            Scale.StartAngle = 135;
            Scale.SweepAngle = 270;
            Scale.StartValue = 0;
            Scale.EndValue = 14;
            Scale.LabelColor = Color.Gray;
            Scale.MinorTicksPerInterval = 0;
            Scale.LabelOffset = -0.3;

            TickSettings major = new TickSettings();
            major.Length = 0;
            major.Thickness = 3;
            major.Color = Color.White;
            major.Offset = 0;
            Scale.MajorTickSettings = major;

            //TickSettings minor = new TickSettings();
            //minor.BindingContext = homeVM;
            //minor.Length = 6;
            //minor.Thickness = 3;
            //minor.SetBinding(TickSettings.ColorProperty, new Binding("MinorTickColor"));
            //minor.Offset = 0.35;
            //Scale.MinorTickSettings = minor;

            List<Pointer> pointers = new List<Pointer>();

            //NeedlePointer needlePointer = new NeedlePointer();
            ////needlePointer.BindingContext = homeVM;
            //needlePointer.SetBinding(NeedlePointer.ValueProperty, new Binding("CompletedJourney"));
            //needlePointer.Color = Color.Black;
            //needlePointer.KnobColor = Color.Gray;
            //needlePointer.KnobRadius = 15;
            //needlePointer.LengthFactor = 0.5;
            //needlePointer.EnableAnimation = true;
            //pointers.Add(needlePointer);

            RangePointer greenRangepointer = new RangePointer();
            greenRangepointer.BindingContext = homeVM;
            greenRangepointer.Color = Color.FromHex("#009900");
            greenRangepointer.Thickness = 30;
            greenRangepointer.SetBinding(RangePointer.ValueProperty, new Binding("RemainderOfJourney"));
            greenRangepointer.EnableAnimation = true;
            pointers.Add(greenRangepointer);

            RangePointer blueRangepointer = new RangePointer();
            blueRangepointer.BindingContext = homeVM;
            blueRangepointer.Color = Color.FromHex("#0000cc");
            blueRangepointer.Thickness = 30;
            blueRangepointer.SetBinding(RangePointer.ValueProperty, new Binding("CompletedJourney"));
            blueRangepointer.EnableAnimation = true;
            pointers.Add(blueRangepointer);

            RangePointer redRangepointer = new RangePointer();
            redRangepointer.BindingContext = homeVM;
            redRangepointer.Color = Color.FromHex("#cc0000");
            redRangepointer.Thickness = 30;
            redRangepointer.SetBinding(RangePointer.ValueProperty, new Binding("RemainderOfJourney"));
            redRangepointer.EnableAnimation = true;
            pointers.Add(redRangepointer);

            Scale.Pointers = pointers;

            Scales.Add(Scale);

            circleGauge.Scales = Scales;
        }

        private void VehiclePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (vehiclePicker.SelectedIndex != -1)
            {
                if (vehiclePicker.SelectedIndex == vehiclePicker.Items.Count - 1)
                {
                    Navigation.PushAsync(new AddVehiclePage());
                    vehiclePicker.SelectedIndex = -1;
                }
                else
                {
                    homeVM.currentVehicle = vehicles[vehiclePicker.SelectedIndex];
                }
            }
        }

        public void UpdateList()
        {
            vehicles = homeVM.GetVehicles();
            vehiclePicker.Items.Clear();
            foreach (VehicleTable vehicle in vehicles)
            {
                vehiclePicker.Items.Add(vehicle.Registration);
            }
            vehiclePicker.Items.Add("Add Vehicle...");
        }
    }
}
