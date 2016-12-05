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
        List<VehicleInUseTable> listOfVehiclesUsed = new List<VehicleInUseTable>();

        List<AmendmentTable> listOfAmendments = new List<AmendmentTable>();

        public EditShiftViewModel()
        {
            ShiftEndTime = Resource.ShiftEndTime;
            ShiftStartTime = Resource.ShiftStartTime;
            ShiftStartInfoVisible = false;
            ShiftEndInfoVisible = false;
            SaveCommand = new Command(Save);
            VehiclesCommand = new Command(Vehicles);
            NotesCommand = new Command(Notes);
            BreaksCommand = new Command(Break);
            SaveText = Resource.Save;
            EditBreaksText = Resource.EditBreaks;
            EditNotesText = Resource.EditNotes;
            EditVehiclesText = Resource.EditVehiclesUsed;
            ChangesMade = false;
            ShiftSelected = false;
        }

        private void Break(object obj)
        {
            if (currentShift.Key != 0)
            {
                Navigation.PushAsync(new EditShiftDetailsPage(1, currentShift));
            }
            else
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.SelectAShift, Resource.DisplayAlertOkay);
            }
        }

        private void Notes(object obj)
        {
            if (currentShift.Key != 0)
            {
                Navigation.PushAsync(new EditShiftDetailsPage(2, currentShift));
            }
            else
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.SelectAShift, Resource.DisplayAlertOkay);
            }
        }

        private void Vehicles(object obj)
        {
            if(currentShift.Key!=0)
            {
                Navigation.PushAsync(new EditShiftDetailsPage(3, currentShift));
            }
            else
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.SelectAShift, Resource.DisplayAlertOkay);
            }
        }

        private void Save()
        {
            string newStartShiftDate = ShiftStartDatePicker.ToString();
            if(!(currentShift.EndTime=="Current"))
            {

                DateTime oldEndShiftDate = DateTime.Parse(currentShift.EndTime).Date;
                TimeSpan oldEndShiftTime = DateTime.Parse(currentShift.EndTime).TimeOfDay;

                if ((ShiftEndDatePicker != oldEndShiftDate) || (ShiftEndTimePicker != oldEndShiftTime))
                {
                    AmendmentTable newStopShift = new AmendmentTable();
                    newStopShift.BeforeValue = oldEndShiftDate.Date.ToString("dd/MM/yyyy") + " " + oldEndShiftTime;
                    newStopShift.Field = "EndTime";
                    newStopShift.ShiftId = currentShift.Key;
                    newStopShift.Table = "ShiftTable";
                    newStopShift.TimeStamp = DateTime.Now.ToString();
                    listOfAmendments.Add(newStopShift);
                    currentShift.EndTime = ShiftEndDatePicker.Date.ToString("dd/MM/yyyy") + " " + ShiftEndTimePicker;
                }
            }
            else
            {
                currentShift.EndTime = null;
            }

            DateTime oldStartShiftDate = DateTime.Parse(currentShift.StartTime).Date;
            TimeSpan oldStartShiftTime = DateTime.Parse(currentShift.StartTime).TimeOfDay;

            if ((ShiftStartDatePicker!=oldStartShiftDate)||(oldStartShiftTime!=ShiftStartTimePicker))
            {
                AmendmentTable newStartShift = new AmendmentTable();
                newStartShift.BeforeValue = oldStartShiftDate.Date.ToString("dd/MM/yyyy") + " " + oldStartShiftTime;
                newStartShift.Field = "StartTime";
                newStartShift.ShiftId = currentShift.Key;
                newStartShift.Table = "ShiftTable";
                newStartShift.TimeStamp = DateTime.Now.ToString();
                listOfAmendments.Add(newStartShift);
                currentShift.StartTime = ShiftStartDatePicker.Date.ToString("dd/MM/yyyy") + " " + ShiftStartTimePicker;
            }

            if (listOfAmendments.Count>0)
            {
                DbService.AddAmendments(listOfAmendments, currentShift);
            }
        }

        public List<ShiftTable> Load(DateTime selectedDate)
        {
            listOfShifts =  DbService.GetShifts(selectedDate);
            if(listOfShifts.Count==0)
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.NoShiftsFound, Resource.DisplayAlertOkay);
                
            }
            return listOfShifts;
        }

        internal void LoadInfoFromShift(ShiftTable shiftTable)
        {
            ShiftStartInfoVisible = true;
            if(!(shiftTable.EndTime=="Current"))
            {
                ShiftEndInfoVisible = true;
                ShiftEndDatePicker = DateTime.Parse(shiftTable.EndTime);
                ShiftEndTimePicker = ShiftEndDatePicker.TimeOfDay;
                OnPropertyChanged("ShiftEndInfoVisible");
            }

            currentShift = shiftTable;

            ShiftStartDatePicker = DateTime.Parse(shiftTable.StartTime);

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
