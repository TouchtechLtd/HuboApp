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
        public Color ShiftButtonColor { get; set; }
        public Color BreakButtonColor { get; set; }
        public Color VehicleTextColor { get; set; }
        public double CompletedJourney { get; set; }
        public int RemainderOfJourney { get; set; }
        public int Break { get; set; }
        public double TotalBeforeBreak { get; set; }
        public string TotalBeforeBreakText { get; set; }
        public int StartValue { get; set; }
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
        public string LoadingText { get; set; }

        public ICommand StartBreakCommand { get; set; }

        //public ICommand EndShiftCommand { get; set; }
        public ICommand VehicleCommand { get; set; }
        //public ICommand AddNoteCommand { get; set; }
        public bool OnBreak { get; set; }

        public Color MinorTickColor { get; set; }
        public string UseOrStopVehicleText { get; set; }

        DatabaseService DbService = new DatabaseService();

        private bool _isBusy;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        private bool _shiftRunning;
        public bool ShiftRunning
        {
            get
            {
                return _shiftRunning;
            }
            set
            {
                _shiftRunning = value;
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
                TotalBeforeBreak = DbService.TotalBeforeBreak();
            }
            else if (hoursTillReset == -2)
            {
                HoursTillReset = Resource.FullyRested;
                TotalBeforeBreak = 0;
            }
            else
            {
                HoursTillReset = hoursTillReset.ToString() + Resource.LastShiftEndText;
                TotalBeforeBreak = DbService.TotalBeforeBreak();
            }


            TotalBeforeBreakText = ((int)TotalBeforeBreak).ToString() + Resource.HoursTotalText;

            CheckActiveShift();
            CheckActiveBreak();
            ShiftButton = new Command(ToggleShift);
            //StartBreakText = Resource.StartBreak;
            EndShiftText = Resource.EndShift;

            //TODO: Code to check if vehicle in use
            if (DbService.VehicleInUse())
            {
                VehicleText = Resource.StopDriving;
            }
            else
            {
                VehicleText = Resource.StartDriving;
            }

            //OnBreak = DbService.CheckOnBreak();

            AddNoteText = Resource.AddNote;
            //BreakButtonColor = Color.FromHex("#009900");
            StartBreakCommand = new Command(StartBreak);
            VehicleCommand = new Command(Vehicle);
            //AddNoteCommand = new Command(AddNote);
            SetVehicleLabel();
            MessagingCenter.Subscribe<string>("UpdateActiveVehicle", "UpdateActiveVehicle", (sender) =>
            {
                SetVehicleLabel();
            });

            UpdateCircularGauge();

            var minutes = TimeSpan.FromMinutes(10);

            Device.StartTimer(minutes, () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    UpdateCircularGauge();
                });
                return true;
            });
            IsBusy = false;

            if (DbService.CheckActiveShift())
            {
                ShiftRunning = true;
                GeoCollection();
            }
            else
            {
                ShiftRunning = false;
            }
        }

        private void SetVehicleLabel()
        {
            if (DbService.VehicleActive())
            {
                //VehicleTable currentVehicle = new VehicleTable();
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
                VehiclePickerText = currentVehicle.Registration;
                VehicleText = Resource.StopDriving;
                VehicleTextColor = Color.FromHex("#cc0000");
                PickerEnabled = false;
                VehicleInUse = true;

            }
            else
            {
                //VehicleRego = -1;
                VehiclePickerText = Resource.NoActiveVehicle;
                VehicleText = Resource.StartDriving;
                VehicleTextColor = Color.FromHex("#009900");
                PickerEnabled = true;
                VehicleInUse = false;
            }

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
            if (DbService.CheckOnBreak())
            {
                BreakButtonColor = Color.FromHex("#cc0000");
                StartBreakText = Resource.EndBreak;
                OnBreak = true;
            }
            else
            {
                BreakButtonColor = Color.FromHex("#009900");
                StartBreakText = Resource.StartBreak;
                OnBreak = false;
            }
        }

        private void AddNote()
        {
            Navigation.PushAsync(new AddNotePage(1));
        }

        private void Vehicle()
        {
            //Navigation.PushAsync(new VehiclesPage(2));
            if (currentVehicle.Registration != null)
            {
                if (VehicleInUse)
                {
                    // Code to switch used vehicle off. 1) change visual elements, 2) code to toggle active off, 3)Code to open new page to input rego information
                    Navigation.PushAsync(new AddNotePage(4, currentVehicle.Key));
                }
                else
                {
                    //Code to switch vehicle on Reverse of previous TODO
                    Navigation.PushAsync(new AddNotePage(4, currentVehicle.Key));
                }
                SetVehicleLabel();
            }
            else
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "PLEASE SELECT A VEHICLE", Resource.DisplayAlertOkay);
            }
        }

        private void StartBreak()
        {
            Navigation.PushModalAsync(new AddNotePage(2));

            if (OnBreak)
            {

                MessagingCenter.Subscribe<string>("AddBreak", "AddBreak", (sender) =>
                {
                    //Add break was successful
                    if (sender == "Success")
                    {
                        BreakButtonColor = Color.FromHex("#009900");
                        StartBreakText = Resource.StartBreak;
                        OnBreak = false;
                        //DbService.StopBreak();
                        OnPropertyChanged("BreakButtonColor");
                        OnPropertyChanged("StartBreakText");
                        OnPropertyChanged("OnBreak");
                    }
                    MessagingCenter.Unsubscribe<string>("AddBreak", "AddBreak");
                });


            }
            else
            {

                //TODO: Implement ability to take note and hubo
                MessagingCenter.Subscribe<string>("AddBreak", "AddBreak", (sender) =>
                {
                    //Add break was successful
                    if (sender == "Success")
                    {
                        BreakButtonColor = Color.FromHex("#cc0000");
                        StartBreakText = Resource.EndBreak;
                        OnBreak = true;
                        //DbService.StartBreak();
                        OnPropertyChanged("BreakButtonColor");
                        OnPropertyChanged("StartBreakText");
                        OnPropertyChanged("OnBreak");
                    }
                    MessagingCenter.Unsubscribe<string>("AddBreak", "AddBreak");
                });

            }
            OnPropertyChanged("BreakButtonColor");
            OnPropertyChanged("StartBreakText");
        }

        private async void ToggleShift()
        {
            if (!ShiftStarted)
            {
                if (await StartShift())
                {
                    MessagingCenter.Subscribe<string>("ShiftStart", "ShiftStart", (sender) =>
                    {
                        if (sender == "Success")
                        {
                            IsBusy = false;
                            ShowEndShiftXAML();
                        }
                        MessagingCenter.Unsubscribe<string>("ShiftStart", "ShiftStart");
                    });
                    await Navigation.PushModalAsync(new AddNotePage(5));
                }
            }
            else
            {
                if (DbService.NoBreaksActive())
                {
                    if (!DbService.VehicleActive())
                    {
                        MessagingCenter.Subscribe<string>("ShiftEnd", "ShiftEnd", async (sender) =>
                        {
                            if (sender == "Success")
                            {
                                await Navigation.PushModalAsync(new NZTAMessagePage(2));
                                IsBusy = false;
                                ShowStartShiftXAML();
                            }
                            MessagingCenter.Unsubscribe<string>("ShiftEnd", "ShiftEnd");
                        });
                        await Navigation.PushModalAsync(new AddNotePage(6));
                    }
                    else
                    {
                        await Navigation.PushModalAsync(new VehicleChecklistPage(3, false));
                        MessagingCenter.Subscribe<string>("EndShiftRegoEntered", "EndShiftRegoEntered", async (sender) =>
                        {
                            if (sender == "Success")
                            {
                                MessagingCenter.Subscribe<string>("ShiftEnd", "ShiftEnd", async (send) =>
                                {
                                    if (sender == "Success")
                                    {
                                        await Navigation.PushModalAsync(new NZTAMessagePage(2));
                                        IsBusy = false;
                                        ShowStartShiftXAML();
                                    }
                                    MessagingCenter.Unsubscribe<string>("ShiftEnd", "ShiftEnd");
                                });
                                await Navigation.PushModalAsync(new AddNotePage(6));
                            }
                            MessagingCenter.Unsubscribe<string>("EndShiftRegoEntered", "EndShiftRegoEntered");
                        });
                    }
                }

            }
            OnPropertyChanged("StartShiftVisibility");
            OnPropertyChanged("ShiftStarted");
            OnPropertyChanged("ShiftText");
            OnPropertyChanged("ShiftButtonColor");
        }

        private void ShowEndShiftXAML()
        {
            ShiftText = "End Shift";
            ShiftButtonColor = Color.FromHex("#cc0000");
            StartShiftVisibility = false;
            ShiftStarted = true;
            ShiftRunning = true;
            //LoadingText = "Stopping Shift...";
        }

        private void ShowStartShiftXAML()
        {
            ShiftText = "Start Shift";
            ShiftButtonColor = Color.FromHex("#009900");
            StartShiftVisibility = true;
            ShiftStarted = false;
            ShiftRunning = false;
            //LoadingText = "Starting Shift...";
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

            IsBusy = true;
            SetLoadingText();

            return true;
        }

        private void UpdateCircularGauge()
        {
            if (DbService.CheckActiveShift())
            {
                TotalBeforeBreak = DbService.TotalBeforeBreak();

                CompletedJourney = TotalBeforeBreak;
                TotalBeforeBreakText = ((int)TotalBeforeBreak).ToString() + Resource.HoursTotalText;

                //if (this.CompletedJourney >= 13)
                //    DependencyService.Get<INotifyService>().LocalNotification("Shift Limit Reached!", "You have reached the maximum allowed shift time for today.", DateTime.Now);

                OnPropertyChanged("CompletedJourney");
                OnPropertyChanged("TotalBeforeBreakText");
            }

        }

        public List<VehicleTable> GetVehicles()
        {
            listOfVehicles = new List<VehicleTable>();
            listOfVehicles = DbService.GetVehicles();
            return listOfVehicles;
        }

        private void SetLoadingText()
        {
            List<LoadTextTable> loadText = new List<LoadTextTable>();
            loadText = DbService.GetLoadingText();

            Random random = new Random();

            int id = random.Next(1, loadText.Count);

            LoadingText = loadText[id - 1].LoadText;

            var sec = TimeSpan.FromSeconds(5);

            Device.StartTimer(sec, () =>
            {
                id = random.Next(1, loadText.Count);

                LoadingText = loadText[id - 1].LoadText;

                return IsBusy;
            });
        }

        private void GeoCollection()
        {
            var min = TimeSpan.FromMinutes(1);

            int shiftKey = DbService.GetCurrentShift().Key;

            DbService.CollectGeolocation(shiftKey);

            Device.StartTimer(min, () =>
            {
                DbService.CollectGeolocation(shiftKey);
                return ShiftRunning;
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
