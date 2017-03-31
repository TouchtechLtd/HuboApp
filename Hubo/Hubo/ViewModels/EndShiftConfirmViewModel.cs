
namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class EndShiftConfirmViewModel : INotifyPropertyChanged
    {
        private TimeSpan startTimePicker;
        private TimeSpan endTimePicker;
        private string startLocation;
        private string endLocation;
        private bool workShift;

        public bool WorkShift
        {
            get
            {
                return workShift;
            }

            set
            {
                workShift = value;
                OnPropertyChanged("WorkShift");
            }
        }

        public TimeSpan StartTimePicker
        {
            get
            {
                return startTimePicker;
            }

            set
            {
                startTimePicker = value;
                OnPropertyChanged("StartTimePicker");
            }
        }

        public TimeSpan EndTimePicker
        {
            get
            {
                return endTimePicker;
            }

            set
            {
                endTimePicker = value;
                OnPropertyChanged("EndTimePicker");
            }
        }

        public string StartLocation
        {
            get
            {
                return startLocation;
            }

            set
            {
                startLocation = value;
                OnPropertyChanged("StartLocation");
            }
        }

        public string EndLocation
        {
            get
            {
                return endLocation;
            }

            set
            {
                endLocation = value;
                OnPropertyChanged("EndLocation");
            }
        }

        public EndShiftConfirmViewModel()
        {
            StartTimePicker = DateTime.Now.TimeOfDay;
            EndTimePicker = DateTime.Now.TimeOfDay;
            StartLocation = "Wellington";
            EndLocation = "Auckland";
            WorkShift = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
