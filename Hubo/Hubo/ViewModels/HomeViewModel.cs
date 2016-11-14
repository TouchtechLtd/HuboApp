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
        public int StartValue { get; set; }

        DatabaseService DbService = new DatabaseService();


        public HomeViewModel()
        {
            this.CompletedJourney = 0;
            this.RemainderOfJourney = 0;
            this.Break = 0;
            this.TotalBeforeBreak = 20;
            this.TotalBeforeBreakText = this.TotalBeforeBreak.ToString() + "/70 Hours Total";
            ShiftText = "Start Shift";
            ShiftButtonColor = Color.FromHex("#009900");
            ShiftButton = new Command(ToggleShift);
            this.StartValue = 5;
        }

        private async void ToggleShift()
        {
            if (ShiftText == "Start Shift")
            {
                if (await StartShift())
                {
                    ShiftText = "Stop Shift";
                    ShiftButtonColor = Color.FromHex("#cc0000"); 
                }

            }
            else
            {
                { 
                    ShiftText = "Start Shift";
                    ShiftButtonColor = Color.FromHex("#009900");
                    UpdateCircularGauge();
                }

            }
            OnPropertyChanged("ShiftText");
            OnPropertyChanged("ShiftButtonColor");
        }

        private async Task<bool> StartShift()
        {
            List<string> checklistQuestions = new List<string>();
            checklistQuestions = DbService.GetChecklist();
            int count = 0;
            foreach(string question in checklistQuestions)
            {
                count++;
                bool result = await Application.Current.MainPage.DisplayAlert(Resource.ChecklistQuestionNumber + count.ToString(), question, Resource.Yes,Resource.No);
                if (!result)
                {
                    return false;
                }
            }
            return true;
        }

        private void UpdateCircularGauge()
        {
            DateTime test = new DateTime();
            test = DateTime.Now;
            int hour = test.Hour;
            this.CompletedJourney = hour;
            OnPropertyChanged("CompletedJourney");
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
