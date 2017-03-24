// <copyright file="HomeViewModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
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
        private bool notifyReady;

        private string shiftTimes;
        private string nextBreakTime;
        private bool canStartShift;
        private string canStartShiftText;

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

            MessagingCenter.Subscribe<string>("Toggle Shift", "Toggle Shift", async (s) =>
            {
                await ToggleShift();
            });

            MessagingCenter.Subscribe<string>("Toggle Drive", "Toggle Drive", async (s) =>
            {
                await ToggleDrive();
            });

            MessagingCenter.Subscribe<string>("Toggle Break", "Toggle Break", async (s) =>
            {
                await ToggleBreak();
            });

            // List<string> test = dbService.CheckPossiblities("DMNIi\u03B8");
            CompletedJourney = 0;
            RemainderOfJourney = 0;
            Break = 0;

            CheckActiveShift();
            CheckActiveBreak();

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

            ShiftButton = new RelayCommand(async () => await ToggleShift());
            EndShiftText = Resource.EndShift;
            AddNoteText = Resource.AddNote;
            StartBreakCommand = new RelayCommand(async () => await ToggleBreak());
            VehicleCommand = new RelayCommand(async () => await ToggleDrive());
            SetVehicleLabel();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool FullBreak { get; set; }

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

        public bool CanStartShift
        {
            get
            {
                return canStartShift;
            }

            set
            {
                canStartShift = value;
                OnPropertyChanged("CanStartShift");
            }
        }

        public string VehiclePickerText { get; set; }

        public bool PickerEnabled { get; set; }

        public string NextBreakTime
        {
            get
            {
                return nextBreakTime;
            }

            set
            {
                nextBreakTime = value;
                OnPropertyChanged("NextBreakTime");
            }
        }

        public string ShiftTimes
        {
            get
            {
                return shiftTimes;
            }

            set
            {
                shiftTimes = value;
                OnPropertyChanged("ShiftTimes");
            }
        }

        public FileImageSource ShiftImage { get; set; }

        public ICommand StartBreakCommand { get; set; }

        public ICommand VehicleCommand { get; set; }

        public bool OnBreak { get; set; }

        public string CanStartShiftText
        {
            get
            {
                return canStartShiftText;
            }

            set
            {
                canStartShiftText = value;
                OnPropertyChanged("CanStartShiftText");
            }
        }

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

        public void PageReload()
        {
            CheckActiveShift();
            CheckActiveBreak();

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
        }

        public List<VehicleTable> GetVehicles()
        {
            listOfVehicles = new List<VehicleTable>();
            listOfVehicles = dbService.GetVehicles();
            return listOfVehicles;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

                VehiclePickerText = currentVehicle.Registration;
                VehicleText = Resource.StopDriving;
                VehicleTextColor = Constants.YELLOW_COLOR;
                VehicleInUse = true;
                DriveShiftRunning = true;

                GeoCollection();
            }
            else
            {
                VehiclePickerText = Resource.NoActiveVehicle;
                VehicleText = Resource.StartDriving;
                VehicleTextColor = Constants.GREEN_COLOR;
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
                ShowEndShiftXAML();

                UpdateCircularGauge();

                if (dbService.CheckOnBreak() == -1)
                {
                    if (!dbService.CheckTimedNotification(NotificationCategory.Ongoing))
                    {
                        dbService.CreateNotification("You are currently on your break", false, NotificationCategory.Ongoing);
                    }

                    DependencyService.Get<INotifyService>().PresentNotification("On Break", "You are currently on your break", false);
                }
                else
                {
                    if (!dbService.CheckTimedNotification(NotificationCategory.Ongoing))
                    {
                        dbService.CreateNotification("You are currently working", false, NotificationCategory.Ongoing);
                    }

                    DependencyService.Get<INotifyService>().PresentNotification("Shift Running", "You are currently working", false);

                    if (!dbService.CheckTimedNotification(NotificationCategory.Break))
                    {
                        if ((TotalBeforeBreak - 1) > 1)
                        {
                            dbService.CreateNotification("You have less than 1 hour until your break", true, NotificationCategory.Break, TimeSpan.FromHours(TotalBeforeBreak - 1));

                            Device.StartTimer(TimeSpan.FromHours(TotalBeforeBreak - 1), () =>
                            {
                                if (!dbService.CheckTimedNotification(NotificationCategory.Break))
                                {
                                    return false;
                                }

                                DependencyService.Get<INotifyService>().UpdateNotification("Break Approaching", "You have less than 1 hour until your break", true);

                                dbService.CancelNotification(NotificationCategory.Break, true, true);
                                dbService.CreateNotification("You have less than 10 mins until your break", true, NotificationCategory.Break, TimeSpan.FromMinutes(50));

                                Device.StartTimer(TimeSpan.FromMinutes(50), () =>
                                {
                                    if (!dbService.CheckTimedNotification(NotificationCategory.Break))
                                    {
                                        return false;
                                    }

                                    DependencyService.Get<INotifyService>().UpdateNotification("Break Approaching", "You have less than 10 mins until your break", true);

                                    dbService.CancelNotification(NotificationCategory.Break, true, true);
                                    dbService.CreateNotification("It is time for your break", true, NotificationCategory.Break, TimeSpan.FromMinutes(10));

                                    Device.StartTimer(TimeSpan.FromMinutes(10), () =>
                                    {
                                        if (!dbService.CheckTimedNotification(NotificationCategory.Break))
                                        {
                                            return false;
                                        }

                                        DependencyService.Get<INotifyService>().UpdateNotification("Break Start", "It is time for your break", true);

                                        dbService.CancelNotification(NotificationCategory.Break, true, true);
                                        return false;
                                    });
                                    return false;
                                });
                                return false;
                            });
                        }
                        else
                        {
                            if ((TimeSpan.FromHours(TotalBeforeBreak) - TimeSpan.FromMinutes(10)) > TimeSpan.FromMinutes(10))
                            {
                                dbService.CreateNotification("You have less than 1 hour until your break", true, NotificationCategory.Break, TimeSpan.FromHours(TotalBeforeBreak) - TimeSpan.FromMinutes(10));

                                DependencyService.Get<INotifyService>().UpdateNotification("Break Approaching", "You have less than 1 hour until your break", true);

                                dbService.CancelNotification(NotificationCategory.Break, true, true);
                                dbService.CreateNotification("You have less than 10 mins until your break", true, NotificationCategory.Break, TimeSpan.FromHours(TotalBeforeBreak) - TimeSpan.FromMinutes(10));

                                Device.StartTimer(TimeSpan.FromHours(TotalBeforeBreak) - TimeSpan.FromMinutes(10), () =>
                                {
                                    if (!dbService.CheckTimedNotification(NotificationCategory.Break))
                                    {
                                        return false;
                                    }

                                    DependencyService.Get<INotifyService>().UpdateNotification("Break Approaching", "You have less than 10 mins until your break", true);

                                    dbService.CancelNotification(NotificationCategory.Break, true, true);
                                    dbService.CreateNotification("It is time for your break", true, NotificationCategory.Break, TimeSpan.FromMinutes(10));

                                    Device.StartTimer(TimeSpan.FromMinutes(10), () =>
                                    {
                                        if (!dbService.CheckTimedNotification(NotificationCategory.Break))
                                        {
                                            return false;
                                        }

                                        DependencyService.Get<INotifyService>().UpdateNotification("Break Start", "It is time for your break", true);

                                        dbService.CancelNotification(NotificationCategory.Break, true, true);
                                        return false;
                                    });
                                    return false;
                                });
                            }
                            else
                            {
                                if (TimeSpan.FromHours(TotalBeforeBreak) > TimeSpan.FromHours(0))
                                {
                                    dbService.CreateNotification("You have less than 10 mins until your break", true, NotificationCategory.Break, TimeSpan.FromHours(TotalBeforeBreak));

                                    DependencyService.Get<INotifyService>().UpdateNotification("Break Approaching", "You have less than 10 mins until your break", true);

                                    dbService.CancelNotification(NotificationCategory.Break, true, true);
                                    dbService.CreateNotification("It is time for your break", true, NotificationCategory.Break, TimeSpan.FromHours(TotalBeforeBreak));

                                    Device.StartTimer(TimeSpan.FromHours(TotalBeforeBreak), () =>
                                    {
                                        if (!dbService.CheckTimedNotification(NotificationCategory.Break))
                                        {
                                            return false;
                                        }

                                        DependencyService.Get<INotifyService>().UpdateNotification("Break Start", "It is time for your break", true);

                                        dbService.CancelNotification(NotificationCategory.Break, true, true);
                                        return false;
                                    });
                                }
                                else
                                {
                                    dbService.CreateNotification("It is time for your break", false, NotificationCategory.Break);

                                    DependencyService.Get<INotifyService>().UpdateNotification("Break Start", "It is time for your break", true);
                                }
                            }
                        }
                    }

                    if (!dbService.CheckTimedNotification(NotificationCategory.Shift))
                    {
                        double remainingShift = 14 - CompletedJourney;

                        if ((remainingShift - 1) > 1)
                        {
                            dbService.CreateNotification("You have less than 1 hour until your shift ends", true, NotificationCategory.Shift, TimeSpan.FromHours(remainingShift - 1));

                            Device.StartTimer(TimeSpan.FromHours(remainingShift - 1), () =>
                            {
                                if (!dbService.CheckTimedNotification(NotificationCategory.Shift))
                                {
                                    return false;
                                }

                                DependencyService.Get<INotifyService>().UpdateNotification("Shift Ending", "You have less than 1 hour until your shift ends", true);

                                dbService.CancelNotification(NotificationCategory.Shift, true, true);
                                dbService.CreateNotification("You have less than 10 mins until your shift ends", true, NotificationCategory.Shift, TimeSpan.FromMinutes(50));

                                Device.StartTimer(TimeSpan.FromMinutes(50), () =>
                                {
                                    if (!dbService.CheckTimedNotification(NotificationCategory.Shift))
                                    {
                                        return false;
                                    }

                                    DependencyService.Get<INotifyService>().UpdateNotification("Shift Ending", "You have less than 10 mins until your shift ends", true);

                                    dbService.CancelNotification(NotificationCategory.Shift, true, true);
                                    dbService.CreateNotification("It is time for your shift to end", true, NotificationCategory.Shift, TimeSpan.FromMinutes(10));

                                    Device.StartTimer(TimeSpan.FromMinutes(10), () =>
                                    {
                                        if (!dbService.CheckTimedNotification(NotificationCategory.Shift))
                                        {
                                            return false;
                                        }

                                        DependencyService.Get<INotifyService>().UpdateNotification("Shift End", "It is time for your shift to end", true);

                                        dbService.CancelNotification(NotificationCategory.Shift, true, true);
                                        return false;
                                    });
                                    return false;
                                });
                                return false;
                            });
                        }
                        else
                        {
                            if ((TimeSpan.FromHours(remainingShift) - TimeSpan.FromMinutes(10)) > TimeSpan.FromMinutes(10))
                            {
                                dbService.CreateNotification("You have less than 1 hour until your shift ends", true, NotificationCategory.Shift, TimeSpan.FromHours(remainingShift) - TimeSpan.FromMinutes(10));

                                DependencyService.Get<INotifyService>().UpdateNotification("Shift Ending", "You have less than 1 hour until your shift ends", true);

                                dbService.CancelNotification(NotificationCategory.Shift, true, true);
                                dbService.CreateNotification("You have less than 10 mins until your shift ends", true, NotificationCategory.Shift, TimeSpan.FromHours(remainingShift) - TimeSpan.FromMinutes(10));

                                Device.StartTimer(TimeSpan.FromHours(remainingShift) - TimeSpan.FromMinutes(10), () =>
                                {
                                    if (!dbService.CheckTimedNotification(NotificationCategory.Shift))
                                    {
                                        return false;
                                    }

                                    DependencyService.Get<INotifyService>().UpdateNotification("Shift Ending", "You have less than 10 mins until your shift ends", true);

                                    dbService.CancelNotification(NotificationCategory.Shift, true, true);
                                    dbService.CreateNotification("It is time for your shift to end", true, NotificationCategory.Shift, TimeSpan.FromMinutes(10));

                                    Device.StartTimer(TimeSpan.FromMinutes(10), () =>
                                    {
                                        if (!dbService.CheckTimedNotification(NotificationCategory.Shift))
                                        {
                                            return false;
                                        }

                                        DependencyService.Get<INotifyService>().UpdateNotification("Shift End", "It is time for your shift to end", true);

                                        dbService.CancelNotification(NotificationCategory.Shift, true, true);
                                        return false;
                                    });
                                    return false;
                                });
                            }
                            else
                            {
                                if (TimeSpan.FromHours(remainingShift) > TimeSpan.FromHours(0))
                                {
                                    dbService.CreateNotification("You have less than 10 mins until your shift ends", true, NotificationCategory.Shift, TimeSpan.FromHours(remainingShift));

                                    DependencyService.Get<INotifyService>().UpdateNotification("Shift Ending", "You have less than 10 mins until your shift ends", true);

                                    dbService.CancelNotification(NotificationCategory.Shift, true, true);
                                    dbService.CreateNotification("It is time for your shift to end", true, NotificationCategory.Shift, TimeSpan.FromHours(remainingShift));

                                    Device.StartTimer(TimeSpan.FromHours(remainingShift), () =>
                                    {
                                        if (!dbService.CheckTimedNotification(NotificationCategory.Shift))
                                        {
                                            return false;
                                        }

                                        DependencyService.Get<INotifyService>().UpdateNotification("Shift End", "It is time for your shift to end", true);

                                        dbService.CancelNotification(NotificationCategory.Shift, true, true);
                                        return false;
                                    });
                                }
                                else
                                {
                                    dbService.CreateNotification("It is time for your shift to end", false, NotificationCategory.Shift);

                                    DependencyService.Get<INotifyService>().UpdateNotification("Shift End", "It is time for your shift to end", true);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                ShiftStarted = false;
                ShowStartShiftXAML();

                if (!dbService.CheckTimedNotification(NotificationCategory.Ongoing))
                {
                    dbService.CreateNotification("This app is ready to record your shift", false, NotificationCategory.Ongoing);
                }

                DependencyService.Get<INotifyService>().PresentNotification("Ready", "This app is ready to record your shift", false);
            }
        }

        private void ShowEndShiftXAML()
        {
            ShiftText = "End Shift";
            ShiftButtonColor = Constants.RED_COLOR;
            StartShiftVisibility = false;
            ShiftStarted = true;
            ShiftRunning = true;
            ShiftImage = "Stop.png";
            ShiftTimes = "End Shift by: " + dbService.GetShiftTimes();
            NextBreakTime = "Take your break by: " + dbService.GetNextBreakTime();
            CanStartShift = false;

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
            ShiftButtonColor = Constants.GREEN_COLOR;
            StartShiftVisibility = true;
            ShiftStarted = false;
            ShiftRunning = false;
            ShiftImage = "Play.png";
            CanStartShiftText = dbService.GetLastShiftTime();
            CanStartShift = true;

            OnPropertyChanged("ShiftText");
            OnPropertyChanged("ShiftRunning");
            OnPropertyChanged("ShiftButtonColor");
            OnPropertyChanged("StartShiftVisibility");
            OnPropertyChanged("ShiftStarted");
            OnPropertyChanged("ShiftImage");
        }

        private async void CheckActiveBreak()
        {
            int onBreak = dbService.CheckOnBreak();
            if (onBreak == -1)
            {
                BreakButtonColor = Constants.RED_COLOR;
                StartBreakText = Resource.EndBreak;
                OnBreak = true;
                ShiftRunning = false;

                DateTime startTime = dbService.GetBreakStart();

                if ((DateTime.Now - startTime) <= TimeSpan.FromMinutes(30))
                {
                    countdown.Restart(30 * 60, startTime);
                }
                else
                {
                    if (await UserDialogs.Instance.ConfirmAsync("You have used up all the time in this break. Would you like to end it now", "Break End", "Yes", "No"))
                    {
                        await ToggleBreak();
                    }

                    // else
                    // {
                    //    PromptConfig notePrompt = new PromptConfig()
                    //    {
                    //        IsCancellable = false,
                    //        OkText = "Okay",
                    //        Message = "Note: "
                    //    };

                    // PromptResult result = await UserDialogs.Instance.PromptAsync(notePrompt);
                    //    if (result.Ok && result.Text != string.Empty)
                    //    {
                    //        return result.Text;
                    //    }
                    // }
                }
            }
            else if (onBreak == 1)
            {
                BreakButtonColor = Constants.GREEN_COLOR;
                StartBreakText = Resource.StartBreak;
                OnBreak = false;
                ShiftRunning = true;

                FullBreak = dbService.CheckFullBreak();
            }
        }

        private async Task<bool> StartDrive()
        {
            int vehicleKey;
            bool offlineDrive = false;
            if (await UserDialogs.Instance.ConfirmAsync("Would you like to start your drive?", "Confirmation", "Yes", "No"))
            {
                string location;

                using (UserDialogs.Instance.Loading("Getting Coordinates....", null, null, true, MaskType.Gradient))
                {
                    location = await GetLocation(await restApi.GetLatAndLong().ConfigureAwait(false));
                }

                if (location == string.Empty)
                {
                    return false;
                }

                List<VehicleTable> listOfVehicles = dbService.GetVehicles();

                var vehicleResult = await UserDialogs.Instance.ActionSheetAsync("Choose Vehicle:", Resource.Cancel, "Add Vehicle...", null, listOfVehicles.Select(l => l.Registration).ToArray());
                using (UserDialogs.Instance.Loading("Adding Vehicle....", null, null, true, MaskType.Gradient))
                {
                    if (vehicleResult == "Add Vehicle...")
                    {
                        vehicleKey = await dbService.GetRego();
                        if (vehicleKey < 0)
                        {
                            return false;
                        }

                        // Was unsuccessful at adding to remote server, thus was added locally, and the drive will have to be added locally until done remotely
                        if (!await dbService.InsertVehicle(vehicleKey))
                        {
                            offlineDrive = true;
                        }
                    }
                    else
                    {
                        VehicleTable vehicle = listOfVehicles.Where(v => v.Registration == vehicleResult).First();
                        vehicleKey = vehicle.Key;
                    }
                }

                int hubo = await HuboPrompt();

                if (hubo == 0)
                {
                    return false;
                }

                string note = await NotePromptAsync();

                if ((location == string.Empty) || (hubo == 0))
                {
                    return false;
                }

                if (offlineDrive)
                {
                    if (!await dbService.StartOfflineDriveAsync(hubo, note, location, vehicleKey))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!await dbService.StartDrive(hubo, note, location, vehicleKey))
                    {
                        return false;
                    }
                }

                UserDialogs.Instance.ShowSuccess("Drive Started!", 1500);
                SetVehicleLabel();
                GeoCollection();
                return true;
            }

            return false;
        }

        private async Task<string> NotePromptAsync()
        {
            if (await UserDialogs.Instance.ConfirmAsync("Would you like to add a note?", "Alert", "I would", "Nope"))
            {
                PromptConfig notePrompt = new PromptConfig()
                {
                    CancelText = "Cancel",
                    IsCancellable = true,
                    OkText = "Okay",
                    Message = "Note: "
                };

                PromptResult result = await UserDialogs.Instance.PromptAsync(notePrompt);
                if (result.Ok && result.Text != string.Empty)
                {
                    return result.Text;
                }
            }

            return string.Empty;
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

                if (location == string.Empty)
                {
                    return false;
                }

                int hubo = await HuboPrompt();

                if (hubo == 0)
                {
                    return false;
                }

                string note = await NotePromptAsync();

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
            await Application.Locator.StartListeningAsync(2000, 0, true);

            if (!DriveShiftRunning)
            {
                await StartDrive();
            }
            else
            {
                await StopDrive();
            }

            await Application.Locator.StopListeningAsync();
        }

        private async Task ToggleBreak()
        {
            await Application.Locator.StartListeningAsync(2000, 0, true);

            if (!OnBreak)
            {
                await StartBreak();
            }
            else
            {
                await StopBreak();
            }

            await Application.Locator.StopListeningAsync();

            OnPropertyChanged("ShiftStarted");
            OnPropertyChanged("BreakButtonColor");
            OnPropertyChanged("StartBreakText");
            OnPropertyChanged("OnBreak");
        }

        private async Task StartBreak()
        {
            if (await UserDialogs.Instance.ConfirmAsync("Are you sure you want to go on a break?", Resource.DisplayAlertTitle, "Yes", "No"))
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
                    BreakButtonColor = Color.FromHex("#cc0000");
                    StartBreakText = Resource.EndBreak;
                    OnBreak = true;
                    ShiftRunning = false;

                    if (FullBreak)
                    {
                        countdown.Start(30 * 60);
                    }
                    else
                    {
                        countdown.Start(RemainTime);
                    }

                    UserDialogs.Instance.ShowSuccess("Break Started!", 1500);
                }
            }
        }

        private async Task StopBreak()
        {
            if (RemainTime > 0)
            {
                TimeSpan time = TimeSpan.FromSeconds(TotalTime - RemainTime);
                string remainTimeString = time.ToString(@"mm\:ss");
                FullBreak = false;
                if (!await UserDialogs.Instance.ConfirmAsync("You have only had a " + remainTimeString + " minute break, this will not count as a full 30 min break, continue anyway?", "WARNING", "Yes", "No"))
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
                if (FullBreak)
                {
                    NextBreakTime = "Take your break by: " + dbService.GetNextBreakTime();

                    dbService.CancelNotification(NotificationCategory.Break, true);

                    dbService.CreateNotification("You have less than 1 hour until your break", true, NotificationCategory.Break, TimeSpan.FromHours(4.5));

                    Device.StartTimer(TimeSpan.FromHours(4.5), () =>
                    {
                        if (!dbService.CheckTimedNotification(NotificationCategory.Break))
                        {
                            return false;
                        }

                        DependencyService.Get<INotifyService>().UpdateNotification("Break Approaching", "You have less than 1 hour until your break", true);

                        dbService.CancelNotification(NotificationCategory.Break, true, true);
                        dbService.CreateNotification("You have less than 10 mins until your break", true, NotificationCategory.Break, TimeSpan.FromMinutes(50));

                        Device.StartTimer(TimeSpan.FromMinutes(50), () =>
                        {
                            if (!dbService.CheckTimedNotification(NotificationCategory.Break))
                            {
                                return false;
                            }

                            DependencyService.Get<INotifyService>().UpdateNotification("Break Approaching", "You have less than 10 mins until your break", true);

                            dbService.CancelNotification(NotificationCategory.Break, true, true);
                            dbService.CreateNotification("It is time for your break", true, NotificationCategory.Break, TimeSpan.FromMinutes(10));

                            Device.StartTimer(TimeSpan.FromMinutes(10), () =>
                            {
                                if (!dbService.CheckTimedNotification(NotificationCategory.Break))
                                {
                                    return false;
                                }

                                DependencyService.Get<INotifyService>().UpdateNotification("Break Start", "It is time for your break", true);

                                dbService.CancelNotification(NotificationCategory.Break, true, true);
                                return false;
                            });
                            return false;
                        });
                        return false;
                    });
                }

                BreakButtonColor = Constants.GREEN_COLOR;
                StartBreakText = Resource.StartBreak;
                OnBreak = false;
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
            await Application.Locator.StartListeningAsync(2000, 0, true);
            bool success = false;
            if (!ShiftStarted)
            {
                success = await StartShift();
            }
            else
            {
                success = await StopShift();
            }

            await Application.Locator.StopListeningAsync();
        }

        private async Task<string> GetLocation(Geolocation geoCoords)
        {
            string location = await restApi.GetLocation(geoCoords);
            PromptConfig locationPrompt = new PromptConfig()
            {
                IsCancellable = true,
                Title = "Current Location: ",
                Text = location
            };
            PromptResult promptResult = await UserDialogs.Instance.PromptAsync(locationPrompt);

            if (!promptResult.Ok || promptResult.Text == string.Empty)
            {
                return string.Empty;
            }

            return promptResult.Text;
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

                            if (location == string.Empty)
                            {
                                return false;
                            }

                            string note = await NotePrompt();

                            if (await dbService.StopShift(location, note, geoCoords))
                            {
                                ShiftStarted = false;
                                ShowStartShiftXAML();
                                UserDialogs.Instance.ShowSuccess("Shift Ended!", 1500);
                                await Navigation.PushModalAsync(new NZTAMessagePage(2));

                                dbService.CancelNotification(NotificationCategory.Ongoing, false);
                                dbService.CancelNotification(NotificationCategory.Shift, true);
                                dbService.CancelNotification(NotificationCategory.Break, true);

                                dbService.CreateNotification("Ready to record your shifts", false, NotificationCategory.Ongoing);

                                DependencyService.Get<INotifyService>().UpdateNotification("Ready", "Ready to record your shifts", false);

                                return true;
                            }

                            return false;
                        }
                    }
                }
                else
                {
                    await UserDialogs.Instance.ConfirmAsync("Please end your break before ending your work shift", "ERROR", "Gotcha");
                }
            }
            else
            {
                await UserDialogs.Instance.ConfirmAsync("Please end your driving shift before ending your work shift", "ERROR", "Gotcha");
            }

            return false;
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
                if (!dbService.CheckTenHourBreak())
                {
                    await UserDialogs.Instance.AlertAsync("You have not had your full 10 hour break between shifts", Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                }

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
                    ShiftStarted = true;
                    ShowEndShiftXAML();
                    UpdateCircularGauge();

                    dbService.CancelNotification(NotificationCategory.Ongoing, false);

                    dbService.CreateNotification("Your Shift is Running", false, NotificationCategory.Ongoing);
                    DependencyService.Get<INotifyService>().UpdateNotification("Shift Running", "Your Shift is Running", false);

                    dbService.CreateNotification("You have less than 1 hour until your break", true, NotificationCategory.Break, TimeSpan.FromHours(4.5));

                    Device.StartTimer(TimeSpan.FromHours(4.5), () =>
                    {
                        if (!dbService.CheckTimedNotification(NotificationCategory.Break))
                        {
                            return false;
                        }

                        DependencyService.Get<INotifyService>().UpdateNotification("Break Approaching", "You have less than 1 hour until your break", true);

                        dbService.CancelNotification(NotificationCategory.Break, true, true);
                        dbService.CreateNotification("You have less than 10 mins until your break", true, NotificationCategory.Break, TimeSpan.FromMinutes(50));

                        Device.StartTimer(TimeSpan.FromMinutes(50), () =>
                        {
                            if (!dbService.CheckTimedNotification(NotificationCategory.Break))
                            {
                                return false;
                            }

                            DependencyService.Get<INotifyService>().UpdateNotification("Break Approaching", "You have less than 10 mins until your break", true);

                            dbService.CancelNotification(NotificationCategory.Break, true, true);
                            dbService.CreateNotification("It is time for your break", true, NotificationCategory.Break, TimeSpan.FromMinutes(10));

                            Device.StartTimer(TimeSpan.FromMinutes(10), () =>
                            {
                                if (!dbService.CheckTimedNotification(NotificationCategory.Break))
                                {
                                    return false;
                                }

                                DependencyService.Get<INotifyService>().UpdateNotification("Break Start", "It is time for your break", true);

                                dbService.CancelNotification(NotificationCategory.Break, true, true);
                                return false;
                            });
                            return false;
                        });
                        return false;
                    });

                    await Application.Locator.StopListeningAsync();

                    dbService.CreateNotification("You have less than 1 hour left in your shift", true, NotificationCategory.Shift, TimeSpan.FromHours(13));
                    Device.StartTimer(TimeSpan.FromHours(13), () =>
                    {
                        if (dbService.CheckTimedNotification(NotificationCategory.Shift))
                        {
                            return false;
                        }

                        DependencyService.Get<INotifyService>().UpdateNotification("Shift End", "You have less than 1 hour left in your shift", true);

                        dbService.CancelNotification(NotificationCategory.Shift, true, true);
                        dbService.CreateNotification("You have less than 10 mins left in your shift", true, NotificationCategory.Shift, TimeSpan.FromMinutes(50));

                        Device.StartTimer(TimeSpan.FromMinutes(50), () =>
                        {
                            if (dbService.CheckTimedNotification(NotificationCategory.Shift))
                            {
                                return false;
                            }

                            DependencyService.Get<INotifyService>().UpdateNotification("Shift End", "You have less than 10 mins left in your shift", true);

                            dbService.CancelNotification(NotificationCategory.Shift, true, true);
                            dbService.CreateNotification("Your shift is at an end", true, NotificationCategory.Shift, TimeSpan.FromMinutes(10));

                            Device.StartTimer(TimeSpan.FromMinutes(10), () =>
                            {
                                if (dbService.CheckTimedNotification(NotificationCategory.Shift))
                                {
                                    return false;
                                }

                                DependencyService.Get<INotifyService>().UpdateNotification("Shift End", "Your shift is at an end", true);

                                dbService.CancelNotification(NotificationCategory.Shift, true, true);
                                return false;
                            });
                            return false;
                        });
                        return false;
                    });
                    UserDialogs.Instance.ShowSuccess("Shift Started!", 1500);

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

            if ((14 - CompletedJourney) < 1)
            {
                ToastConfig toast = new ToastConfig("You have " + convert.Convert(50400 - ((CompletedJourney * 60) * 60), null, null, null) + " left in your shift");
                UserDialogs.Instance.Toast(toast);

                notifyReady = true;
            }

            Device.StartTimer(TimeSpan.FromMinutes(5), () =>
            {
                CompletedJourney = dbService.TotalSinceStart();

                OnPropertyChanged("CompletedJourney");

                if ((14 - CompletedJourney) < 1 && !notifyReady)
                {
                    ToastConfig toast = new ToastConfig("You have " + convert.Convert(50400 - ((CompletedJourney * 60) * 60), null, null, null) + " left in your shift");
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