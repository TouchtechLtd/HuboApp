using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class NZTAMessageViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public string NZTADisclaimer { get; set; }
        public string NZTAButtonText { get; set; }
        public ICommand NZTAButton { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public NZTAMessageViewModel()
        {
            
        }

        internal void Load(int instruction)
        {
            if(instruction==1)
            {
                NZTAButtonText = Resource.NZTAButtonText;
                NZTADisclaimer = Resource.NZTADisclaimer;
                NZTAButton = new Command(ProceedToHomePage);
            }
            else if(instruction==2)
            {
                NZTADisclaimer = Resource.EndShiftDisclaimer;
                NZTAButtonText = Resource.Continue;
                NZTAButton = new Command(PopPage);
            }
            OnPropertyChanged("NZTAButtonText");
            OnPropertyChanged("NZTADisclaimer");
            OnPropertyChanged("NZTAButton");
        }

        private void PopPage()
        {
            Navigation.PopModalAsync();
        }

        private void ProceedToHomePage()
        {
            Application.Current.MainPage = new RootPage();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
