
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
        private TimeSpan startTimeDrivePicker;
        private TimeSpan endTimePicker;
        private TimeSpan endTimeDrivePicker;
        private string startLocation;
        private string endLocation;
        private bool workShift;
        private bool driveShift;
        private bool breakShift;
        private int driveShiftCount;
        private int driveShiftTotalCount;
        private int breakCount;
        private int breakTotalCount;
        private string driveShiftTitle;
        private string registrationTitle;
        private int startHubo;
        private int endHubo;
        private List<DriveTable> listOfDriveShifts;
        private List<BreakTable> listOfBreaks;
        private DatabaseService dbService;

        public int StartHubo
        {
            get
            {
                return startHubo;
            }

            set
            {
                startHubo = value;
                OnPropertyChanged("StartHubo");
            }
        }

        public int EndHubo
        {
            get
            {
                return endHubo;
            }

            set
            {
                endHubo = value;
                OnPropertyChanged("EndHubo");
            }
        }

        public string RegistrationTitle
        {
            get
            {
                return registrationTitle;
            }

            set
            {
                registrationTitle = value;
                OnPropertyChanged("RegistrationTitle");
            }
        }

        public string DriveShiftTitle
        {
            get
            {
                return driveShiftTitle;
            }

            set
            {
                driveShiftTitle = value;
                OnPropertyChanged("DriveShiftTitle");
            }
        }

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

        public bool DriveShift
        {
            get
            {
                return driveShift;
            }

            set
            {
                driveShift = value;
                OnPropertyChanged("DriveShift");
            }
        }

        public bool BreakShift
        {
            get
            {
                return breakShift;
            }

            set
            {
                breakShift = value;
                OnPropertyChanged("BreakShift");
            }
        }

        public int DriveShiftTotalCount
        {
            get
            {
                return driveShiftTotalCount;
            }

            set
            {
                driveShiftTotalCount = value;
                OnPropertyChanged("DriveShiftTotalCount");
            }
        }

        public int BreakTotalCount
        {
            get
            {
                return breakTotalCount;
            }

            set
            {
                breakTotalCount = value;
                OnPropertyChanged("BreakTotalCount");
            }
        }

        public int DriveShiftCount
        {
            get
            {
                return driveShiftCount;
            }

            set
            {
                driveShiftCount = value;
                OnPropertyChanged("DriveShiftCount");
            }
        }

        public int BreakCount
        {
            get
            {
                return breakCount;
            }

            set
            {
                breakCount = value;
                OnPropertyChanged("BreakCount");
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

        public TimeSpan StartTimeDrivePicker
        {
            get
            {
                return startTimeDrivePicker;
            }

            set
            {
                startTimeDrivePicker = value;
                OnPropertyChanged("StartTimeDrivePicker");
            }
        }

        public TimeSpan EndTimeDrivePicker
        {
            get
            {
                return endTimeDrivePicker;
            }

            set
            {
                endTimeDrivePicker = value;
                OnPropertyChanged("EndTimeDrivePicker");
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
            WorkShift = true;
            DriveShift = false;
            BreakShift = false;
            dbService = new DatabaseService();

            //Get last workshift

            ShiftTable currentShift = dbService.GetLastShift();

            string uneditedStartLocation = currentShift.StartLocation;
            string uneditedEndLocation = currentShift.EndLocation;

            StartLocation = uneditedStartLocation.Substring(uneditedStartLocation.LastIndexOf(',') + 1);
            EndLocation = uneditedEndLocation.Substring(uneditedEndLocation.LastIndexOf(',') + 1);
            StartTimePicker = DateTime.Parse(currentShift.StartDate).TimeOfDay;
            EndTimePicker = DateTime.Parse(currentShift.EndDate).TimeOfDay;

            //Get all driving shifts associated
            listOfDriveShifts = dbService.GetDriveShifts(currentShift.Key);
            DriveShiftTotalCount = listOfDriveShifts.Count;

            //Get all breaks associated
            listOfBreaks = dbService.GetBreaks(currentShift);
            BreakTotalCount = listOfBreaks.Count;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void WorkShiftDone()
        {
            WorkShift = false;

            // Load details for driveShifts
            if (listOfDriveShifts.Count > 0)
            {
                //Load Details for driveShift
                DriveShiftCount = 1;
                DriveShift = true;
                LoadDriveDetails(listOfDriveShifts[0]);
            }
            else if (listOfBreaks.Count > 0)
            {
                BreakCount = 1;
                BreakShift = true;
            }

            // Else finish off work
        }

        private void LoadDriveDetails(DriveTable currentDrive)
        {
            DriveShiftTitle = "Drive Shift " + DriveShiftCount.ToString() + " of " + DriveShiftTotalCount.ToString();
            VehicleTable currentVehicle;
            if (currentDrive.ServerVehicleKey == 0)
            {
                currentVehicle = dbService.GetVehicleById(currentDrive.VehicleKey);
            }
            else
            {
                currentVehicle = dbService.GetVehicleByServerId(currentDrive.ServerVehicleKey);
            }

            RegistrationTitle = currentVehicle.Registration;
            StartTimeDrivePicker = DateTime.Parse(currentDrive.StartDate).TimeOfDay;
            EndTimeDrivePicker = DateTime.Parse(currentDrive.EndDate).TimeOfDay;
            StartHubo = currentDrive.StartHubo;
            EndHubo = currentDrive.EndHubo;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
