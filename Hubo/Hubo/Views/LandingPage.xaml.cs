using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Hubo
{
    public partial class LandingPage : ContentPage
    {
        LandingPageViewModel landingPageVM = new LandingPageViewModel();

        public LandingPage()
        {
            InitializeComponent();
            landingPageVM.Navigation = Navigation;
            BindingContext = landingPageVM;
        }
    }
}
