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

            Range rangeGreen = new Range();
            rangeGreen.BindingContext = homeVM;
            rangeGreen.StartValue = 0;
            rangeGreen.EndValue = 14;
            rangeGreen.Color = Color.FromHex("#009900");
            rangeGreen.Thickness = 30;
            Scale.Ranges.Add(rangeGreen);

            Scale.Interval = 14;
            Scale.RimThickness = 30;
            Scale.StartAngle = 135;
            Scale.SweepAngle = 270;
            Scale.StartValue = 0;
            Scale.EndValue = 14;
            Scale.LabelColor = Color.Gray;
            Scale.MinorTicksPerInterval = 13;
            Scale.LabelOffset = -0.45;
            Scale.LabelFontSize = 24;

            TickSettings major = new TickSettings();
            major.Length = 0;
            major.Thickness = 3;
            major.Color = Color.White;
            major.Offset = 0;
            Scale.MajorTickSettings = major;

            List<Pointer> pointers = new List<Pointer>();

            RangePointer blueRangepointer = new RangePointer();
            blueRangepointer.BindingContext = homeVM;
            blueRangepointer.Color = Color.FromHex("#0000cc");
            blueRangepointer.Thickness = 60;
            blueRangepointer.SetBinding(RangePointer.ValueProperty, new Binding("CompletedJourney"));
            blueRangepointer.EnableAnimation = true;
            pointers.Add(blueRangepointer);

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
