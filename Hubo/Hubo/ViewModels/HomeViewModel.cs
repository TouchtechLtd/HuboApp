using Syncfusion.SfGauge.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hubo
{
    class HomeViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand ShiftButton { get; set; }
        public string ShiftText { get; set; }
        public Color ShiftButtonColor { get; set; }
        public double CompletedJourney { get; set; }
        public int RemainderOfJourney { get; set; }
        public int Break { get; set; }
        public int TotalBeforeBreak { get; set; }
        public string TotalBeforeBreakText { get; set; }

        


        public HomeViewModel()
        {
            this.CompletedJourney = 5;
            this.RemainderOfJourney = 14;
            this.Break = 0;
            this.TotalBeforeBreak = 20;
            this.TotalBeforeBreakText = this.TotalBeforeBreak.ToString() + "/70 Hours Total";
            ShiftText = "Start Shift";
            ShiftButtonColor = Color.Lime;
            ShiftButton = new Command(ToggleShift);
        }

        private void ToggleShift()
        {
            if(ShiftText == "Start Shift")
            {
                ShiftText = "Stop Shift";
                ShiftButtonColor = Color.Red;
                
            }
            else
            {
                ShiftText = "Start Shift";
                ShiftButtonColor = Color.Lime;
            }
            OnPropertyChanged("ShiftText");
            OnPropertyChanged("ShiftButtonColor");
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
