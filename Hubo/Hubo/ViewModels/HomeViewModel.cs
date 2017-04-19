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
    using System.IO;

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
        private string canStartShiftText1;
        private string canStartShiftText2;
        private double completedSeventy;

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

            MessagingCenter.Subscribe<string>("ReloadPage", "ReloadPage", async (s) =>
            {
                await PageReload();
                MessagingCenter.Unsubscribe<string>("ReloadPage", "ReloadPage");
            });

            // List<string> test = dbService.CheckPossiblities("DMNIi\u03B8");
            CompletedJourney = 0;
            RemainderOfJourney = 0;
            Break = 0;

            CheckActiveShift();
            CheckActiveBreak();

            if (dbService.CheckActiveShift())
            {
                UpdateCircularGauge();

                if (dbService.CheckOnBreak() == -1)
                {
                    if (!dbService.CheckOngoingNotification())
                    {
                        dbService.CreateNotification(Resource.NotifyOnBreak, false, NotificationCategory.Ongoing);
                    }

                    DependencyService.Get<INotifyService>().PresentNotification(Resource.NotifyBreakRunningTitle, Resource.NotifyOnBreak, false);
                }
                else
                {
                    if (!dbService.CheckOngoingNotification())
                    {
                        dbService.CreateNotification(Resource.NotifyOnShift, false, NotificationCategory.Ongoing);
                    }

                    DependencyService.Get<INotifyService>().PresentNotification(Resource.NotifyShiftRunningTitle, Resource.NotifyOnShift, false);

                    if (!dbService.CheckTimedNotification(NotificationCategory.Break))
                    {
                        if ((TotalBeforeBreak - 1) > 1)
                        {
                            dbService.CreateNotification(Resource.Notify1Break, true, NotificationCategory.Break, TimeSpan.FromHours(TotalBeforeBreak - 1));

                            Device.StartTimer(TimeSpan.FromHours(TotalBeforeBreak - 1), () =>
                            {
                                if (!dbService.CheckTimedNotification(NotificationCategory.Break, DateTime.Now))
                                {
                                    return false;
                                }

                                DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyApproachingBreak, Resource.Notify1Break, true);

                                dbService.CancelNotification(NotificationCategory.Break, true, true);
                                dbService.CreateNotification(Resource.Notify10Break, true, NotificationCategory.Break, TimeSpan.FromMinutes(50));

                                Device.StartTimer(TimeSpan.FromMinutes(50), () =>
                                {
                                    if (!dbService.CheckTimedNotification(NotificationCategory.Break, DateTime.Now))
                                    {
                                        return false;
                                    }

                                    DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyApproachingBreak, Resource.Notify10Break, true);

                                    dbService.CancelNotification(NotificationCategory.Break, true, true);
                                    dbService.CreateNotification(Resource.NotifyBreakTime, true, NotificationCategory.Break, TimeSpan.FromMinutes(10));

                                    Device.StartTimer(TimeSpan.FromMinutes(10), () =>
                                    {
                                        if (!dbService.CheckTimedNotification(NotificationCategory.Break, DateTime.Now))
                                        {
                                            return false;
                                        }

                                        DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyBreakTimeTitle, Resource.NotifyBreakTime, true);

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
                                dbService.CreateNotification(Resource.Notify1Break, true, NotificationCategory.Break, TimeSpan.FromHours(TotalBeforeBreak) - TimeSpan.FromMinutes(10));

                                DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyApproachingBreak, Resource.Notify1Break, true);

                                dbService.CancelNotification(NotificationCategory.Break, true, true);
                                dbService.CreateNotification(Resource.Notify10Break, true, NotificationCategory.Break, TimeSpan.FromHours(TotalBeforeBreak) - TimeSpan.FromMinutes(10));

                                Device.StartTimer(TimeSpan.FromHours(TotalBeforeBreak) - TimeSpan.FromMinutes(10), () =>
                                {
                                    if (!dbService.CheckTimedNotification(NotificationCategory.Break, DateTime.Now))
                                    {
                                        return false;
                                    }

                                    DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyApproachingBreak, Resource.Notify10Break, true);

                                    dbService.CancelNotification(NotificationCategory.Break, true, true);
                                    dbService.CreateNotification(Resource.NotifyBreakTime, true, NotificationCategory.Break, TimeSpan.FromMinutes(10));

                                    Device.StartTimer(TimeSpan.FromMinutes(10), () =>
                                    {
                                        if (!dbService.CheckTimedNotification(NotificationCategory.Break, DateTime.Now))
                                        {
                                            return false;
                                        }

                                        DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyBreakTimeTitle, Resource.NotifyBreakTime, true);

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
                                    dbService.CreateNotification(Resource.Notify10Break, true, NotificationCategory.Break, TimeSpan.FromHours(TotalBeforeBreak));

                                    DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyApproachingBreak, Resource.Notify10Break, true);

                                    dbService.CancelNotification(NotificationCategory.Break, true, true);
                                    dbService.CreateNotification(Resource.NotifyBreakTime, true, NotificationCategory.Break, TimeSpan.FromHours(TotalBeforeBreak));

                                    Device.StartTimer(TimeSpan.FromHours(TotalBeforeBreak), () =>
                                    {
                                        if (!dbService.CheckTimedNotification(NotificationCategory.Break, DateTime.Now))
                                        {
                                            return false;
                                        }

                                        DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyBreakTimeTitle, Resource.NotifyBreakTime, true);

                                        dbService.CancelNotification(NotificationCategory.Break, true, true);
                                        return false;
                                    });
                                }
                                else
                                {
                                    dbService.CreateNotification(Resource.NotifyBreakTime, false, NotificationCategory.Break);

                                    DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyBreakTimeTitle, Resource.NotifyBreakTime, true);
                                }
                            }
                        }
                    }
                }
            }

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
            CompletedSeventy = dbService.GetTotalOfSeventy();
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

        public double CompletedSeventy
        {
            get
            {
                return completedSeventy;
            }

            set
            {
                completedSeventy = value;
                OnPropertyChanged("CompletedSeventy");
            }
        }

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

        public string CanStartShiftText1
        {
            get
            {
                return canStartShiftText1;
            }

            set
            {
                canStartShiftText1 = value;
                OnPropertyChanged("CanStartShiftText1");
            }
        }

        public string CanStartShiftText2
        {
            get
            {
                return canStartShiftText2;
            }

            set
            {
                canStartShiftText2 = value;
                OnPropertyChanged("CanStartShiftText2");
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

        public Task PageReload()
        {
            CheckActiveShift();
            CheckActiveBreak();
            CompletedSeventy = dbService.GetTotalOfSeventy();

            if (dbService.CheckActiveShift())
            {
                UpdateCircularGauge();
            }

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

            return Task.FromResult(0);
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

                if (!dbService.CheckTimedNotification(NotificationCategory.Shift))
                {
                    double remainingShift = 14 - CompletedJourney;

                    if ((remainingShift - 1) > 1)
                    {
                        dbService.CreateNotification(Resource.Notify1Shift, true, NotificationCategory.Shift, TimeSpan.FromHours(remainingShift - 1));

                        Device.StartTimer(TimeSpan.FromHours(remainingShift - 1), () =>
                        {
                            if (!dbService.CheckTimedNotification(NotificationCategory.Shift, DateTime.Now))
                            {
                                return false;
                            }

                            DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyEndingShiftTitle, Resource.Notify1Shift, true);

                            dbService.CancelNotification(NotificationCategory.Shift, true, true);
                            dbService.CreateNotification(Resource.Notify10Shift, true, NotificationCategory.Shift, TimeSpan.FromMinutes(50));

                            Device.StartTimer(TimeSpan.FromMinutes(50), () =>
                            {
                                if (!dbService.CheckTimedNotification(NotificationCategory.Shift, DateTime.Now))
                                {
                                    return false;
                                }

                                DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyEndingShiftTitle, Resource.Notify10Shift, true);

                                dbService.CancelNotification(NotificationCategory.Shift, true, true);
                                dbService.CreateNotification(Resource.NotifyEndShift, true, NotificationCategory.Shift, TimeSpan.FromMinutes(10));

                                Device.StartTimer(TimeSpan.FromMinutes(10), () =>
                                {
                                    if (!dbService.CheckTimedNotification(NotificationCategory.Shift, DateTime.Now))
                                    {
                                        return false;
                                    }

                                    DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyEndShiftTitle, Resource.NotifyEndShift, true);

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
                            dbService.CreateNotification(Resource.Notify1Shift, true, NotificationCategory.Shift, TimeSpan.FromHours(remainingShift) - TimeSpan.FromMinutes(10));

                            DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyEndingShiftTitle, Resource.Notify1Shift, true);

                            dbService.CancelNotification(NotificationCategory.Shift, true, true);
                            dbService.CreateNotification(Resource.Notify10Shift, true, NotificationCategory.Shift, TimeSpan.FromHours(remainingShift) - TimeSpan.FromMinutes(10));

                            Device.StartTimer(TimeSpan.FromHours(remainingShift) - TimeSpan.FromMinutes(10), () =>
                            {
                                if (!dbService.CheckTimedNotification(NotificationCategory.Shift, DateTime.Now))
                                {
                                    return false;
                                }

                                DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyEndingShiftTitle, Resource.Notify10Shift, true);

                                dbService.CancelNotification(NotificationCategory.Shift, true, true);
                                dbService.CreateNotification(Resource.NotifyEndShift, true, NotificationCategory.Shift, TimeSpan.FromMinutes(10));

                                Device.StartTimer(TimeSpan.FromMinutes(10), () =>
                                {
                                    if (!dbService.CheckTimedNotification(NotificationCategory.Shift, DateTime.Now))
                                    {
                                        return false;
                                    }

                                    DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyEndShiftTitle, Resource.NotifyEndShift, true);

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
                                dbService.CreateNotification(Resource.Notify10Shift, true, NotificationCategory.Shift, TimeSpan.FromHours(remainingShift));

                                DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyEndingShiftTitle, Resource.Notify10Shift, true);

                                dbService.CancelNotification(NotificationCategory.Shift, true, true);
                                dbService.CreateNotification(Resource.NotifyEndShift, true, NotificationCategory.Shift, TimeSpan.FromHours(remainingShift));

                                Device.StartTimer(TimeSpan.FromHours(remainingShift), () =>
                                {
                                    if (!dbService.CheckTimedNotification(NotificationCategory.Shift, DateTime.Now))
                                    {
                                        return false;
                                    }

                                    DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyEndShiftTitle, Resource.NotifyEndShift, true);

                                    dbService.CancelNotification(NotificationCategory.Shift, true, true);
                                    return false;
                                });
                            }
                            else
                            {
                                dbService.CreateNotification(Resource.NotifyEndShift, false, NotificationCategory.Shift);

                                DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyEndShiftTitle, Resource.NotifyEndShift, true);
                            }
                        }
                    }
                }
            }
            else
            {
                ShiftStarted = false;
                ShowStartShiftXAML();

                if (!dbService.CheckOngoingNotification())
                {
                    dbService.CreateNotification(Resource.NotifyInactive, false, NotificationCategory.Ongoing);
                }

                DependencyService.Get<INotifyService>().PresentNotification(Resource.NotifyInactiveTitle, Resource.NotifyInactive, false);
            }
        }

        private void ShowEndShiftXAML()
        {
            ShiftText = Resource.EndShift;
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
            ShiftText = Resource.StartShift;
            ShiftButtonColor = Constants.GREEN_COLOR;
            StartShiftVisibility = true;
            ShiftStarted = false;
            ShiftRunning = false;
            ShiftImage = "Play.png";

            double endRestTime = dbService.GetLastShiftTime();
            if (endRestTime > 0)
            {
                DateTime restEnd = DateTime.Now + TimeSpan.FromMinutes(endRestTime);
                CanStartShiftText1 = Resource.RestBreakEnds;
                CanStartShiftText2 = restEnd.ToString("h: mm tt dddd");
            }
            else
            {
                CanStartShiftText1 = Resource.YouAreRestedPart1;
                CanStartShiftText2 = Resource.YouAreRestedPart2;
            }

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
                    if (await UserDialogs.Instance.ConfirmAsync(Resource.NoBreakRemaining, Resource.NotifyEndingBreakTitle, Resource.Yes, Resource.No))
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
            if (await UserDialogs.Instance.ConfirmAsync(Resource.StartDriveQuery, Resource.Confirmation, Resource.Yes, Resource.No))
            {
                List<string> checklistQuestions = dbService.GetChecklist();
                Geolocation geocords;
                int count = 0;
                foreach (string question in checklistQuestions)
                {
                    count++;
                    bool result = await UserDialogs.Instance.ConfirmAsync(question, Resource.ChecklistQuestionNumber + count.ToString(), Resource.Okay, Resource.NotOkay);
                    if (!result)
                    {
                        return false;
                    }
                }

                string location;

                using (UserDialogs.Instance.Loading(Resource.GetCoordinates, null, null, true, MaskType.Gradient))
                {
                    geocords = await restApi.GetLatAndLong().ConfigureAwait(false);
                    location = await GetLocation(geocords);
                }

                PromptConfig locationPrompt = new PromptConfig()
                {
                    IsCancellable = true,
                    Title = Resource.CurrentLocation,
                    Text = location
                };
                PromptResult promptResult = await UserDialogs.Instance.PromptAsync(locationPrompt);

                if (!promptResult.Ok || promptResult.Text == string.Empty)
                {
                    return false;
                }

                location = promptResult.Text;

                List<VehicleTable> listOfVehicles = dbService.GetVehicles();

                var vehicleResult = await UserDialogs.Instance.ActionSheetAsync(Resource.ChooseVehicle, Resource.Cancel, Resource.AddVehicleText, null, listOfVehicles.Select(l => l.Registration).ToArray());
                using (UserDialogs.Instance.Loading(Resource.AddingVehicle, null, null, true, MaskType.Gradient))
                {
                    if (vehicleResult == Resource.AddVehicleText)
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

                UserDialogs.Instance.ShowSuccess(Resource.DriveStarted, 1500);
                SetVehicleLabel();
                GeoCollection();
                return true;
            }

            return false;
        }

        private async Task<string> NotePromptAsync()
        {
            if (await UserDialogs.Instance.ConfirmAsync(Resource.AddNoteQuery, Resource.Alert, Resource.IWould, Resource.No))
            {
                PromptConfig notePrompt = new PromptConfig()
                {
                    CancelText = Resource.Cancel,
                    IsCancellable = true,
                    OkText = Resource.Okay,
                    Message = Resource.Note
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
            if (await UserDialogs.Instance.ConfirmAsync(Resource.EndDriveQuery, Resource.Confirmation, Resource.Yes, Resource.No))
            {
                Geolocation geocords;
                string location;

                using (UserDialogs.Instance.Loading(Resource.GetCoordinates, null, null, true, MaskType.Gradient))
                {
                    geocords = await restApi.GetLatAndLong().ConfigureAwait(false);
                    location = await GetLocation(geocords);
                }

                PromptConfig locationPrompt = new PromptConfig()
                {
                    IsCancellable = true,
                    Title = Resource.CurrentLocation,
                    Text = location
                };
                PromptResult promptResult = await UserDialogs.Instance.PromptAsync(locationPrompt);

                if (!promptResult.Ok || promptResult.Text == string.Empty)
                {
                    return false;
                }

                location = promptResult.Text;

                int hubo = await HuboPrompt();

                if (hubo == 0)
                {
                    return false;
                }

                string note = await NotePromptAsync();

                if (await dbService.StopDrive(hubo, note, location))
                {
                    UserDialogs.Instance.ShowSuccess(Resource.DriveEnd, 1500);
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
            string promptTitle = Resource.OdometerReading;
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

                promptTitle = Resource.InvalidOdometer;
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
            if (await UserDialogs.Instance.ConfirmAsync(Resource.StartBreakQuery, Resource.Alert, Resource.Yes, Resource.No))
            {
                Geolocation geoCoords;
                string location;

                using (UserDialogs.Instance.Loading(Resource.GetCoordinates, null, null, true, MaskType.Gradient))
                {
                    geoCoords = await restApi.GetLatAndLong().ConfigureAwait(false);
                    location = await GetLocation(geoCoords);
                }

                PromptConfig locationPrompt = new PromptConfig()
                {
                    IsCancellable = true,
                    Title = Resource.CurrentLocation,
                    Text = location
                };
                PromptResult promptResult = await UserDialogs.Instance.PromptAsync(locationPrompt);

                if (!promptResult.Ok || promptResult.Text == string.Empty)
                {
                    return;
                }

                string note = await NotePrompt();

                if (await dbService.StartBreak(location, note))
                {
                    BreakButtonColor = Constants.RED_COLOR;
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

                    UserDialogs.Instance.ShowSuccess(Resource.BreakStart, 1500);
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
                if (!await UserDialogs.Instance.ConfirmAsync("You have only had a " + remainTimeString + " minute break, this will not count as a full 30 min break, continue anyway?", Resource.Warning, Resource.Yes, Resource.No))
                {
                    return;
                }
            }
            else
            {
                if (!await UserDialogs.Instance.ConfirmAsync(Resource.EndBreakQuery, Resource.Alert, Resource.Yes, Resource.No))
                {
                    return;
                }
            }

            Geolocation geoCoords;
            string location;

            using (UserDialogs.Instance.Loading(Resource.GetCoordinates, null, null, true, MaskType.Gradient))
            {
                geoCoords = await restApi.GetLatAndLong().ConfigureAwait(false);
                location = await GetLocation(geoCoords);
            }

            PromptConfig locationPrompt = new PromptConfig()
            {
                IsCancellable = true,
                Title = Resource.CurrentLocation,
                Text = location
            };
            PromptResult promptResult = await UserDialogs.Instance.PromptAsync(locationPrompt);

            if (!promptResult.Ok || promptResult.Text == string.Empty)
            {
                return;
            }

            string note = await NotePrompt();

            if (await dbService.StopBreak(location, note))
            {
                if (FullBreak)
                {
                    NextBreakTime = Resource.TakeBreakBy + dbService.GetNextBreakTime();

                    dbService.CancelNotification(NotificationCategory.Break, true);

                    dbService.CreateNotification(Resource.Notify1Break, true, NotificationCategory.Break, TimeSpan.FromHours(4.5));

                    Device.StartTimer(TimeSpan.FromHours(4.5), () =>
                    {
                        if (!dbService.CheckTimedNotification(NotificationCategory.Break, DateTime.Now))
                        {
                            return false;
                        }

                        DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyApproachingBreak, Resource.Notify1Break, true);

                        dbService.CancelNotification(NotificationCategory.Break, true, true);
                        dbService.CreateNotification(Resource.Notify10Break, true, NotificationCategory.Break, TimeSpan.FromMinutes(50));

                        Device.StartTimer(TimeSpan.FromMinutes(50), () =>
                        {
                            if (!dbService.CheckTimedNotification(NotificationCategory.Break, DateTime.Now))
                            {
                                return false;
                            }

                            DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyApproachingBreak, Resource.Notify10Break, true);

                            dbService.CancelNotification(NotificationCategory.Break, true, true);
                            dbService.CreateNotification(Resource.NotifyBreakTime, true, NotificationCategory.Break, TimeSpan.FromMinutes(10));

                            Device.StartTimer(TimeSpan.FromMinutes(10), () =>
                            {
                                if (!dbService.CheckTimedNotification(NotificationCategory.Break, DateTime.Now))
                                {
                                    return false;
                                }

                                DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyBreakTimeTitle, Resource.NotifyBreakTime, true);

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
                    UserDialogs.Instance.ShowSuccess(Resource.BreakEnd, 1500);
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
            return location;
        }

        private async Task<bool> StopShift()
        {
            if (!dbService.VehicleActive())
            {
                if (dbService.CheckOnBreak() == 1)
                {
                    if (await UserDialogs.Instance.ConfirmAsync(Resource.EndShiftQuery, Resource.Confirmation, Resource.Yes, Resource.No))
                    {
                        Geolocation geoCoords;
                        string location;

                        using (UserDialogs.Instance.Loading(Resource.GetCoordinates, null, null, true, MaskType.Gradient))
                        {
                            geoCoords = await restApi.GetLatAndLong().ConfigureAwait(false);
                            location = await GetLocation(geoCoords);
                        }

                        PromptConfig locationPrompt = new PromptConfig()
                        {
                            IsCancellable = true,
                            Title = Resource.CurrentLocation,
                            Text = location
                        };
                        PromptResult promptResult = await UserDialogs.Instance.PromptAsync(locationPrompt);

                        if (!promptResult.Ok || promptResult.Text == string.Empty)
                        {
                            return false;
                        }

                        location = promptResult.Text;

                        string note = await NotePrompt();

                        if (await dbService.StopShift(location, note, geoCoords))
                        {
                            ShiftStarted = false;
                            ShowStartShiftXAML();
                            UserDialogs.Instance.ShowSuccess(Resource.ShiftEnd, 1500);
                            //MessagingCenter.Send<string>("ShiftEdited", "ShiftEdited");

                            dbService.CancelNotification(NotificationCategory.Ongoing, false);
                            dbService.CancelNotification(NotificationCategory.Shift, true);
                            dbService.CancelNotification(NotificationCategory.Break, true);

                            dbService.CreateNotification(Resource.NotifyInactive, false, NotificationCategory.Ongoing);

                            DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyInactiveTitle, Resource.NotifyInactive, false);
                            await Navigation.PushModalAsync(new EndShiftConfirmPage());

                            return true;
                        }

                        return false;
                    }
                }
                else
                {
                    await UserDialogs.Instance.AlertAsync(Resource.EndShiftBreakError, Resource.Error, Resource.GotIt);
                }
            }
            else
            {
                await UserDialogs.Instance.AlertAsync(Resource.EndShiftDriveError, Resource.Error, Resource.GotIt);
            }

            return false;
        }

        private async Task<bool> StartShift()
        {
            if (CrossBattery.Current.Status == Plugin.Battery.Abstractions.BatteryStatus.Unknown)
            {
                await UserDialogs.Instance.AlertAsync(Resource.BatteryStatusError, Resource.Alert, Resource.Okay);
            }

            if (CrossBattery.Current.RemainingChargePercent <= 50 && (CrossBattery.Current.Status == Plugin.Battery.Abstractions.BatteryStatus.Discharging || CrossBattery.Current.Status == Plugin.Battery.Abstractions.BatteryStatus.Unknown))
            {
                await UserDialogs.Instance.AlertAsync("Battery at " + CrossBattery.Current.RemainingChargePercent + "%, Please ensure the device is charged soon!", Resource.Alert, Resource.Okay);
            }

            if (await UserDialogs.Instance.ConfirmAsync(Resource.ShiftStartQuery, Resource.Confirmation, Resource.Yes, Resource.No))
            {
                if (!dbService.CheckTenHourBreak())
                {
                    await UserDialogs.Instance.AlertAsync(Resource.ShortBreakBetweenShifts, Resource.Alert, Resource.Okay);
                }

                List<QuestionModel> checklistQuestions = dbService.GetChecklistHealthSafety();
                int count = 0;
                foreach (QuestionModel question in checklistQuestions)
                {
                    count++;
                    bool result = await UserDialogs.Instance.ConfirmAsync(question.Question, Resource.ChecklistQuestionNumber + count.ToString(), Resource.Yes, Resource.No);

                    if (question.YesCorrect ? !result : result)
                    {
                        return false;
                    }
                }

                Geolocation geoCoords;
                string location;

                using (UserDialogs.Instance.Loading(Resource.GetCoordinates, null, null, true, MaskType.Gradient))
                {
                    geoCoords = await restApi.GetLatAndLong().ConfigureAwait(false);
                    location = await GetLocation(geoCoords);
                }

                PromptConfig locationPrompt = new PromptConfig()
                {
                    IsCancellable = true,
                    Title = Resource.CurrentLocation,
                    Text = location
                };
                PromptResult promptResult = await UserDialogs.Instance.PromptAsync(locationPrompt);

                if (!promptResult.Ok || promptResult.Text == string.Empty)
                {
                    return false;
                }

                location = promptResult.Text;

                string note = await NotePrompt();

                if (await dbService.StartShift(location, note, geoCoords))
                {
                    ShiftStarted = true;
                    ShowEndShiftXAML();
                    UpdateCircularGauge();

                    dbService.CancelNotification(NotificationCategory.Ongoing, false);

                    dbService.CreateNotification(Resource.NotifyOnShift, false, NotificationCategory.Ongoing);
                    DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyShiftRunningTitle, Resource.NotifyOnShift, false);

                    dbService.CreateNotification(Resource.Notify1Break, true, NotificationCategory.Break, TimeSpan.FromHours(4.5));

                    Device.StartTimer(TimeSpan.FromHours(4.5), () =>
                    {
                        if (!dbService.CheckTimedNotification(NotificationCategory.Break, DateTime.Now))
                        {
                            return false;
                        }

                        DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyApproachingBreak, Resource.Notify1Break, true);

                        dbService.CancelNotification(NotificationCategory.Break, true, true);
                        dbService.CreateNotification(Resource.Notify10Break, true, NotificationCategory.Break, TimeSpan.FromMinutes(50));

                        Device.StartTimer(TimeSpan.FromMinutes(50), () =>
                        {
                            if (!dbService.CheckTimedNotification(NotificationCategory.Break, DateTime.Now))
                            {
                                return false;
                            }

                            DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyApproachingBreak, Resource.Notify10Break, true);

                            dbService.CancelNotification(NotificationCategory.Break, true, true);
                            dbService.CreateNotification(Resource.NotifyBreakTime, true, NotificationCategory.Break, TimeSpan.FromMinutes(10));

                            Device.StartTimer(TimeSpan.FromMinutes(10), () =>
                            {
                                if (!dbService.CheckTimedNotification(NotificationCategory.Break, DateTime.Now))
                                {
                                    return false;
                                }

                                DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyBreakTimeTitle, Resource.NotifyBreakTime, true);

                                dbService.CancelNotification(NotificationCategory.Break, true, true);
                                return false;
                            });
                            return false;
                        });
                        return false;
                    });

                    await Application.Locator.StopListeningAsync();

                    dbService.CreateNotification(Resource.Notify1Shift, true, NotificationCategory.Shift, TimeSpan.FromHours(13));
                    Device.StartTimer(TimeSpan.FromHours(13), () =>
                    {
                        if (dbService.CheckTimedNotification(NotificationCategory.Shift, DateTime.Now))
                        {
                            return false;
                        }

                        DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyEndingShiftTitle, Resource.Notify1Shift, true);

                        dbService.CancelNotification(NotificationCategory.Shift, true, true);
                        dbService.CreateNotification(Resource.Notify10Shift, true, NotificationCategory.Shift, TimeSpan.FromMinutes(50));

                        Device.StartTimer(TimeSpan.FromMinutes(50), () =>
                        {
                            if (dbService.CheckTimedNotification(NotificationCategory.Shift, DateTime.Now))
                            {
                                return false;
                            }

                            DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyEndingShiftTitle, Resource.Notify10Shift, true);

                            dbService.CancelNotification(NotificationCategory.Shift, true, true);
                            dbService.CreateNotification(Resource.NotifyEndShift, true, NotificationCategory.Shift, TimeSpan.FromMinutes(10));

                            Device.StartTimer(TimeSpan.FromMinutes(10), () =>
                            {
                                if (dbService.CheckTimedNotification(NotificationCategory.Shift, DateTime.Now))
                                {
                                    return false;
                                }

                                DependencyService.Get<INotifyService>().UpdateNotification(Resource.NotifyEndShiftTitle, Resource.NotifyEndShift, true);

                                dbService.CancelNotification(NotificationCategory.Shift, true, true);
                                return false;
                            });
                            return false;
                        });
                        return false;
                    });
                    UserDialogs.Instance.ShowSuccess(Resource.ShiftStart, 1500);

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
                UserDialogs.Instance.AlertAsync(Resource.MoreOneActiveShift, Resource.Alert, Resource.Okay);
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
            if (await UserDialogs.Instance.ConfirmAsync(Resource.AddNoteQuery, Resource.Confirmation, Resource.IWould, Resource.No))
            {
                PromptConfig notePrompt = new PromptConfig()
                {
                    IsCancellable = true,
                    Title = Resource.NoteText
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