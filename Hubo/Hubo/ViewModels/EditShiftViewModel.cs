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



        public bool ShiftInfoVisible { get; set; }

        DatabaseService DbService = new DatabaseService();

        ShiftTable currentShift = new ShiftTable();

        List<ShiftTable> listOfShifts = new List<ShiftTable>();
        List<BreakTable> listOfBreaks = new List<BreakTable>();
        List<NoteTable> listOfNotes = new List<NoteTable>();
        List<VehicleInUseTable> listOfVehiclesUsed = new List<VehicleInUseTable>();

        List<AmendmentTable> listOfAmendments = new List<AmendmentTable>();

        public EditShiftViewModel()
        {
            ShiftEndTime = "Shift End Time";
            ShiftStartTime = "Shift Start Time";
            ShiftInfoVisible = false;
            SaveCommand = new Command(Save);
            VehiclesCommand = new Command(Vehicles);
            NotesCommand = new Command(Notes);
            BreaksCommand = new Command(Break);
            SaveText = Resource.Save;
            EditBreaksText = Resource.EditBreaks;
            EditNotesText = Resource.EditNotes;
            EditVehiclesText = Resource.EditVehiclesUsed;
            ChangesMade = false;
        }

        private void Break(object obj)
        {
            Navigation.PushAsync(new EditShiftDetailsPage(1, currentShift));
        }

        private void Notes(object obj)
        {
            Navigation.PushAsync(new EditShiftDetailsPage(2, currentShift));
        }

        private void Vehicles(object obj)
        {
            Navigation.PushAsync(new EditShiftDetailsPage(3, currentShift));
        }

        private void Save()
        {
            string newStartShiftDate = ShiftStartDatePicker.ToString();
            string newEndShiftDate = ShiftEndDatePicker.ToString();

            string oldStartShiftDate = DateTime.Parse(currentShift.TimeStart).ToString();
            string oldEndShiftDate = DateTime.Parse(currentShift.TimeEnd).ToString();

            newStartShiftDate = newStartShiftDate.Substring(0, newStartShiftDate.IndexOf(" ") + 1);
            newEndShiftDate = newEndShiftDate.Substring(0, newEndShiftDate.IndexOf(" ") + 1);

            oldStartShiftDate = oldStartShiftDate.Substring(0, oldStartShiftDate.IndexOf(" ") + 1);
            oldEndShiftDate = oldEndShiftDate.Substring(0, oldEndShiftDate.IndexOf(" ") + 1);


            string newStartShiftTime = ShiftStartTimePicker.ToString();
            string newEndShiftTime = ShiftEndTimePicker.ToString();

            string oldStartShiftTime = DateTime.Parse(currentShift.TimeStart).TimeOfDay.ToString();
            string oldEndShiftTime = DateTime.Parse(currentShift.TimeEnd).TimeOfDay.ToString();

            oldStartShiftTime = oldStartShiftTime.Remove(oldStartShiftTime.Length - 2);
            oldEndShiftTime = oldEndShiftTime.Remove(oldEndShiftTime.Length - 2);

            newStartShiftTime = newStartShiftTime.Remove(newStartShiftTime.Length - 2);
            newEndShiftTime = newEndShiftTime.Remove(newEndShiftTime.Length - 2);
            
            oldStartShiftTime = oldStartShiftTime + "00";
            oldEndShiftTime = oldEndShiftTime + "00";

            newStartShiftTime = newStartShiftTime + "00";
            newEndShiftTime = newEndShiftTime + "00";


            //TODO: Code to check if any differences have occured
            if ((newStartShiftDate!=oldStartShiftDate)||(oldStartShiftTime!=newStartShiftTime))
            {
                AmendmentTable newStartShift = new AmendmentTable();
                newStartShift.BeforeValue = oldStartShiftDate + " " + oldStartShiftTime;
                newStartShift.Field = "TimeStart";
                newStartShift.ShiftId = currentShift.Key;
                newStartShift.Table = "ShiftTable";
                newStartShift.TimeStamp = DateTime.Now.ToString();
                listOfAmendments.Add(newStartShift);
                currentShift.TimeStart = newStartShiftDate + " " + newStartShiftTime;
            }
            if ((newEndShiftDate!=oldEndShiftDate)||(newEndShiftTime!=oldEndShiftTime))
            {
                AmendmentTable newStopShift = new AmendmentTable();
                newStopShift.BeforeValue = oldEndShiftDate + " " + oldEndShiftTime;
                newStopShift.Field = "TimeEnd";
                newStopShift.ShiftId = currentShift.Key;
                newStopShift.Table = "ShiftTable";
                newStopShift.TimeStamp = DateTime.Now.ToString();
                listOfAmendments.Add(newStopShift);
                currentShift.TimeEnd= newEndShiftDate + " " + newEndShiftTime;
            }
            if (listOfAmendments.Count>0)
            {
                //TODO: Code to add to ammendment table / update curentshift table
                DbService.AmendShift(listOfAmendments, currentShift);
            }
        }

        public List<ShiftTable> Load(DateTime selectedDate)
        {   
            string dateString = selectedDate.Day + "/" + selectedDate.Month + "/" + selectedDate.Year;
            listOfShifts =  DbService.GetShifts(dateString);
            if(listOfShifts.Count==0)
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "No shifts found for this date", Resource.DisplayAlertOkay);
                //Navigation.PopAsync();
            }
            return listOfShifts;
        }

        internal void LoadInfoFromShift(ShiftTable shiftTable)
        {            
            ShiftInfoVisible = true;
            //string[] shiftStart = shiftTable.TimeStart.Split(new char[] { ' ' }, 2);
            //string[] shiftEnd = shiftTable.TimeEnd.Split(new char[] { ' ' }, 2);

            currentShift = shiftTable;

            ShiftStartDatePicker = DateTime.Parse(shiftTable.TimeStart);
            ShiftEndDatePicker = DateTime.Parse(shiftTable.TimeEnd);

            ShiftStartTimePicker = ShiftStartDatePicker.TimeOfDay;
            ShiftEndTimePicker = ShiftEndDatePicker.TimeOfDay;

            OnPropertyChanged("ShiftInfoVisible");
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
