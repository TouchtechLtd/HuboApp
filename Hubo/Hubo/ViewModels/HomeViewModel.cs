// <copyright file="HomeViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Acr.UserDialogs;
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

        private bool isRunning;
        private double totalTime;
        private double remainTime;
        private int notificationId;
        private bool notifyReady;

        private CancellationTokenSource cancel;

        public HomeViewModel()
        {
            MessagingCenter.Subscribe<string, MessagingModel>("Countdown Update", "CountDown Update", (s, sentValues) =>
            {
                switch (sentValues.PropertyName)
                {
                    case "IsRunning":
                        IsRunning = sentValues.PropertyBool;
                        break;
                    case "TotalTime":
                        TotalTime = sentValues.PropertyValue;
                        break;
                    case "RemainTime":
                        RemainTime = sentValues.PropertyValue;
                        break;
                    default:
                        break;
                }
            });

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

            if (dbService.CheckActiveShift())
            {
                UpdateCircularGauge();

                if (dbService.CheckOnBreak() == -1)
                {
                    DependencyService.Get<INotifyService>().PresentNotification("On Break", "You are currently on your break");
                }
                else
                {
                    DependencyService.Get<INotifyService>().PresentNotification("Working", "You are currently working");
                }
            }
            else
            {
                DependencyService.Get<INotifyService>().PresentNotification("Ready", "This app is ready to record your shift");
            }

            ShiftButton = new RelayCommand(async () => await ToggleShift());
            EndShiftText = Resource.EndShift;
            AddNoteText = Resource.AddNote;
            StartBreakCommand = new RelayCommand(async () => await ToggleBreak());
            VehicleCommand = new RelayCommand(async () => await ToggleDrive());
            SetVehicleLabel();
            cancel = new CancellationTokenSource();
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

        public bool IsRunning
        {
            get
            {
                return isRunning;
            }

            set
            {
                isRunning = value;
                OnPropertyChanged("IsRunning");
            }
        }

        public double TotalTime
        {
            get
            {
                return totalTime;
            }

            set
            {
                totalTime = value;
                OnPropertyChanged("TotalTime");
            }
        }

        public double RemainTime
        {
            get
            {
                return remainTime;
            }

            set
            {
                remainTime = value;
                OnPropertyChanged("RemainTime");
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
                notificationId = 5;
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
            await Application.locator.StartListeningAsync(2000, 0, true);

            if (OnBreak)
            {
                var result = await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Are you sure you want to end your break?", "Yes", "No");

                if (result)
                {
                    if (await dbService.StopBreak())
                    {
                        BreakButtonColor = Color.FromHex("#009900");
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
            }
            else
            {
                var result = await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Are you sure you want to go on a break?", "Yes", "No");

                if (result)
                {
                    // Implement ability to take note and hubo
                    if (await dbService.StartBreak())
                    {
                        BreakButtonColor = Color.FromHex("#cc0000");
                        StartBreakText = Resource.EndBreak;
                        OnBreak = true;
                        DriveShiftRunning = false;
                        ShiftRunning = false;

                        countdown.Start(30 * 60);
                    }
                }
            }

            await Application.locator.StopListeningAsync();

            OnPropertyChanged("ShiftStarted");
            OnPropertyChanged("BreakButtonColor");
            OnPropertyChanged("StartBreakText");
            OnPropertyChanged("OnBreak");
            OnPropertyChanged("DriveShiftRunning");
        }

        private async Task ToggleShift()
        {
            await Application.locator.StartListeningAsync(2000, 0, true);
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

                if (await Application.Current.MainPage.DisplayAlert("Confirmation", "Would you like to start your shift?", "Yes", "No"))
                {
                    if (await StartShift().ConfigureAwait(false))
                    {
                        if (await dbService.StartShift())
                        {
                            ShowEndShiftXAML();
                            UpdateCircularGauge();

                            DependencyService.Get<INotifyService>().UpdateNotification("Shift Running", "Your Shift is Running", false);

                            CancellationTokenSource cts = this.cancel;

                            Device.StartTimer(TimeSpan.FromHours(13), () =>
                            {
                                DependencyService.Get<INotifyService>().UpdateNotification("Shift End", "You have less than 1 hour left in your shift", true);
                                return false;
                            });
                        }
                    }
                }
            }
            else
            {
                if (!dbService.VehicleActive())
                {
                    if (dbService.CheckOnBreak() == 1)
                    {
                        if (await Application.Current.MainPage.DisplayAlert("Confirmation", "Would you like to end your shift?", "Yes", "No"))
                        {
                            if (await dbService.StopShift())
                            {
                                await Navigation.PushModalAsync(new NZTAMessagePage(2));
                                ShowStartShiftXAML();

                                DependencyService.Get<INotifyService>().UpdateNotification("Ready", "Ready to record your shifts", false);

                                Interlocked.Exchange(ref this.cancel, new CancellationTokenSource()).Cancel();
                            }
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("ERROR", "Please end your break before ending your work shift", "Gotcha");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("ERROR", "Please end your driving shift before ending your work shift", "Gotcha");
                }
            }

            await Application.locator.StopListeningAsync();
        }

        private void ShowEndShiftXAML()
        {
            ShiftText = "End Shift";
            ShiftButtonColor = Color.FromHex("#cc0000");
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
            ShiftButtonColor = Color.FromHex("#009900");
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
            notifyReady = false;
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

            CountdownConverter convert = new CountdownConverter();
            ToastConfig toast = new ToastConfig("You have " + convert.Convert(14 - CompletedJourney, null, null, null) + " left in your shift");

            if ((14 - CompletedJourney) < 1)
            {
                UserDialogs.Instance.Toast(toast);

                notifyReady = true;
            }

            Device.StartTimer(TimeSpan.FromMinutes(5), () =>
            {
                CompletedJourney = dbService.TotalSinceStart();

                OnPropertyChanged("CompletedJourney");

                if ((14 - CompletedJourney) < 1 && !notifyReady)
                {
                    toast = new ToastConfig("You have " + convert.Convert(14 - CompletedJourney, null, null, null) + " left in your shift");
                    UserDialogs.Instance.Toast(toast);

                    notifyReady = true;
                }

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