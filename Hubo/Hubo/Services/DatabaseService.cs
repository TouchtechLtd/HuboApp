using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using Xamarin.Forms;

namespace Hubo
{
    public class DatabaseService
    {
        public SQLiteConnection db;

        public DatabaseService()
        {
            db = DependencyService.Get<ISQLite>().GetConnection();
            db.CreateTable<UserTable>();
            db.CreateTable<VehicleTable>();
            db.CreateTable<BreakTable>();
            db.CreateTable<NoteTable>();
            db.CreateTable<ShiftTable>();
            db.CreateTable<VehicleInUseTable>();
            db.CreateTable<AmendmentTable>();
            db.CreateTable<TipTable>();
        }

        public void IncrementDates()
        {
            List<ShiftTable> shifts = new List<ShiftTable>();
            shifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable]");
            foreach(ShiftTable shift in shifts)
            {
                List<BreakTable> breaks = new List<BreakTable>();
                breaks = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ShiftKey] == " + shift.Key + "");
                foreach(BreakTable breakItem in breaks)
                {
                    DateTime start = new DateTime();
                    DateTime end = new DateTime();
                    start = DateTime.Parse(breakItem.StartTime);
                    end = DateTime.Parse(breakItem.EndTime);
                    start = start.AddDays(1);
                    end = end.AddDays(1);
                    breakItem.StartTime = start.ToString();
                    breakItem.EndTime = end.ToString();
                    db.Update(breakItem);
                }
                List<NoteTable> notes = new List<NoteTable>();
                notes = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [ShiftKey] == " + shift.Key + "");
                foreach (NoteTable note in notes)
                {
                    DateTime date = new DateTime();
                    date = DateTime.Parse(note.Date);
                    date = date.AddDays(1);
                    note.Date = date.ToString();
                    db.Update(note);
                }
                DateTime startShift = new DateTime();
                DateTime endShift = new DateTime();
                startShift = DateTime.Parse(shift.StartTime);
                endShift = DateTime.Parse(shift.EndTime);
                startShift = startShift.AddDays(1);
                endShift = endShift.AddDays(1);
                shift.StartTime = startShift.ToString();
                shift.EndTime = endShift.ToString();
                db.Update(shift);
            }
        }

        internal string GetName()
        {
            List<UserTable> listUser = new List<UserTable>();
            listUser = db.Query<UserTable>("SELECT * FROM [UserTable]");
            if((listUser.Count==0)||(listUser.Count > 1))
            {
                return "ERROR";
            }
            return listUser[0].FirstName + " " + listUser[0].LastName;
        }

        internal VehicleTable LoadVehicleInfo(VehicleInUseTable vehicle)
        {
            List<VehicleTable> vehicleDetails = new List<VehicleTable>();
            vehicleDetails = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [Key] == " + vehicle.VehicleKey + "");
            if((vehicleDetails.Count==0)||(vehicleDetails.Count>1))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "ERROR GET ONE VEHICLE", Resource.DisplayAlertOkay);
            }
            return vehicleDetails[0];
        }

        internal void HideTip(string tipName)
        {
            List<TipTable> listOfTips = new List<TipTable>();
            listOfTips = db.Query<TipTable>("SELECT * FROM [TipTable] WHERE [TipName] == '" + tipName + "'");
            listOfTips[0].ActiveTip = 0;
            db.Execute("UPDATE [TipTable] SET [ActiveTip] = 0 WHERE [TipName] == '" + tipName + "'");
        }

        internal bool ShowTip(string tipName)
        {
            List<TipTable> listOfTips = new List<TipTable>();
            listOfTips = db.Query<TipTable>("SELECT * FROM [TipTable] WHERE [TipName] == '" + tipName + "'");
            if(listOfTips[0].ActiveTip==1)
            {
                return true;
            }
            return false;
        }

        internal int TotalBeforeBreak()
        {
            //TODO: Code to retrieve hours since last gap of 24 hours
            List<ShiftTable> listOfShiftsForAmount = new List<ShiftTable>();
            int totalHours = 0;
            listOfShiftsForAmount = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] ORDER BY Key DESC LIMIT 20");
            for(int i = 1; i < listOfShiftsForAmount.Count; i++)
            {
                DateTime previous = new DateTime();
                DateTime current = new DateTime();

                previous = DateTime.Parse(listOfShiftsForAmount[i - 1].StartTime);
                current = DateTime.Parse(listOfShiftsForAmount[i].EndTime);

                TimeSpan difference = new TimeSpan();
                difference =  previous - current;
                if(difference.TotalHours > 24)
                {
                    if(listOfShiftsForAmount[i - 1].EndTime == null)
                    {
                        totalHours = Convert.ToInt32((DateTime.Now - DateTime.Parse(listOfShiftsForAmount[i - 1].StartTime)).Hours) + totalHours;
                    }
                    else
                    {
                        totalHours = Convert.ToInt32((DateTime.Parse(listOfShiftsForAmount[i - 1].EndTime) - DateTime.Parse(listOfShiftsForAmount[i - 1].StartTime)).Hours) + totalHours;
                    }
                    return totalHours;
                }
                else
                {
                    TimeSpan amountOfTime = new TimeSpan();
                    if(listOfShiftsForAmount[i - 1].EndTime == null)
                    {
                        amountOfTime = DateTime.Now - DateTime.Parse(listOfShiftsForAmount[i - 1].StartTime);
                    }
                    else
                    {
                        amountOfTime = DateTime.Parse(listOfShiftsForAmount[i - 1].EndTime) - DateTime.Parse(listOfShiftsForAmount[i - 1].StartTime);
                    }
                    totalHours = totalHours + Convert.ToInt32(amountOfTime.Hours);
                }
            }
            return totalHours;
        }

        internal List<ShiftTable> GetShiftsWeek(DateTime selectedDate)
        {
            List<ShiftTable> returnShifts = new List<ShiftTable>();
            int daysWithoutShifts = 0;
            while((returnShifts.Count < 8))
            {
                List<ShiftTable> listOfShifts = new List<ShiftTable>();
                string dateString = selectedDate.Day + "/" + selectedDate.Month + "/" + selectedDate.Year;
                listOfShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartTime] LIKE '" + dateString + "%'");
                foreach(ShiftTable shift in listOfShifts)
                {
                    returnShifts.Add(shift);
                }
                if(listOfShifts.Count == 0)
                {
                    daysWithoutShifts++;
                }
                else
                {
                    daysWithoutShifts = 0;
                }
                if(daysWithoutShifts == 8)
                {
                    break;
                }
                selectedDate = selectedDate.AddDays(-1);
            }
            return returnShifts;
        }

        internal int HoursTillReset()
        {
            //TODO: Code to retrieve time since last shift (Once it hits 24 hours, 70 hour a week will reset
            List<ShiftTable> listOfShifts = new List<ShiftTable>();
            listOfShifts = db.Query<ShiftTable>("SELECT * FROM[ShiftTable] ORDER BY Key DESC LIMIT 2");
            ShiftTable lastShift = new ShiftTable();
            if(listOfShifts.Count==0)
            {
                return -1;
            }
            lastShift = listOfShifts[0];
            DateTime dateNow = new DateTime();
            DateTime dateOnLastShift = new DateTime();
            dateNow = DateTime.Now;
            string dateLastShiftString = lastShift.EndTime;
            if(dateLastShiftString == null)
            {
                //Shift is still occuring, thus 0
                return 0;
            }
            dateOnLastShift = DateTime.Parse(dateLastShiftString);
            TimeSpan time = new TimeSpan();
            time = dateNow - dateOnLastShift;
            if(time.TotalDays >= 1)
            {
                //Have rested for longer/as long as 24 hours
                return -2;
            }
            int timeSinceReset = Int32.Parse(time.ToString().Remove(2));
            return timeSinceReset;
        }

        internal List<VehicleInUseTable> GetUsedVehicles(ShiftTable currentShift)
        {
            List<VehicleInUseTable> usedVehicles = new List<VehicleInUseTable>();
            usedVehicles = db.Query<VehicleInUseTable>("SELECT * FROM [VehicleInUseTable] WHERE [ShiftKey] == " + currentShift.Key + "");
            return usedVehicles;
        }


        internal void AddAmendments(List<AmendmentTable> listOfAmendments, ShiftTable currentShift = null, VehicleInUseTable currentVehicleInUse = null, List<NoteTable> listOfNotes = null, BreakTable currentBreak = null, NoteTable currentNote = null)
        {
            if(currentNote != null)
            {
                db.Update(currentNote);
            }

            if(currentBreak != null)
            {
                db.Update(currentBreak);
            }
            if(listOfNotes!=null)
            {
                foreach(NoteTable note in listOfNotes)
                {
                    db.Update(note);
                }
            }
            if(currentShift!=null)
            {
                db.Update(currentShift);
            }
            if(currentVehicleInUse!=null)
            {
                db.Update(currentVehicleInUse);
            }

            foreach (AmendmentTable amendment in listOfAmendments)
            {
                db.Insert(amendment);
            }
        }

        

        internal List<ShiftTable> GetShifts(DateTime selectedDate)
        {
            List<ShiftTable> listOfShiftsToAdd = new List<ShiftTable>();
            List<ShiftTable> listOfShifts = new List<ShiftTable>();
            List<DateTime> listOfDates = new List<DateTime>();
            DateTime fromDate = selectedDate.AddDays(-7);
            selectedDate = selectedDate.AddDays(1);
            while(fromDate != selectedDate)
            {
                listOfDates.Add(fromDate);
                fromDate = fromDate.AddDays(1);
            }

            foreach(DateTime date in listOfDates)
            {
                string dateString = date.Day + "/" + date.Month + "/" + date.Year;
                listOfShiftsToAdd = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartTime] LIKE '" + dateString + "%'");
                if(listOfShiftsToAdd.Count != 0)
                {
                    listOfShifts.Add(listOfShiftsToAdd[0]);
                }
            }

            return listOfShifts;
        }

        internal void SaveNote(string note, DateTime date, string location, int huboEntry=0)
        {
            NoteTable newNote = new NoteTable();
            newNote.Note = note;
            newNote.StandAloneNote = true;
            newNote.Date = date.ToString();
            newNote.Location = location;
            //newNote.Time = time.ToString();
            newNote.Hubo = huboEntry;
            List<ShiftTable> currentShiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if((currentShiftList.Count==0)||(currentShiftList.Count>1))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "UNABLE TO GET CURRENT SHIFT, THUS NOTE NOT SAVED", Resource.DisplayAlertOkay);
            }
            else
            {
                ShiftTable currentShift = currentShiftList[0];
                newNote.ShiftKey = currentShift.Key;
                db.Insert(newNote);
            }
        }

        internal void SaveNoteFromVehicle(string note, DateTime date, string location, int huboEntry, int vehicleKey)
        {

            List<ShiftTable> listOfShifts = new List<ShiftTable>();
            listOfShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if ((listOfShifts.Count == 0) || (listOfShifts.Count > 1))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "UNABLE TO GET CURRENT SHIFT, THUS NOTE NOT SAVED", Resource.DisplayAlertOkay);
            }

            else
            {
                NoteTable newNote = new NoteTable();
                newNote.Date = date.ToString();
                newNote.Location = location;
                newNote.Note = note;
                newNote.ShiftKey = listOfShifts[0].Key;
                db.Insert(newNote);

                List<NoteTable> listOfNotes = new List<NoteTable>();
                listOfNotes = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [Date] == '" + date + "' AND [Location] == '" + location + "' AND [Note] == '" + note + "' AND [ShiftKey] == '" + listOfShifts[0].Key + "'");
                if (VehicleInUse())
                {
                    //TODO: CODE TO Turn off vehicle in use
                    List<VehicleInUseTable> listOfVehiclesInUse = new List<VehicleInUseTable>();
                    listOfVehiclesInUse = db.Query<VehicleInUseTable>("SELECT * FROM [VehicleInUseTable] WHERE [ActiveVehicle] == 1");
                    if(listOfVehiclesInUse.Count==0)
                    {
                        Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "CANNOT RETRIEVE VEHICLE IN USE", Resource.DisplayAlertOkay);
                        return;
                    }
                    VehicleInUseTable vehicleInUse = new VehicleInUseTable();
                    vehicleInUse = listOfVehiclesInUse[0];
                    vehicleInUse.ActiveVehicle = 0;
                    vehicleInUse.EndNoteKey = listOfNotes[0].Key;
                    vehicleInUse.HuboEnd = huboEntry;
                    vehicleInUse.ShiftKey = listOfShifts[0].Key;
                    vehicleInUse.VehicleKey = vehicleKey;
                    db.Update(vehicleInUse);
                }

                else
                {
                    //TODO: Code to turn on vehicle in use
                    VehicleInUseTable newVehicleInUse = new VehicleInUseTable();
                    newVehicleInUse.ActiveVehicle = 1;
                    newVehicleInUse.HuboStart = huboEntry;
                    newVehicleInUse.ShiftKey = listOfShifts[0].Key;
                    newVehicleInUse.StartNoteKey = listOfNotes[0].Key;
                    newVehicleInUse.VehicleKey = vehicleKey;
                    db.Insert(newVehicleInUse);
                }
            }
            MessagingCenter.Send<string>("UpdateActiveVehicle", "UpdateActiveVehicle");
            MessagingCenter.Send<string>("UpdateVehicleInUse", "UpdateVehicleInUse");

        }

        internal void SaveNoteFromBreak(string note, DateTime date, string location, int huboEntry)
        {
            NoteTable newNote = new NoteTable();
            newNote.Note = note;
            newNote.Date = date.ToString();
            newNote.Location = location;
            newNote.Hubo = huboEntry;
            List<ShiftTable> currentShiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if ((currentShiftList.Count == 0) || (currentShiftList.Count > 1))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "UNABLE TO GET CURRENT SHIFT, THUS NOTE NOT SAVED", Resource.DisplayAlertOkay);
            }
            else
            {
                ShiftTable currentShift = currentShiftList[0];
                newNote.ShiftKey = currentShift.Key;
                db.Insert(newNote);
                List<NoteTable> notes = new List<NoteTable>();
                notes = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [Note] == '" + note + "' AND [ShiftKey] == " + currentShift.Key + "");
                if((notes.Count==0)||(notes.Count>1))
                {
                    Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "UNABLE TO GET CURRENT NOTE", Resource.DisplayAlertOkay);
                    return;
                }
                List<BreakTable> listBreaks = new List<BreakTable>();
                listBreaks = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ActiveBreak] == 1");
                int noteKeyForBreak = notes[0].Key;
                //No active breaks, so starting break.
                if(listBreaks.Count==0)
                {
                    StartBreak(noteKeyForBreak);
                }
                //Active Break, so stopping break.
                else if(listBreaks.Count == 1)
                {
                    StopBreak(noteKeyForBreak);
                }
            }
        }

        internal List<NoteTable> GetNotes(List<BreakTable> listOfBreaks)
        {
            List<NoteTable> listOfNotes = new List<NoteTable>();
            if(listOfBreaks.Count!=0 && listOfBreaks!=null)
            {
                foreach (BreakTable breakItem in listOfBreaks)
                {
                    List<NoteTable> tempList = new List<NoteTable>();
                    tempList = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [Key] ==" + breakItem.StartNoteKey + " OR [Key] == " + breakItem.StopNoteKey + "");
                    foreach (NoteTable note in tempList)
                    {
                        listOfNotes.Add(note);
                    }
                }
            }
            else
            {
                listOfNotes = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [StandAloneNote] == 1");
            }

            return listOfNotes;
            
        }

        internal List<BreakTable> GetBreaks(ShiftTable currentShift)
        {
            List<BreakTable> listOfBreaks = new List<BreakTable>();
            listOfBreaks = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ShiftKey] == " + currentShift.Key + "");
            return listOfBreaks;
        }


        internal bool CheckLoggedIn()
        {
            List<UserTable> list = db.Query<UserTable>("SELECT * FROM [UserTable]");
            if(list.Count > 0)
            {
                return true;
            }
            return false;
        }

        internal bool VehicleInUse()
        {
            List<VehicleInUseTable> listVehiclesInUse = new List<VehicleInUseTable>();
            listVehiclesInUse = db.Query<VehicleInUseTable>("SELECT * FROM [VehicleInUseTable] WHERE [ActiveVehicle] == 1");
            if(listVehiclesInUse.Count==0)
            {
                return false;
            }
            else if(listVehiclesInUse.Count > 1)
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "MORE THAN ONE ACTIVE VEHICLE DETECTED", Resource.DisplayAlertOkay);
                return false;
            }
            return true;
        }
        internal void StopVehicleInUse(string huboEntry)
        {
            List<VehicleInUseTable> listVehiclesInUse = new List<VehicleInUseTable>();
            listVehiclesInUse = db.Query<VehicleInUseTable>("SELECT * FROM [VehicleInUseTable] WHERE [ActiveVehicle]==1");
            if((listVehiclesInUse.Count==0)||(listVehiclesInUse.Count>1))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "MORE THAN ONE VEHICLE FOUND", Resource.DisplayAlertOkay);
                return;
            }
            VehicleInUseTable vehicleInUse = new VehicleInUseTable();
            vehicleInUse = listVehiclesInUse[0];
            vehicleInUse.HuboEnd = Int32.Parse(huboEntry);
            vehicleInUse.ActiveVehicle = 0;
            db.Update(vehicleInUse);

            MessagingCenter.Send<string>("UpdateActiveVehicle", "UpdateActiveVehicle");
            MessagingCenter.Send<string>("UpdateVehicleInUse", "UpdateVehicleInUse");
            
            return;
             
        }

        internal bool NoBreaksActive()
        {
            List<BreakTable> listOfBreaks = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ActiveBreak] == 1");
            if(listOfBreaks.Count>0)
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "PLEASE END BREAKS BEFORE ENDING SHIFT", Resource.DisplayAlertOkay);
                return false;
            }
            return true;
        }

        internal List<VehicleTable> GetVehicles()
        {
            List<VehicleTable> vehiclesList = new List<VehicleTable>();
            vehiclesList = db.Query<VehicleTable>("SELECT * FROM [VehicleTable]");
            return vehiclesList;
        }

        internal void SetVehicleInUse(int key, string huboEntry)
        {
            VehicleInUseTable vehicleToUse = new VehicleInUseTable();
            List<ShiftTable> listOfActiveShifts = new List<ShiftTable>();
            listOfActiveShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift]==1");
            if((listOfActiveShifts.Count==0)||(listOfActiveShifts.Count>1))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "UNABLE TO RETRIEVE ACTIVE SHIFT", Resource.DisplayAlertOkay);
                return;
            }
            vehicleToUse.ShiftKey = listOfActiveShifts[0].Key;
            vehicleToUse.VehicleKey = key;
            vehicleToUse.HuboStart = Int32.Parse(huboEntry);
            vehicleToUse.ActiveVehicle = 1;

            db.Insert(vehicleToUse);

            MessagingCenter.Send<string>("UpdateActiveVehicle", "UpdateActiveVehicle");
            MessagingCenter.Send<string>("UpdateVehicleInUse", "UpdateVehicleInUse");

        }

        internal bool CheckActiveShift()
        {
            List<ShiftTable> shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if(shiftList.Count==0)
            {
                return false;
            }
            else if(shiftList.Count==1)
            {
                return true;
            }
            Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "MORE THAN ONE ACTIVE SHIFT DISCOVERED", Resource.DisplayAlertOkay);
            return false;

        }

        internal List<NoteTable> GetNotesFromVehicle(VehicleInUseTable currentVehicleInUse)
        {
            List<NoteTable> listOfNotes = new List<NoteTable>();
            listOfNotes = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [Key] == " + currentVehicleInUse.StartNoteKey + " OR [Key] == " + currentVehicleInUse.EndNoteKey + "");
            return listOfNotes;
        }

        internal VehicleTable GetCurrentVehicle()
        {
            //Retrieve currently used vehicle
            VehicleInUseTable currentVehicleInUse;
            List<VehicleInUseTable> currentVehicleInUseList = db.Query<VehicleInUseTable>("SELECT * FROM [VehicleInUseTable] WHERE [ActiveVehicle] == 1");
            currentVehicleInUse = currentVehicleInUseList[0];

            //Retrieve vehicle details using currentvehicles key
            VehicleTable vehicle;
            List<VehicleTable> vehicleList = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [Key] == " + currentVehicleInUse.VehicleKey + "");
            vehicle = vehicleList[0];

            return vehicle;
        }

        internal bool VehicleActive()
        {
            List<VehicleInUseTable> currentVehicles = new List<VehicleInUseTable>();
            currentVehicles = db.Query<VehicleInUseTable>("SELECT * FROM [VehicleInUseTable] WHERE [ActiveVehicle] == 1");
            if(currentVehicles.Count==0)
            {
                return false;
            }
            else if(currentVehicles.Count==1)
            {
                return true;
            }
            Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "MORE THAN ONE VEHICLE ACTIVE", Resource.DisplayAlertOkay);
            return false;
        }

        internal bool StopBreak(int noteKey)
        {
            List<BreakTable> currentBreaks = new List<BreakTable>();
            currentBreaks = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ActiveBreak] == 1");
            if ((currentBreaks.Count == 0) || (currentBreaks.Count > 1))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "UNABLE TO GET ACTIVE BREAK", Resource.DisplayAlertOkay);
                return false;
            }
            BreakTable currentBreak = currentBreaks[0];
            currentBreak.EndTime = DateTime.Now.ToString();
            currentBreak.ActiveBreak = 0;
            currentBreak.StopNoteKey = noteKey;
            db.Update(currentBreak);
            return true;
        }

        internal void InsertVehicle(VehicleTable vehicleToAdd)
        {
            db.Insert(vehicleToAdd);
            MessagingCenter.Send<string>("UpdateVehicles", "UpdateVehicles");
        }

        internal bool Login(UserTable user)
        {
            db.Insert(user);
            return true;
        }

        internal bool StartBreak(int noteKey)
        {
            BreakTable newBreak = new BreakTable();
            List<ShiftTable> activeShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if(CheckActiveShiftIsCorrect(activeShifts))
            {
                newBreak.ShiftKey = activeShifts[0].Key;
                newBreak.StartTime = DateTime.Now.ToString();
                newBreak.StartNoteKey = noteKey;
                newBreak.ActiveBreak = 1;
                db.Insert(newBreak);
                return true;
            }
            return false;
            
        }

        internal List<string> GetChecklist()
        {
            List<string> questions = new List<string>();
            questions.Add("This is a test");
            questions.Add("This is a test");
            questions.Add("This is a test");
            questions.Add("This is a test");
            return questions;
        }

        internal void SetVehicleActive(VehicleTable currentVehicle)
        {
            List<VehicleInUseTable> activeVehicles = db.Query<VehicleInUseTable>("SELECT * FROM [VehicleInUseTable] WHERE [ActiveVehicle] == 1");
            foreach(VehicleInUseTable vehicle in activeVehicles)
            {
                if(currentVehicle.Key == vehicle.VehicleKey)
                {
                    vehicle.ActiveVehicle = 1;
                    db.Update(vehicle);
                }
            }

        }
        internal void SetVehicleInactive()
        {
            List<VehicleInUseTable> activeVehicles = db.Query<VehicleInUseTable>("SELECT * FROM [VehicleInUseTable] WHERE [ActiveVehicle] == 1");
            foreach (VehicleInUseTable vehicle in activeVehicles)
            {
                vehicle.ActiveVehicle = 0;
                db.Update(vehicle);
            }
        }

        internal void UpdateVehicleInfo(VehicleTable editedVehicle)
        {
            db.Update(editedVehicle);
            MessagingCenter.Send<string>("UpdateVehicles", "UpdateVehicles");
        }

        internal void Logout()
        {
            db.Query<UserTable>("DELETE FROM [UserTable]");
        }

        internal void StartShift()
        {
            ShiftTable newShift = new ShiftTable();
            newShift.ActiveShift = 1;
            newShift.StartTime = DateTime.Now.ToString();
            db.Insert(newShift);
        }

        internal bool StopShift()
        {
            //Get current shift
            List<ShiftTable> activeShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if(CheckActiveShiftIsCorrect(activeShifts))
            {
                //TODO: Code to check that this shift had a vehicle assigned to it
                List<VehicleInUseTable> listOfUsedVehicles = db.Query<VehicleInUseTable>("SELECT * FROM [VehicleInUseTable] WHERE [ShiftKey] == " + activeShifts[0].Key + "");
                if(listOfUsedVehicles.Count == 0)
                {
                    Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Please select a vehicle before ending your shift", Resource.DisplayAlertOkay);
                    return false;
                }
                ShiftTable activeShift = activeShifts[0];
                activeShift.EndTime = DateTime.Now.ToString();
                activeShift.ActiveShift = 0;
                db.Update(activeShift);
                return true;
            }
            return false;
        }

        private bool CheckActiveShiftIsCorrect(List<ShiftTable> activeShifts)
        {
            if (activeShifts.Count == 0)
            {
                Application.Current.MainPage.DisplayAlert("WARNING", "NO ACTIVE SHIFTS", "OK");
                return false;
            }
            else if (activeShifts.Count > 1)
            {
                Application.Current.MainPage.DisplayAlert("WARNING", "MORE THAN ACTIVE SHIFT", "OK");
                return false;
            }
            return true;
        }

        internal UserTable GetUserInfo()
        {
            List<UserTable> list = db.Query<UserTable>("SELECT * FROM [UserTable]");
            return list[0];
        }

        internal void UpdateUserInfo(UserTable user)
        {
            db.Query<UserTable>("DELETE FROM [UserTable]");
            db.Insert(user);
        }

        internal int SaveShift(DateTime startDate, DateTime endDate, VehicleTable vehicleKey, string huboStart, string huboEnd, string locationStart, string locationEnd)
        {
            ShiftTable shiftTable = new ShiftTable();
            VehicleInUseTable vehicleTable = new VehicleInUseTable();
            NoteTable noteTableStart = new NoteTable();
            NoteTable noteTableEnd = new NoteTable();

            shiftTable.ActiveShift = 0;
            shiftTable.StartTime = startDate.ToString();
            shiftTable.EndTime = endDate.ToString();
            db.Insert(shiftTable);

            List<ShiftTable> currentShiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] ORDER BY [Key] DESC LIMIT 1");
            if (currentShiftList.Count < 1 && currentShiftList.Count > 1)
            {
                return -1;
            }

            ShiftTable shiftKey = currentShiftList[0];
            vehicleTable.ShiftKey = shiftKey.Key;
            noteTableStart.ShiftKey = shiftKey.Key;
            noteTableEnd.ShiftKey = shiftKey.Key;

            noteTableStart.Note = "Start Shift";
            noteTableStart.Date = startDate.ToString();
            noteTableStart.Hubo = int.Parse(huboStart);
            noteTableStart.Location = locationStart;
            noteTableStart.StandAloneNote = false;

            noteTableEnd.Note = "End Shift";
            noteTableEnd.Date = endDate.ToString();
            noteTableEnd.Hubo = int.Parse(huboEnd);
            noteTableEnd.Location = locationEnd;
            noteTableEnd.StandAloneNote = false;

            db.Insert(noteTableStart);
            db.Insert(noteTableEnd);

            List<NoteTable> currentNoteList = db.Query<NoteTable>("SELECT * FROM [NoteTable] ORDER BY [Key] DESC LIMIT 2");
            if (currentNoteList.Count < 2 && currentNoteList.Count > 2)
            {
                db.Delete(shiftKey.Key);
                return -1;
            }

            NoteTable startNoteKey = currentNoteList[1];
            NoteTable endNoteKey = currentNoteList[0];
            vehicleTable.StartNoteKey = startNoteKey.Key;
            vehicleTable.EndNoteKey = endNoteKey.Key;

            vehicleTable.VehicleKey = vehicleKey.Key;
            vehicleTable.ActiveVehicle = 0;
            vehicleTable.HuboStart = int.Parse(huboStart);
            vehicleTable.HuboEnd = int.Parse(huboEnd);

            db.Insert(vehicleTable);

            return shiftKey.Key;
        }

        internal void SaveBreak(string breakStart, string breakEnd, int shiftKey, string startNote, int startHubo, string startLocation, string endNote, int endHubo, string endLocation)
        {
            //TODO: create save break
            NoteTable breakStartNote = new NoteTable();
            NoteTable breakEndNote = new NoteTable();
            BreakTable newBreak = new BreakTable();

            breakStartNote.Note = startNote;
            breakStartNote.Date = breakStart;
            breakStartNote.ShiftKey = shiftKey;
            breakStartNote.Hubo = startHubo;
            breakStartNote.Location = startLocation;
            breakStartNote.StandAloneNote = false;

            breakEndNote.Note = endNote;
            breakEndNote.Date = breakEnd;
            breakEndNote.ShiftKey = shiftKey;
            breakEndNote.Hubo = endHubo;
            breakEndNote.Location = endLocation;
            breakEndNote.StandAloneNote = false;

            db.Insert(breakStartNote);
            db.Insert(breakEndNote);

            List<NoteTable> currentNoteList = db.Query<NoteTable>("SELECT * FROM [NoteTable] ORDER BY [Key] DESC LIMIT 2");

            newBreak.StartTime = breakStart;
            newBreak.EndTime = breakEnd;
            newBreak.ShiftKey = shiftKey;
            newBreak.ActiveBreak = 0;
            newBreak.StartNoteKey = currentNoteList[1].Key;
            newBreak.StopNoteKey = currentNoteList[0].Key;

            db.Insert(newBreak);
        }

        internal void SaveManNote(string note, string date, int shiftKey, int hubo, string location)
        {
            //TODO: create save note
            NoteTable newNote = new NoteTable();
            newNote.Note = note;
            newNote.Date = date;
            newNote.ShiftKey = shiftKey;
            newNote.Hubo = hubo;
            newNote.Location = location;
            newNote.StandAloneNote = true;

            db.Insert(newNote);
        }

        internal List<ExportData> GetExportData()
        {
            ExportData exportData = new ExportData();

            List<BreakTable> breakList = new List<BreakTable>();
            List<NoteTable> noteList = new List<NoteTable>();
            List<ShiftTable> shiftList = new List<ShiftTable>();
            List<VehicleInUseTable> vehicleInUseList = new List<VehicleInUseTable>();
            List<VehicleTable> vehicleList = new List<VehicleTable>();
            List<ExportData> exportList = new List<ExportData>();

            shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartTime] > (SELECT DATE('now', '-7 day'))");

            foreach (ShiftTable shiftData in shiftList)
            {
                breakList = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ShiftKey] = " + shiftData.Key);
                noteList = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [ShiftKey] = " + shiftData.Key + " AND [StandAloneNote] = 1");
                vehicleInUseList = db.Query<VehicleInUseTable>("SELECT * FROM [VehicleInUseTable] WHERE [ShiftKey] = " + shiftData.Key);
                vehicleList = db.Query<VehicleTable>("SELECT * FROM [Vehicle] WHERE [Key] = " + vehicleInUseList[0].VehicleKey);

                List<NoteTable> breakStartNotesList = db.Query<NoteTable>("SELECT [Note], [Hubo], [Location] FROM [NoteTable] WHERE [Key] = " + breakList[0].StartNoteKey + " AND [ShiftKey]" + shiftData.Key);
                List<NoteTable> breakEndNotesList = db.Query<NoteTable>("SELECT [Note], [Hubo], [Location] FROM [NoteTable] WHERE [Key] = " + breakList[0].StopNoteKey + " AND [ShiftKey]" + shiftData.Key);
                List<NoteTable> locationStartNotesList = db.Query<NoteTable>("SELECT [Location] FROM [NoteTable] WHERE [Key] = " + vehicleInUseList[0].StartNoteKey + " AND [ShiftKey]" + shiftData.Key);
                List<NoteTable> locationEndNotesList = db.Query<NoteTable>("SELECT [Location] FROM [NoteTable] WHERE [Key] = " + vehicleInUseList[0].EndNoteKey + " AND [ShiftKey]" + shiftData.Key);

                exportData.shiftStart = shiftData.StartTime;
                exportData.shiftEnd = shiftData.EndTime;
                exportData.activeShift = shiftData.ActiveShift.ToString();

                exportData.vehicleMake = vehicleList[0].Make;
                exportData.vehicleModel = vehicleList[0].Model;
                exportData.vehicleRego = vehicleList[0].Registration;
                exportData.vehicleCompany = vehicleList[0].Company;
                exportData.currentVehicle = vehicleInUseList[0].ActiveVehicle.ToString();

                exportData.huboStart = vehicleInUseList[0].HuboStart.ToString();
                exportData.huboEnd = vehicleInUseList[0].HuboEnd.ToString();
                exportData.startLocation = locationStartNotesList[0].Location;
                exportData.endLocation = locationEndNotesList[0].Location;

                exportData.breakStart = breakList[0].StartTime;
                exportData.breakEnd = breakList[0].EndTime;
                exportData.activeBreak = breakList[0].ActiveBreak.ToString();
                exportData.breakDetails = breakStartNotesList[0].Note;
                exportData.breakStartHubo = breakStartNotesList[0].Hubo.ToString();
                exportData.breakEndHubo = breakEndNotesList[0].Hubo.ToString();
                exportData.breakStartLocation = breakStartNotesList[0].Location;
                exportData.breakEndLocation = breakEndNotesList[0].Location;

                exportData.noteTime = noteList[0].Date;
                exportData.noteDetails = noteList[0].Note;
                exportData.noteHubo = noteList[0].Hubo.ToString();
                exportData.noteLocation = noteList[0].Location;

                exportList.Add(exportData);        
            }

            return exportList;
        }
    }
}
