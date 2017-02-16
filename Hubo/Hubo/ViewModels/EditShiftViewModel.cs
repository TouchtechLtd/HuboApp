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

        DatabaseService DbService = new DatabaseService();

        ShiftTable currentShift = new ShiftTable();

        List<ShiftTable> listOfShifts = new List<ShiftTable>();
        List<BreakTable> listOfBreaks = new List<BreakTable>();
        List<NoteTable> listOfNotes = new List<NoteTable>();
        List<DriveTable> listOfVehiclesUsed = new List<DriveTable>();

        List<AmendmentTable> listOfAmendments = new List<AmendmentTable>();

        public EditShiftViewModel()
        {
            //ShiftEndTime = Resource.ShiftEndTime;
            ShiftStartTime = Resource.Shift;
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
                    newStopShift.TimeStamp = DateTime.Now.ToString();
                    newStopShift.BeforeValue = currentShift.EndDate;
                    newStopShift.AfterValue = (ShiftEndDatePicker + ShiftEndTimePicker).ToString();
                    currentShift.EndDate = (ShiftEndDatePicker + ShiftEndTimePicker).ToString();
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
                newStartShift.TimeStamp = DateTime.Now.ToString();
                newStartShift.BeforeValue = currentShift.StartDate;
                newStartShift.AfterValue = (ShiftStartDatePicker + ShiftStartTimePicker).ToString();
                listOfAmendments.Add(newStartShift);
                currentShift.StartDate = (ShiftStartDatePicker + ShiftStartTimePicker).ToString();
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

            ShiftSelected = true;

            OnPropertyChanged("ShiftSelected");
            OnPropertyChanged("ShiftStartInfoVisible");
            OnPropertyChanged("ShiftEndInfoVisible");
            OnPropertyChanged("ShiftStartTimePicker");
            OnPropertyChanged("ShiftEndTimePicker");
            OnPropertyChanged("ShiftStartDatePicker");
            OnPropertyChanged("ShiftEndDatePicker");
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
