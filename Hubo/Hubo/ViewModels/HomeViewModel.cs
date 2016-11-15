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
        public bool StartShiftVisibility { get; set; }
        public bool ShiftStarted { get; set; }
        public string StartBreakText { get; set; }
        public string EndShiftText { get; set; }
        public string VehicleText { get; set; }
        public string AddNoteText { get; set; }
        public ICommand StartBreakCommand { get; set; }
        public ICommand EndShiftCommand { get; set; }
        public ICommand VehicleCommand { get; set; }
        public ICommand AddNoteCommand { get; set; }

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
            StartShiftVisibility = true;
            StartBreakText = Resource.StartBreak;
            EndShiftText = Resource.EndShift;
            VehicleText = Resource.Vehicle;
            AddNoteText = Resource.AddNote;
        }

        private async void ToggleShift()
        {
            if (!ShiftStarted)
            {
                if (await StartShift())
                {
                    ShiftText = "End Shift";
                    ShiftButtonColor = Color.FromHex("#cc0000");
                    StartShiftVisibility = false;
                    ShiftStarted = true;
                    OnPropertyChanged("StartShiftVisibility");
                    OnPropertyChanged("ShiftStarted");
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
