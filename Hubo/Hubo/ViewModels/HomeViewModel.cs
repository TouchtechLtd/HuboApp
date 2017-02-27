using Acr.UserDialogs;
using Microsoft.ProjectOxford.Vision.Contract;
using Plugin.Battery;
using Plugin.Media;
using Plugin.Media.Abstractions;
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
        public List<VehicleTable> listOfVehicles;
        public VehicleTable currentVehicle;

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand ShiftButton { get; set; }
        public string ShiftText { get; set; }
        public Xamarin.Forms.Color ShiftButtonColor { get; set; }
        public Xamarin.Forms.Color BreakButtonColor { get; set; }
        public Xamarin.Forms.Color VehicleTextColor { get; set; }
        public double CompletedJourney { get; set; }
        public int RemainderOfJourney { get; set; }
        public int Break { get; set; }
        public double TotalBeforeBreak { get; set; }
        public string TotalBeforeBreakText { get; set; }
        public bool StartShiftVisibility { get; set; }
        public bool ShiftStarted { get; set; }
        public string StartBreakText { get; set; }
        public string EndShiftText { get; set; }
        public string VehicleText { get; set; }
        public string AddNoteText { get; set; }
        public int VehicleRego { get; set; }
        public string HoursTillReset { get; set; }
        public bool VehicleInUse { get; set; }
        public string VehiclePickerText { get; set; }
        public bool PickerEnabled { get; set; }
        public string DriveTimes { get; set; }

        public FileImageSource ShiftImage { get; set; }

        public ICommand StartBreakCommand { get; set; }

        //public ICommand EndShiftCommand { get; set; }
        public ICommand VehicleCommand { get; set; }
        //public ICommand AddNoteCommand { get; set; }
        public bool OnBreak { get; set; }

        DatabaseService DbService = new DatabaseService();

        private bool _driveShiftRunning;
        public bool DriveShiftRunning
        {
            get
            {
                return _driveShiftRunning;
            }
            set
            {
                _driveShiftRunning = value;
                OnPropertyChanged("ShiftRunning");
            }
        }

        public HomeViewModel()
        {
            CompletedJourney = 0;
            RemainderOfJourney = 0;
            Break = 0;

            currentVehicle = new VehicleTable();

            int hoursTillReset = DbService.HoursTillReset();
            if (hoursTillReset == -1)
            {
                HoursTillReset = Resource.NoShiftsDoneYet;
                //TotalBeforeBreak = DbService.TotalBeforeBreak();
            }
            else if (hoursTillReset == -2)
            {
                HoursTillReset = Resource.FullyRested;
                //TotalBeforeBreak = 0;
            }
            else
            {
                HoursTillReset = hoursTillReset.ToString() + Resource.LastShiftEndText;
                //TotalBeforeBreak = DbService.TotalBeforeBreak();
            }

            CheckActiveShift();
            CheckActiveBreak();
            ShiftButton = new Command(ToggleShift);
            EndShiftText = Resource.EndShift;

            AddNoteText = Resource.AddNote;
            StartBreakCommand = new Command(ToggleBreak);
            VehicleCommand = new Command(ToggleDrive);
            SetVehicleLabel();
        }

        private void SetVehicleLabel()
        {
            if (DbService.VehicleActive())
            {
                currentVehicle = DbService.GetCurrentVehicle();
                GetVehicles();

                int count = 0;
                foreach (VehicleTable item in listOfVehicles)
                {
                    if (item.Registration == currentVehicle.Registration)
                    {
                        VehicleRego = count;
                        break;
                    }
                    count++;
                }

                DriveTimes = DbService.GetDriveTimes();
                VehiclePickerText = currentVehicle.Registration;
                VehicleText = Resource.StopDriving;
                VehicleTextColor = Xamarin.Forms.Color.FromHex("#F9D029");
                VehicleInUse = true;
                DriveShiftRunning = true;

                UpdateCircularGauge();
                GeoCollection();
            }
            else
            {
                VehiclePickerText = Resource.NoActiveVehicle;
                VehicleText = Resource.StartDriving;
                VehicleTextColor = Xamarin.Forms.Color.FromHex("#009900");
                VehicleInUse = false;
                DriveShiftRunning = false;
            }

            OnPropertyChanged("DriveTimes");
            OnPropertyChanged("DriveShiftRunning");
            OnPropertyChanged("VehicleRego");
            OnPropertyChanged("PickerEnabled");
            OnPropertyChanged("VehicleText");
            OnPropertyChanged("VehicleTextColor");
            OnPropertyChanged("VehicleInUse");
        }

        private void CheckActiveShift()
        {
            if (DbService.CheckActiveShift())
            {
                ShowEndShiftXAML();
            }
            else
            {
                ShowStartShiftXAML();
            }

        }

        private void CheckActiveBreak()
        {
            int onBreak = DbService.CheckOnBreak();
            if (onBreak == -1)
            {
                BreakButtonColor = Xamarin.Forms.Color.FromHex("#cc0000");
                StartBreakText = Resource.EndBreak;
                OnBreak = true;
            }
            else if (onBreak == 1)
            {
                BreakButtonColor = Xamarin.Forms.Color.FromHex("#009900");
                StartBreakText = Resource.StartBreak;
                OnBreak = false;
            }
        }

        public async void ToggleDrive()
        {
            if (!DriveShiftRunning)
            {
                if (await DbService.SaveDrive(DateTime.Now))
                {
                    SetVehicleLabel();
                    GeoCollection();
                    UpdateCircularGauge();
                }
            }
            else
            {
                if (await DbService.SaveDrive(DateTime.Now))
                    SetVehicleLabel();
            }
        }

        private async void ToggleBreak()
        {
            if (OnBreak)
            {
                if (await DbService.StopBreak())
                {
                    BreakButtonColor = Xamarin.Forms.Color.FromHex("#009900");
                    StartBreakText = Resource.StartBreak;
                    OnBreak = false;
                    DriveShiftRunning = true;
                    GeoCollection();
                    UpdateCircularGauge();
                }
            }
            else
            {
                var result = await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Are you sure you want to go on a break?", "Yes", "No");

                if (result)
                {
                    //Implement ability to take note and hubo
                    if (await DbService.StartBreak())
                    {
                        BreakButtonColor = Xamarin.Forms.Color.FromHex("#cc0000");
                        StartBreakText = Resource.EndBreak;
                        OnBreak = true;
                        DriveShiftRunning = false;
                    }
                }
            }
            OnPropertyChanged("BreakButtonColor");
            OnPropertyChanged("StartBreakText");
            OnPropertyChanged("OnBreak");
            OnPropertyChanged("DriveShiftRunning");
        }

        private async void ToggleShift()
        {
            if (!ShiftStarted)
            {
                if (CrossBattery.Current.Status == Plugin.Battery.Abstractions.BatteryStatus.Unknown)
                    await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to get battery status, Please use another device!", Resource.DisplayAlertOkay);

                if (CrossBattery.Current.RemainingChargePercent <= 50 && (CrossBattery.Current.Status == Plugin.Battery.Abstractions.BatteryStatus.Discharging || CrossBattery.Current.Status == Plugin.Battery.Abstractions.BatteryStatus.Unknown))
                    await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Battery at " + CrossBattery.Current.RemainingChargePercent + "%, Please ensure the device is charged soon!", Resource.DisplayAlertOkay);

                if (await StartShift())
                {
                    if (await DbService.StartShift())
                        ShowEndShiftXAML();
                }
            }
            else
            {
                if (!DbService.VehicleActive())
                {
                    if (DbService.CheckOnBreak() == 1)
                    {
                        if (await DbService.StopShift())
                        {
                            await Navigation.PushModalAsync(new NZTAMessagePage(2));
                            ShowStartShiftXAML();
                        }
                        else
                            await Application.Current.MainPage.DisplayAlert("ERROR", "There was a problem ending your shift", "Understood");
                    }
                    else
                        await Application.Current.MainPage.DisplayAlert("ERROR", "Pleas end your break before ending your work shift", "Gotcha");
                }
                else
                    await Application.Current.MainPage.DisplayAlert("ERROR", "Pleas end your driving shift before ending your work shift", "Gotcha");
            }
        }

        private void ShowEndShiftXAML()
        {
            ShiftText = "End Shift";
            ShiftButtonColor = Xamarin.Forms.Color.FromHex("#cc0000");
            StartShiftVisibility = false;
            ShiftStarted = true;
            ShiftImage = "Stop.png";

            OnPropertyChanged("ShiftText");
            OnPropertyChanged("ShiftButtonColor");
            OnPropertyChanged("StartShiftVisibility");
            OnPropertyChanged("ShiftStarted");
            OnPropertyChanged("ShiftImage");
        }

        private void ShowStartShiftXAML()
        {
            ShiftText = "Start Shift";
            ShiftButtonColor = Xamarin.Forms.Color.FromHex("#009900");
            StartShiftVisibility = true;
            ShiftStarted = false;
            ShiftImage = "Play.png";

            OnPropertyChanged("ShiftText");
            OnPropertyChanged("ShiftButtonColor");
            OnPropertyChanged("StartShiftVisibility");
            OnPropertyChanged("ShiftStarted");
            OnPropertyChanged("ShiftImage");
        }

        private async Task<bool> StartShift()
        {
            List<string> checklistQuestions = new List<string>();
            checklistQuestions = DbService.GetChecklist();
            int count = 0;
            foreach (string question in checklistQuestions)
            {
                count++;
                bool result = await Application.Current.MainPage.DisplayAlert(Resource.ChecklistQuestionNumber + count.ToString(), question, Resource.Yes, Resource.No);
                if (!result)
                {
                    return false;
                }
            }
            return true;
        }

        private void UpdateCircularGauge()
        {
            CompletedJourney = DbService.TotalSinceStart();

            if (CompletedJourney == -1)
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "More Than One Active Shift!", Resource.DisplayAlertOkay);
                return;
            }

            TotalBeforeBreak = DbService.NextBreak();
            //TotalBeforeBreakText = ((int)TotalBeforeBreak).ToString() + Resource.HoursTotalText;

            OnPropertyChanged("CompletedJourney");
            OnPropertyChanged("TotalBeforeBreak");
            OnPropertyChanged("TotalBeforeBreakText");

            Device.StartTimer(TimeSpan.FromMinutes(5), () =>
            {
                TotalBeforeBreak = DbService.TotalSinceStart();

                if (TotalBeforeBreak == -1)
                {
                    Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "More Than One Active Shift!", Resource.DisplayAlertOkay);
                }
                else
                {
                    CompletedJourney = TotalBeforeBreak;
                    TotalBeforeBreakText = ((int)TotalBeforeBreak).ToString() + Resource.HoursTotalText;

                    OnPropertyChanged("CompletedJourney");
                    OnPropertyChanged("TotalBeforeBreakText");
                }
                return DriveShiftRunning;
            });
        }

        public List<VehicleTable> GetVehicles()
        {
            listOfVehicles = new List<VehicleTable>();
            listOfVehicles = DbService.GetVehicles();
            return listOfVehicles;
        }

        private void GeoCollection()
        {
            var min = TimeSpan.FromMinutes(1);

            int driveKey = DbService.GetCurrentDriveShift().Key;

            DbService.CollectGeolocation(driveKey);

            Device.StartTimer(min, () =>
            {
                DbService.CollectGeolocation(driveKey);
                return DriveShiftRunning;
            });
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
