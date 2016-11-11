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

        public HomePage()
        {
            InitializeComponent();
            BindingContext = homeVM;
            homeVM.Navigation = Navigation;
            greenRangePointer.Color = Color.FromHex("#009900");
            redRangePointer.Color = Color.FromHex("#cc0000");
            blueRangePointer.Color = Color.FromHex("#0000cc");
            //blueBarPointer.Color = Color.FromHex("#0000cc");
            //redBarPointer.Color = Color.FromHex("cc0000");
            //greenBarPointer.Color = Color.FromHex("#009900");
            circularScale.RimColor = Color.FromHex("#3E606F");
        }
    }
}
