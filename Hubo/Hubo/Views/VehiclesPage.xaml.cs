using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class VehiclesPage : ContentPage
    {
        VehiclesViewModel vehiclesVM = new VehiclesViewModel();

        public VehiclesPage()
        {
            InitializeComponent();
            BindingContext = vehiclesVM;
            switchToggle.Toggled += SwitchToggle_Toggled;
        }

        private void SwitchToggle_Toggled(object sender, ToggledEventArgs e)
        {
            vehiclesVM.ToggleSwitch(e.Value);
        }
    }
}
