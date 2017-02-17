using System;
using System.Collections.Generic;
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
        public ICommand BreaksCommand { get; set; }
        public ICommand NotesCommand { get; set; }
        public ICommand VehiclesCommand { get; set; }
        public string SaveText { get; set; }
        public string EditBreaksText { get; set; }
        public string EditNotesText { get; set; }
        public string EditVehiclesText { get; set; }
        public bool ChangesMade { get; set; }
        public bool ShiftStartInfoVisible { get; set; }
        public bool ShiftEndInfoVisible { get; set; }
        public bool ShiftSelected { get; set; }
        public string DashText { get; set; }

        DatabaseService DbService = new DatabaseService();

        ShiftTable currentShift = new ShiftTable();

        public List<DriveTable> Drives { get; set; }

        List<AmendmentTable> listOfAmendments = new List<AmendmentTable>();

        public EditShiftViewModel()
        {
            //ShiftEndTime = Resource.ShiftEndTime;
            DashText = Resource.Dash;
            ShiftStartTime = Resource.ShiftStartTime;
            ShiftEndTime = Resource.ShiftEndTime;
            ShiftStartInfoVisible = false;
            ShiftEndInfoVisible = false;
            SaveCommand = new Command(Save);
            VehiclesCommand = new Command(EditShiftDetails);
            NotesCommand = new Command(EditShiftDetails);
            BreaksCommand = new Command(EditShiftDetails);
            SaveText = Resource.Save;
            EditBreaksText = Resource.EditBreaks;
            EditNotesText = Resource.EditNotes;
            EditVehiclesText = Resource.EditVehiclesUsed;
            ChangesMade = false;
            ShiftSelected = false;
        }

        private void EditShiftDetails(object obj)
        {
            if (currentShift.Key != 0)
            {
                Navigation.PushAsync(new EditShiftDetailsPage(obj.ToString(), currentShift));
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
                AmendmentTable newStartShift = new AmendmentTable();;
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
                ShiftEndDatePicker = DateTime.Parse(shiftTable.EndDate);
                ShiftEndTimePicker = ShiftEndDatePicker.TimeOfDay;
            }

            currentShift = shiftTable;

            ShiftStartDatePicker = DateTime.Parse(shiftTable.StartDate);

            ShiftStartTimePicker = ShiftStartDatePicker.TimeOfDay;

            Drives = DbService.GetDriveShifts(currentShift.Key);

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