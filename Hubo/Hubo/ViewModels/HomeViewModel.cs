// <copyright file="HomeViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Plugin.Battery;
    using Xamarin.Forms;
    using XLabs;

    internal class HomeViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService dbService = new DatabaseService();

        private readonly Countdown countdown = new Countdown();

        private List<VehicleTable> listOfVehicles;

        private VehicleTable currentVehicle;

        private bool driveShiftRunning;

        public HomeViewModel()
        {
            CompletedJourney = 0;
            RemainderOfJourney = 0;
            Break = 0;

            currentVehicle = new VehicleTable();

            int hoursTillReset = dbService.HoursTillReset();
            if (hoursTillReset == -1)
            {
                HoursTillReset = Resource.NoShiftsDoneYet;
            }
            else if (hoursTillReset == -2)
            {
                HoursTillReset = Resource.FullyRested;
            }
            else
            {
                HoursTillReset = hoursTillReset.ToString() + Resource.LastShiftEndText;
            }

            CheckActiveShift();
            CheckActiveBreak();

            if (!OnBreak && dbService.CheckActiveShift())
            {
                UpdateCircularGauge();
            }

            ShiftButton = new RelayCommand(async () => await ToggleShift());
            EndShiftText = Resource.EndShift;
            AddNoteText = Resource.AddNote;
            StartBreakCommand = new RelayCommand(async () => await ToggleBreak());
            VehicleCommand = new RelayCommand(async () => await ToggleDrive());
            SetVehicleLabel();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public INavigation Navigation { get; set; }

        public ICommand ShiftButton { get; set; }

        public string ShiftText { get; set; }

        public Color ShiftButtonColor { get; set; }

        public Color BreakButtonColor { get; set; }

        public Color VehicleTextColor { get; set; }

        public double CompletedJourney { get; set; }

        public int RemainderOfJourney { get; set; }

        public int Break { get; set; }

        public double TotalBeforeBreak { get; set; }

        public bool StartShiftVisibility { get; set; }

        public bool ShiftStarted { get; set; }

        public bool ShiftRunning { get; set; }

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

        public ICommand VehicleCommand { get; set; }

        public bool OnBreak { get; set; }

        public bool DriveShiftRunning
        {
            get
            {
                return driveShiftRunning;
            }

            set
            {
                driveShiftRunning = value;
                OnPropertyChanged("ShiftRunning");
            }
        }

        public List<VehicleTable> GetVehicles()
        {
            listOfVehicles = new List<VehicleTable>();
            listOfVehicles = dbService.GetVehicles();
            return listOfVehicles;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void SetVehicleLabel()
        {
            if (dbService.VehicleActive())
            {
                currentVehicle = dbService.GetCurrentVehicle();
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

                DriveTimes = dbService.GetDriveTimes();
                VehiclePickerText = currentVehicle.Registration;
                VehicleText = Resource.StopDriving;
                VehicleTextColor = Color.FromHex("#F9D029");
                VehicleInUse = true;
                DriveShiftRunning = true;

                GeoCollection();
            }
            else
            {
                VehiclePickerText = Resource.NoActiveVehicle;
                VehicleText = Resource.StartDriving;
                VehicleTextColor = Color.FromHex("#009900");
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
            if (dbService.CheckActiveShift())
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
            int onBreak = dbService.CheckOnBreak();
            if (onBreak == -1)
            {
                BreakButtonColor = Color.FromHex("#cc0000");
                StartBreakText = Resource.EndBreak;
                OnBreak = true;
                ShiftRunning = false;

                DateTime startTime = dbService.GetBreakStart();

                countdown.Restart(30 * 60, startTime);
            }
            else if (onBreak == 1)
            {
                BreakButtonColor = Color.FromHex("#009900");
                StartBreakText = Resource.StartBreak;
                OnBreak = false;
                ShiftRunning = true;
            }
        }

        private async Task ToggleDrive()
        {
            if (!DriveShiftRunning)
            {
                if (await dbService.SaveDrive(DateTime.Now))
                {
                    SetVehicleLabel();
                    GeoCollection();
                }
            }
            else
            {
                if (await dbService.SaveDrive(DateTime.Now))
                {
                    SetVehicleLabel();
                }
            }
        }

        private async Task ToggleBreak()
        {
            if (OnBreak)
            {
                if (await dbService.StopBreak())
                {
                    BreakButtonColor = Xamarin.Forms.Color.FromHex("#009900");
                    StartBreakText = Resource.StartBreak;
                    OnBreak = false;
                    DriveShiftRunning = true;
                    ShiftRunning = true;

                    if (dbService.VehicleActive())
                    {
                        GeoCollection();
                    }

                    UpdateCircularGauge();

                    countdown.Stop();
                }
            }
            else
            {
                var result = await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Are you sure you want to go on a break?", "Yes", "No");

                if (result)
                {
                    // Implement ability to take note and hubo
                    if (await dbService.StartBreak())
                    {
                        BreakButtonColor = Xamarin.Forms.Color.FromHex("#cc0000");
                        StartBreakText = Resource.EndBreak;
                        OnBreak = true;
                        DriveShiftRunning = false;
                        ShiftRunning = false;

                        countdown.Start(30 * 60);
                    }
                }
            }

            OnPropertyChanged("ShiftStarted");
            OnPropertyChanged("BreakButtonColor");
            OnPropertyChanged("StartBreakText");
            OnPropertyChanged("OnBreak");
            OnPropertyChanged("DriveShiftRunning");
        }

        private async Task ToggleShift()
        {
            if (!ShiftStarted)
            {
                if (CrossBattery.Current.Status == Plugin.Battery.Abstractions.BatteryStatus.Unknown)
                {
                    await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to get battery status, Please use another device!", Resource.DisplayAlertOkay);
                }

                if (CrossBattery.Current.RemainingChargePercent <= 50 && (CrossBattery.Current.Status == Plugin.Battery.Abstractions.BatteryStatus.Discharging || CrossBattery.Current.Status == Plugin.Battery.Abstractions.BatteryStatus.Unknown))
                {
                    await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Battery at " + CrossBattery.Current.RemainingChargePercent + "%, Please ensure the device is charged soon!", Resource.DisplayAlertOkay);
                }

                if (await StartShift().ConfigureAwait(false))
                {
                    if (await dbService.StartShift())
                    {
                        ShowEndShiftXAML();
                        UpdateCircularGauge();
                    }
                }
            }
            else
            {
                if (!dbService.VehicleActive())
                {
                    if (dbService.CheckOnBreak() == 1)
                    {
                        if (await dbService.StopShift())
                        {
                            await Navigation.PushModalAsync(new NZTAMessagePage(2));
                            ShowStartShiftXAML();
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("ERROR", "There was a problem ending your shift", "Understood");
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("ERROR", "Pleas end your break before ending your work shift", "Gotcha");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("ERROR", "Pleas end your driving shift before ending your work shift", "Gotcha");
                }
            }
        }

        private void ShowEndShiftXAML()
        {
            ShiftText = "End Shift";
            ShiftButtonColor = Xamarin.Forms.Color.FromHex("#cc0000");
            StartShiftVisibility = false;
            ShiftStarted = true;
            ShiftRunning = true;
            ShiftImage = "Stop.png";

            OnPropertyChanged("ShiftText");
            OnPropertyChanged("ShiftRunning");
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
            ShiftRunning = false;
            ShiftImage = "Play.png";

            OnPropertyChanged("ShiftText");
            OnPropertyChanged("ShiftRunning");
            OnPropertyChanged("ShiftButtonColor");
            OnPropertyChanged("StartShiftVisibility");
            OnPropertyChanged("ShiftStarted");
            OnPropertyChanged("ShiftImage");
        }

        private async Task<bool> StartShift()
        {
            List<string> checklistQuestions = dbService.GetChecklist();
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
            CompletedJourney = dbService.TotalSinceStart();

            if (CompletedJourney == -1)
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "More Than One Active Shift!", Resource.DisplayAlertOkay);
                return;
            }

            if (!OnBreak)
            {
                TotalBeforeBreak = dbService.NextBreak();
            }
            else
            {
                TotalBeforeBreak = CompletedJourney;
            }

            OnPropertyChanged("CompletedJourney");
            OnPropertyChanged("TotalBeforeBreak");

            Device.StartTimer(TimeSpan.FromMinutes(5), () =>
            {
                CompletedJourney = dbService.TotalSinceStart();

                OnPropertyChanged("CompletedJourney");

                return ShiftRunning;
            });
        }

        private void GeoCollection()
        {
            var min = TimeSpan.FromMinutes(1);

            int driveKey = dbService.GetCurrentDriveShift().Key;

            dbService.CollectGeolocation(driveKey);

            Device.StartTimer(min, () =>
            {
                dbService.CollectGeolocation(driveKey);
                return DriveShiftRunning;
            });
        }
    }
}
