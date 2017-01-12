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

        public HomePage()
        {
            InitializeComponent();
            BindingContext = homeVM;
            homeVM.Navigation = Navigation;
            BackgroundColor = Color.FromHex("#FCFFF5");
            Title = Resource.Hubo;

            Range rangeBlue = new Range();
            rangeBlue.StartValue = 0;
            rangeBlue.EndValue = homeVM.CompletedJourney;
            rangeBlue.Color = Color.FromHex("#0000cc");
            rangeBlue.Thickness = 10;
            Scale.Ranges.Add(rangeBlue);

            Range rangeGreen = new Range();
            rangeGreen.StartValue = homeVM.CompletedJourney;
            rangeGreen.EndValue = 13;
            rangeGreen.Color = Color.FromHex("#009900");
            rangeGreen.Thickness = 10;
            Scale.Ranges.Add(rangeGreen);

            Range rangeRed = new Range();
            rangeRed.StartValue = 13;
            rangeRed.EndValue = 24;
            rangeRed.Color = Color.FromHex("#cc0000");
            rangeRed.Thickness = 10;
            Scale.Ranges.Add(rangeRed);

            Scale.Interval = 2;
            Scale.RimThickness = 10;
            Scale.StartAngle = 135;
            Scale.SweepAngle = 270;
            Scale.StartValue = 0;
            Scale.EndValue = 24;
            Scale.LabelColor = Color.Gray;
            Scale.MinorTicksPerInterval = 1;
            Scale.LabelOffset = 0.2;

            TickSettings major = new TickSettings();
            major.Length = 12;
            major.Thickness = 3;
            major.Color = Color.Black;
            major.Offset = 0.35;
            Scale.MajorTickSettings = major;

            TickSettings minor = new TickSettings();
            minor.Length = 6;
            minor.Thickness = 3;
            minor.Color = Color.Green;
            minor.Offset = 0.35;
            Scale.MinorTickSettings = minor;

            List<Pointer> pointers = new List<Pointer>();

            NeedlePointer needlePointer = new NeedlePointer();
            needlePointer.Value = homeVM.CompletedJourney;
            needlePointer.Color = Color.Black;
            needlePointer.KnobColor = Color.Gray;
            needlePointer.KnobRadius = 15;
            needlePointer.LengthFactor = 0.5;
            needlePointer.EnableAnimation = true;
            pointers.Add(needlePointer);

            RangePointer greenRangepointer = new RangePointer();
            greenRangepointer.Color = Color.FromHex("#009900");
            greenRangepointer.Thickness = 10;
            greenRangepointer.Value = homeVM.RemainderOfJourney;
            greenRangepointer.EnableAnimation = true;
            pointers.Add(greenRangepointer);

            RangePointer blueRangepointer = new RangePointer();
            blueRangepointer.Color = Color.FromHex("#0000cc");
            blueRangepointer.Thickness = 10;
            blueRangepointer.Value = homeVM.CompletedJourney;
            blueRangepointer.EnableAnimation = true;
            pointers.Add(blueRangepointer);

            RangePointer redRangepointer = new RangePointer();
            redRangepointer.Color = Color.FromHex("#cc0000");
            redRangepointer.Thickness = 10;
            redRangepointer.Value = homeVM.RemainderOfJourney;
            redRangepointer.EnableAnimation = true;
            pointers.Add(redRangepointer);

            Scale.Pointers = pointers;

            Scales.Add(Scale);
            
            circleGauge.Scales = Scales;
        }
    }
}
