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
        }
    }
}
