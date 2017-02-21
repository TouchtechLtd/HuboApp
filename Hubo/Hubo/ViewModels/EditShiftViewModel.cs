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
    class EditShiftViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }
        public string ShiftStartTime { get; set; }
        public string ShiftEndTime { get; set; }
        public TimeSpan ShiftStartTimePicker { get; set; }
        public TimeSpan ShiftEndTimePicker { get; set; }
        public DateTime ShiftStartDatePicker { get; set; }
        public DateTime ShiftEndDatePicker { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DrivesCommand { get; set; }
        public ICommand NotesCommand { get; set; }
        public string SaveText { get; set; }
        public string AddDrivesText { get; set; }
        public string EditNotesText { get; set; }
        public string CancelText { get; set; }
        public bool ChangesMade { get; set; }
        public bool ShiftStartInfoVisible { get; set; }
        public bool ShiftEndInfoVisible { get; set; }
        public bool ShiftSelected { get; set; }
        public string DashText { get; set; }
        public int SelectedDrive { get; set; }

        DatabaseService DbService = new DatabaseService();

        ShiftTable currentShift = new ShiftTable();
        List<DriveTable> driveList = new List<DriveTable>();

        public ObservableCollection<DriveTable> Drives { get; set; }

        List<AmendmentTable> listOfAmendments = new List<AmendmentTable>();

        public EditShiftViewModel()
        {
            DashText = Resource.Dash;
            ShiftStartTime = Resource.ShiftStartTime;
            ShiftEndTime = Resource.ShiftEndTime;
            ShiftStartInfoVisible = false;
            ShiftEndInfoVisible = false;
            SaveCommand = new Command(Save);
            NotesCommand = new Command(EditShiftDetails);
            DrivesCommand = new Command(EditShiftDetails);
            SaveText = Resource.Save;
            AddDrivesText = Resource.AddDrive;
            EditNotesText = Resource.EditNotes;
            CancelText = Resource.Cancel;
            ChangesMade = false;
            ShiftSelected = false;
            SelectedDrive = -1;
        }

        public void EditShiftDetails(object obj)
        {
            if (currentShift.Key != 0)
            {
                if (SelectedDrive > -1)
                {
                    Navigation.PushModalAsync(new EditShiftDetailsPage(obj.ToString(), Drives[SelectedDrive]));

                    driveList = DbService.GetDriveShifts(currentShift.Key);
                    Drives = new ObservableCollection<DriveTable>(driveList);

                    SelectedDrive = -1;

                    OnPropertyChanged("Drives");
                    OnPropertyChanged("SelectedDrive");
                }
                else if (SelectedDrive == -1)
                    Navigation.PushModalAsync(new EditShiftDetailsPage(obj.ToString(), null, currentShift));
            }
            else
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.SelectAShift, Resource.DisplayAlertOkay);
            }
        }

        private void Save()
        {
            if (!(currentShift.EndDate == "Current"))
            {

                DateTime oldEndShiftDate = DateTime.Parse(currentShift.EndDate).Date;
                TimeSpan oldEndShiftTime = DateTime.Parse(currentShift.EndDate).TimeOfDay;

                if ((ShiftEndDatePicker != oldEndShiftDate) || (ShiftEndTimePicker != oldEndShiftTime))
                {
                    AmendmentTable newStopShift = new AmendmentTable();
                    newStopShift.Field = "EndDate";
                    newStopShift.ShiftId = currentShift.Key;
                    newStopShift.Table = "ShiftTable";
                    newStopShift.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    newStopShift.BeforeValue = currentShift.EndDate;
                    newStopShift.AfterValue = (ShiftEndDatePicker + ShiftEndTimePicker).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    currentShift.EndDate = (ShiftEndDatePicker + ShiftEndTimePicker).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    listOfAmendments.Add(newStopShift);
                }
            }

            DateTime oldStartShiftDate = DateTime.Parse(currentShift.StartDate).Date;
            TimeSpan oldStartShiftTime = DateTime.Parse(currentShift.StartDate).TimeOfDay;

            if ((ShiftStartDatePicker != oldStartShiftDate) || (oldStartShiftTime != ShiftStartTimePicker))
            {
                AmendmentTable newStartShift = new AmendmentTable(); ;
                newStartShift.Field = "StartTime";
                newStartShift.ShiftId = currentShift.Key;
                newStartShift.Table = "ShiftTable";
                newStartShift.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                newStartShift.BeforeValue = currentShift.StartDate;
                newStartShift.AfterValue = (ShiftStartDatePicker + ShiftStartTimePicker).ToString("yyyy-MM-dd HH:mm:ss.fff");
                listOfAmendments.Add(newStartShift);
                currentShift.StartDate = (ShiftStartDatePicker + ShiftStartTimePicker).ToString("yyyy-MM-dd HH:mm:ss.fff");
            }

            if (listOfAmendments.Count > 0)
            {
                DbService.AddAmendments(listOfAmendments, currentShift);
            }

            MessagingCenter.Send<string>("ShiftEdited", "ShiftEdited");
        }

        internal void LoadInfoFromShift(ShiftTable shiftTable)
        {
            ShiftStartInfoVisible = true;
            if (!(shiftTable.EndDate == "Current"))
            {
                ShiftEndInfoVisible = true;
                ShiftEndDatePicker = DateTime.Parse(shiftTable.EndDate).Date;
                ShiftEndTimePicker = ShiftEndDatePicker.TimeOfDay;
            }

            currentShift = shiftTable;

            ShiftStartDatePicker = DateTime.Parse(shiftTable.StartDate).Date;

            ShiftStartTimePicker = ShiftStartDatePicker.TimeOfDay;

            driveList = DbService.GetDriveShifts(currentShift.Key);
            Drives = new ObservableCollection<DriveTable>(driveList);

            ShiftSelected = true;

            OnPropertyChanged("ShiftSelected");
            OnPropertyChanged("ShiftStartInfoVisible");
            OnPropertyChanged("ShiftEndInfoVisible");
            OnPropertyChanged("ShiftStartTimePicker");
            OnPropertyChanged("ShiftEndTimePicker");
            OnPropertyChanged("ShiftStartDatePicker");
            OnPropertyChanged("ShiftEndDatePicker");
            OnPropertyChanged("Drives");
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