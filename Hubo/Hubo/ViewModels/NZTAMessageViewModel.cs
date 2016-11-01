using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class NZTAMessageViewModel
    {
        public INavigation Navigation { get; set; }
        public string NZTADisclaimer { get; set; }
        public string NZTAButtonText { get; set; }
        public ICommand NZTAButton { get; set; }

        public NZTAMessageViewModel()
        {
            NZTAButtonText = Resource.NZTAButtonText;
            NZTADisclaimer = Resource.NZTADisclaimer;
            NZTAButton = new Command(ProceedToHomePage);
        }

        private void ProceedToHomePage()
        {
            App.Current.MainPage = new RootPage();
        }
    }
}
