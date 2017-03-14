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
    using Acr.UserDialogs;
    using Plugin.Battery;
    using Xamarin.Forms;
    using XLabs;

    internal class HomeViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService dbService = new DatabaseService();

        private readonly Countdown countdown = new Countdown();

        private RestService restApi = new RestService();

        private List<VehicleTable> listOfVehicles;

        private VehicleTable currentVehicle;

        private bool driveShiftRunning;

        private bool isRunning;
        private double totalTime;
        private double remainTime;
        private int notificationId;
        private bool notifyReady;

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
                ShiftStarted = true;
                ToggleShiftXaml();
            }
            else
            {
                ShiftStarted = false;
                ToggleShiftXaml();
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

        private async Task<bool> StartDrive()
        {
            if (await UserDialogs.Instance.ConfirmAsync("Would you like to start your drive?", "Confirmation", "Yes", "No"))
            {

                string location;

                using (UserDialogs.Instance.Loading("Getting Coordinates....", null, null, true, MaskType.Gradient))
                {
                    location = await GetLocation(await restApi.GetLatAndLong().ConfigureAwait(false));
                }

                int hubo = await HuboPrompt();

                string note = await NotePrompt();

                if ((location == string.Empty) || (hubo == 0))
                {
                    return false;
                }

                if (await dbService.StartDrive(hubo, note, location))
                {
                    UserDialogs.Instance.ShowSuccess("Drive Started!", 1500);
                    SetVehicleLabel();
                    GeoCollection();
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> StopDrive()
        {
            if (await UserDialogs.Instance.ConfirmAsync("Would you like to end your drive?", "Confirmation", "Yes", "No"))
            {
                string location;

                using (UserDialogs.Instance.Loading("Getting Coordinates....", null, null, true, MaskType.Gradient))
                {
                    location = await GetLocation(await restApi.GetLatAndLong().ConfigureAwait(false));
                }

                int hubo = await HuboPrompt();

                string note = await NotePrompt();

                if ((location == string.Empty) || (hubo == 0))
                {
                    return false;
                }

                if (await dbService.StopDrive(hubo, note, location))
                {
                    UserDialogs.Instance.ShowSuccess("Drive Ended!", 1500);
                    SetVehicleLabel();
                    return true;
                }
            }

            return false;
        }

        private async Task<int> HuboPrompt()
        {
            bool invalidFormat = true;
            int hubo = 0;
            string promptTitle = "Current Odometer Reading: ";
            while (invalidFormat)
            {
                hubo = 0;

                PromptConfig huboPrompt = new PromptConfig()
                {
                    IsCancellable = true,
                    Title = promptTitle
                };
                huboPrompt.SetInputMode(InputType.Number);
                PromptResult promptResult = await UserDialogs.Instance.PromptAsync(huboPrompt);

                bool isNumeric = int.TryParse(promptResult.Text, out hubo);

                if (promptResult.Ok && isNumeric)
                {
                    invalidFormat = false;
                }

                if (!promptResult.Ok)
                {
                    return 0;
                }

                promptTitle = "Please enter a VALID odometer reading: ";
            }

            return hubo;
        }

        private async Task ToggleDrive()
        {
            await Application.locator.StartListeningAsync(2000, 0, true);

            if (!DriveShiftRunning)
            {
                await StartDrive();
            }
            else
            {
                await StopDrive();
            }

            await Application.locator.StopListeningAsync();

        }

        private async Task ToggleBreak()
        {
            await Application.locator.StartListeningAsync(2000, 0, true);

            if (!OnBreak)
            {
                await StartBreak();
            }
            else
            {
                await StopBreak();
            }

            await Application.locator.StopListeningAsync();

            OnPropertyChanged("ShiftStarted");
            OnPropertyChanged("BreakButtonColor");
            OnPropertyChanged("StartBreakText");
            OnPropertyChanged("OnBreak");
            OnPropertyChanged("DriveShiftRunning");
        }

        private async Task StartBreak()
        {
            if (await UserDialogs.Instance.ConfirmAsync("Are you sure you want to go on a break?", Resource.DisplayAlertTitle,  "Yes", "No"))
            {
                Geolocation geoCoords;
                string location;

                using (UserDialogs.Instance.Loading("Getting Coordinates....", null, null, true, MaskType.Gradient))
                {
                    geoCoords = await restApi.GetLatAndLong().ConfigureAwait(false);
                    location = await GetLocation(geoCoords);
                }

                if (location == string.Empty)
                {
                    return;
                }

                string note = await NotePrompt();

                if (await dbService.StartBreak(location, note))
                {
                    BreakButtonColor = Xamarin.Forms.Color.FromHex("#cc0000");
                    StartBreakText = Resource.EndBreak;
                    OnBreak = true;
                    DriveShiftRunning = false;
                    ShiftRunning = false;

                    countdown.Start(30 * 60);

                    UserDialogs.Instance.ShowSuccess("Break Started!", 1500);
                }
            }
        }

        private async Task StopBreak()
        {
            if (TotalTime > 0)
            {
                if (!await UserDialogs.Instance.ConfirmAsync("You have not had a full 30 minute break, this will not count as a break if you continue. Would you like to end it anyway?", "WARNING", "Yes", "No"))
                {
                    return;
                }
            }
            else
            {
                if (!await UserDialogs.Instance.ConfirmAsync("Are you sure you want to end your break?", Resource.DisplayAlertTitle, "Yes", "No"))
                {
                    return;
                }
            }

            Geolocation geoCoords;
            string location;

            using (UserDialogs.Instance.Loading("Getting Coordinates....", null, null, true, MaskType.Gradient))
            {
                geoCoords = await restApi.GetLatAndLong().ConfigureAwait(false);
                location = await GetLocation(geoCoords);
            }

            if (location == string.Empty)
            {
                return;
            }

            string note = await NotePrompt();

            if (await dbService.StopBreak(location, note))
            {
                BreakButtonColor = Xamarin.Forms.Color.FromHex("#009900");
                StartBreakText = Resource.StartBreak;
                OnBreak = false;
                DriveShiftRunning = true;
                ShiftRunning = true;

                if (dbService.VehicleActive())
                {
                    UserDialogs.Instance.ShowSuccess("Break Ended!", 1500);
                    GeoCollection();
                }

                UpdateCircularGauge();

                countdown.Stop();
            }
        }

        private async Task ToggleShift()
        {
            await Application.locator.StartListeningAsync(2000, 0, true);
            bool success = false;
            if (!ShiftStarted)
            {
                success = await StartShift();
            }
            else
            {
                success = await StopShift();
            }

            if (success)
            {
                if (ShiftStarted)
                {
                    ShiftStarted = false;
                }
                else
                {
                    ShiftStarted = true;
                }

                OnPropertyChanged("ShiftStarted");
                ToggleShiftXaml();
            }

            await Application.locator.StopListeningAsync();
        }

        private async Task<string> GetLocation(Geolocation geoCoords)
        {
            string location = await restApi.GetLocation(geoCoords);
            PromptConfig locationPrompt = new PromptConfig();
            locationPrompt.IsCancellable = true;
            locationPrompt.Title = "Current Location: ";
            locationPrompt.Text = location;
            PromptResult promptResult = await UserDialogs.Instance.PromptAsync(locationPrompt);

            if (!promptResult.Ok || promptResult.Text == string.Empty)
            {
                return string.Empty;
            }

            return location;
        }

        private async Task<bool> StopShift()
        {
            if (!dbService.VehicleActive())
            {
                if (dbService.CheckOnBreak() == 1)
                {
                    if (await UserDialogs.Instance.ConfirmAsync("Would you like to end your shift?", "Confirmation", "Yes", "No"))
                    {
                        Geolocation geoCoords;
                        string location;

                        using (UserDialogs.Instance.Loading("Getting Coordinates....", null, null, true, MaskType.Gradient))
                        {
                            geoCoords = await restApi.GetLatAndLong().ConfigureAwait(false);
                            location = await GetLocation(geoCoords);
                        }

                        if (location == string.Empty)
                        {
                            return false;
                        }

                        string note = await NotePrompt();

                        if (await dbService.StopShift(location, note, geoCoords))
                        {
                            UserDialogs.Instance.ShowSuccess("Shift Ended!", 1500);
                            MessagingCenter.Send<string>("ShiftEdited", "ShiftEdited");
                            await Navigation.PushModalAsync(new NZTAMessagePage(2));
                            DependencyService.Get<INotifyService>().CancelNotification(notificationId);
                            return true;
                        }

                        return false;
                    }

                    return false;
                }
                else
                {
                    await UserDialogs.Instance.ConfirmAsync("Please end your break before ending your work shift", "ERROR", "Gotcha");
                    return false;
                }
            }
            else
            {
                await UserDialogs.Instance.ConfirmAsync("Please end your driving shift before ending your work shift", "ERROR", "Gotcha");
                return false;
            }
        }

        private void ToggleShiftXaml()
        {
            if (ShiftStarted)
            {
                ShiftText = "End Shift";
                ShiftButtonColor = Xamarin.Forms.Color.FromHex("#cc0000");
                StartShiftVisibility = false;
                ShiftRunning = true;
            }
            else
            {
                ShiftText = "Start Shift";
                ShiftButtonColor = Xamarin.Forms.Color.FromHex("#009900");
                StartShiftVisibility = true;
                ShiftRunning = false;
            }

            OnPropertyChanged("ShiftText");
            OnPropertyChanged("ShiftRunning");
            OnPropertyChanged("ShiftButtonColor");
            OnPropertyChanged("StartShiftVisibility");
            OnPropertyChanged("ShiftImage");
        }

        private async Task<bool> StartShift()
        {
            if (CrossBattery.Current.Status == Plugin.Battery.Abstractions.BatteryStatus.Unknown)
            {
                await UserDialogs.Instance.ConfirmAsync("Unable to get battery status, Please use another device!", Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
            }

            if (CrossBattery.Current.RemainingChargePercent <= 50 && (CrossBattery.Current.Status == Plugin.Battery.Abstractions.BatteryStatus.Discharging || CrossBattery.Current.Status == Plugin.Battery.Abstractions.BatteryStatus.Unknown))
            {
                await UserDialogs.Instance.ConfirmAsync("Battery at " + CrossBattery.Current.RemainingChargePercent + "%, Please ensure the device is charged soon!", Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
            }

            if (await UserDialogs.Instance.ConfirmAsync("Would you like to start your shift?", "Confirmation", "Yes", "No"))
            {
                List<string> checklistQuestions = dbService.GetChecklist();
                int count = 0;
                foreach (string question in checklistQuestions)
                {
                    count++;
                    bool result = await UserDialogs.Instance.ConfirmAsync(question, Resource.ChecklistQuestionNumber + count.ToString(), Resource.Yes, Resource.No);
                    if (!result)
                    {
                        return false;
                    }
                }

                Geolocation geoCoords;
                string location;

                using (UserDialogs.Instance.Loading("Getting Coordinates....", null, null, true, MaskType.Gradient))
                {
                    geoCoords = await restApi.GetLatAndLong().ConfigureAwait(false);
                    location = await GetLocation(geoCoords);
                }

                if (location == string.Empty)
                {
                    return false;
                }

                string note = await NotePrompt();

                if (await dbService.StartShift(location, note, geoCoords))
                {
                    UpdateCircularGauge();
                    MessagingCenter.Send<string>("ShiftEdited", "ShiftEdited");
                    UserDialogs.Instance.ShowSuccess("Shift Started!", 1500);
                    CountdownConverter convert = new CountdownConverter();

                    //notificationId = 5;
                    //DependencyService.Get<INotifyService>().LocalNotification("Shift End", "You have " + convert.Convert(14 - CompletedJourney, null, null, null) + " left in your shift", DateTime.Now + TimeSpan.FromHours(13), notificationId);

                    return true;
                }
            }

            return false;
        }

        private void UpdateCircularGauge()
        {
            notifyReady = false;
            CompletedJourney = dbService.TotalSinceStart();

            if (CompletedJourney == -1)
            {
                UserDialogs.Instance.ConfirmAsync("More Than One Active Shift!", Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
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

                // notificationId = 5;
                // DependencyService.Get<INotifyService>().LocalNotification("Shift End", "You have " + convert.Convert(14 - CompletedJourney, null, null, null) + " left in your shift", DateTime.Now, notificationId);
                notifyReady = true;
            }

            Device.StartTimer(TimeSpan.FromMinutes(5), () =>
            {
                CompletedJourney = dbService.TotalSinceStart();

                OnPropertyChanged("CompletedJourney");

                if ((14 - CompletedJourney) < 1)
                {
                    UserDialogs.Instance.Toast(toast);

                    // notificationId = 5;
                    // DependencyService.Get<INotifyService>().LocalNotification("Shift End", "You have " + convert.Convert(14 - CompletedJourney, null, null, null) + " left in your shift", DateTime.Now, notificationId);
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

        private async Task<string> NotePrompt()
        {
            if (await UserDialogs.Instance.ConfirmAsync("Would you like to add a note?", "Confirmation", "I would", "Nope"))
            {
                PromptConfig notePrompt = new PromptConfig()
                {
                    IsCancellable = true,
                    Title = "Note"
                };
                PromptResult promptResult = await UserDialogs.Instance.PromptAsync(notePrompt);
                if (promptResult.Ok)
                {
                    return promptResult.Text;
                }
            }

            return string.Empty;
        }
    }
}