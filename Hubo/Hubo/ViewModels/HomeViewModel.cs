using Acr.UserDialogs;
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

        private string _loadingText;
        public string LoadingText
        {
            get { return _loadingText; }
            set
            {
                _loadingText = value;
                OnPropertyChanged("LoadingText");
            }
        }

        public FileImageSource ShiftImage { get; set; }

        public ICommand StartBreakCommand { get; set; }

        //public ICommand EndShiftCommand { get; set; }
        public ICommand VehicleCommand { get; set; }
        //public ICommand AddNoteCommand { get; set; }
        public bool OnBreak { get; set; }

        public Color MinorTickColor { get; set; }
        public string UseOrStopVehicleText { get; set; }

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
            EndShiftText = Resource.EndShift;

            //Code to check if vehicle in use
            if (DbService.CheckActiveDriveShift())
            {
                VehicleText = Resource.StopDriving;
                DriveShiftRunning = true;
            }
            else
            {
                VehicleText = Resource.StartDriving;
                DriveShiftRunning = false;
            }

            AddNoteText = Resource.AddNote;
            StartBreakCommand = new Command(ToggleBreak);
            VehicleCommand = new Command(ToggleDrive);
            SetVehicleLabel();

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

        }

        private void SetVehicleLabel()
        {
            if (DbService.CheckActiveDriveShift())
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
                VehiclePickerText = currentVehicle.Registration;
                VehicleText = Resource.StopDriving;
                VehicleTextColor = Color.FromHex("#cc0000");
                PickerEnabled = false;
                VehicleInUse = true;

            }
            else
            {
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

        public async void ToggleDrive()
        {
            if (await DbService.SaveDrive(DateTime.Now, currentVehicle.Key))
            {
                SetVehicleLabel();
            }
        }

        private async void ToggleBreak()
        {
            if (OnBreak)
            {
                if(await DbService.StopBreak())
                {
                    BreakButtonColor = Color.FromHex("#009900");
                    StartBreakText = Resource.StartBreak;
                    OnBreak = false;
                    DriveShiftRunning = true;
                }                
            }
            else
            {
                if(await DbService.StartBreak())
                {
                    BreakButtonColor = Color.FromHex("#cc0000");
                    StartBreakText = Resource.EndBreak;
                    OnBreak = true;
                    DriveShiftRunning = false;
                }
            }
            OnPropertyChanged("BreakButtonColor");
            OnPropertyChanged("StartBreakText");
            OnPropertyChanged("OnBreak");
        }

        private async void ToggleShift()
        {
            if (!ShiftStarted)
            {
                if (await StartShift())
                {
                    if(await DbService.StartShift())
                    {
                        ShowEndShiftXAML();
                    }
                    
                }
            }
            else
            {
                if (DbService.CheckOnBreak())
                {
                    await Application.Current.MainPage.DisplayAlert("ERROR", "Please end your break before ending your work shift", "Gotcha");
                }
                else
                {
                    if (DbService.CheckActiveDriveShift())
                    {
                        await Application.Current.MainPage.DisplayAlert("ERROR", "Pleas end your driving shift before ending your work shift", "Gotcha");
                    }
                    else
                    {
                        if (await DbService.StopShift(DateTime.Now))
                        {
                            ShowStartShiftXAML();
                            await Navigation.PushModalAsync(new NZTAMessagePage(2));
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("ERROR", "There was a problem ending your shift", "Understood");
                        }
                    }
                }
            }
        }

        private void ShowEndShiftXAML()
        {
            ShiftText = "End Shift";
            ShiftButtonColor = Color.FromHex("#cc0000");
            StartShiftVisibility = false;
            ShiftStarted = true;
            ShiftImage = "Stop.png";
            OnPropertyChanged("ShiftImage");
            OnPropertyChanged("ShiftText");
            OnPropertyChanged("ShiftButtonColor");
            OnPropertyChanged("StartShiftVisibility");
            OnPropertyChanged("ShiftStarted");
        }

        private void ShowStartShiftXAML()
        {
            ShiftText = "Start Shift";
            ShiftButtonColor = Color.FromHex("#009900");
            StartShiftVisibility = true;
            ShiftStarted = false;
            DriveShiftRunning = false;
            ShiftImage = "Play.png";
            OnPropertyChanged("ShiftImage");
            OnPropertyChanged("StartShiftVisibility");
            OnPropertyChanged("ShiftStarted");
            OnPropertyChanged("ShiftText");
            OnPropertyChanged("ShiftButtonColor");
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
            if (DbService.CheckActiveDriveShift())
            {
                TotalBeforeBreak = DbService.TotalBeforeBreak();

                CompletedJourney = TotalBeforeBreak;
                TotalBeforeBreakText = ((int)TotalBeforeBreak).ToString() + Resource.HoursTotalText;                

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
