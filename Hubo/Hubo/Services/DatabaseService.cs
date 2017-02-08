using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using Xamarin.Forms;
using Plugin.Geolocator;

namespace Hubo
{
    public class DatabaseService
    {
        public SQLiteConnection db;
        RestService RestAPI;

        public DatabaseService()
        {
            db = DependencyService.Get<ISQLite>().GetConnection();
            db.CreateTable<UserTable>();
            db.CreateTable<VehicleTable>();
            db.CreateTable<BreakTable>();
            db.CreateTable<NoteTable>();
            db.CreateTable<ShiftTable>();
            db.CreateTable<DriveTable>();
            db.CreateTable<AmendmentTable>();
            db.CreateTable<TipTable>();
            db.CreateTable<CompanyTable>();
            db.CreateTable<LoadTextTable>();
            db.CreateTable<Geolocation>();
        }

        internal List<LoadTextTable> GetLoadingText()
        {
            List<LoadTextTable> loadList = new List<LoadTextTable>();
            loadList = db.Query<LoadTextTable>("SELECT * FROM [LoadTextTable]");

            return loadList;
        }

        public void CheckShifts()
        {
            List<ShiftTable> shifts = new List<ShiftTable>();
            shifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable]");
            foreach (ShiftTable item in shifts)
                Application.Current.MainPage.DisplayAlert("Check Shifts", item.ActiveShift.ToString(), "OK");
        }

        //public void IncrementDates()
        //{
        //    List<ShiftTable> shifts = new List<ShiftTable>();
        //    shifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable]");
        //    foreach (ShiftTable shift in shifts)
        //    {
        //        List<BreakTable> breaks = new List<BreakTable>();
        //        breaks = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ShiftKey] == " + shift.Key + "");
        //        foreach (BreakTable breakItem in breaks)
        //        {
        //            DateTime start = new DateTime();
        //            DateTime end = new DateTime();
        //            start = DateTime.Parse(breakItem.StartTime);
        //            end = DateTime.Parse(breakItem.EndTime);
        //            start = start.AddDays(1);
        //            end = end.AddDays(1);
        //            breakItem.StartTime = start.ToString();
        //            breakItem.EndTime = end.ToString();
        //            db.Update(breakItem);
        //        }
        //        List<NoteTable> notes = new List<NoteTable>();
        //        notes = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [ShiftKey] == " + shift.Key + "");
        //        foreach (NoteTable note in notes)
        //        {
        //            DateTime date = new DateTime();
        //            date = DateTime.Parse(note.Date);
        //            date = date.AddDays(1);
        //            note.Date = date.ToString();
        //            db.Update(note);
        //        }
        //        DateTime startShift = new DateTime();
        //        DateTime endShift = new DateTime();
        //        startShift = DateTime.Parse(shift.StartTime);
        //        endShift = DateTime.Parse(shift.EndTime);
        //        startShift = startShift.AddDays(1);
        //        endShift = endShift.AddDays(1);
        //        shift.StartTime = startShift.ToString();
        //        shift.EndTime = endShift.ToString();
        //        db.Update(shift);
        //    }
        //}

        internal void InsertUser(UserTable user)
        {
            db.Insert(user);
        }

        internal string GetName()
        {
            List<UserTable> listUser = new List<UserTable>();
            listUser = db.Query<UserTable>("SELECT * FROM [UserTable]");
            if ((listUser.Count == 0) || (listUser.Count > 1))
            {
                return "ERROR";
            }
            return listUser[0].FirstName + " " + listUser[0].LastName;
        }

        internal VehicleTable LoadVehicleInfo(DriveTable vehicle)
        {
            List<VehicleTable> vehicleDetails = new List<VehicleTable>();
            vehicleDetails = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [Key] == " + vehicle.VehicleKey + "");
            if ((vehicleDetails.Count == 0) || (vehicleDetails.Count > 1))
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
            if (listOfTips[0].ActiveTip == 1)
            {
                return true;
            }
            return false;
        }

        internal bool ClearTablesForNewUser()
        {
            try
            {
                db.Query<VehicleTable>("DROP TABLE [VehicleTable]");
                db.Query<UserTable>("DROP TABLE [UserTable]");
                db.Query<CompanyTable>("DROP TABLE [CompanyTable]");

                db.CreateTable<UserTable>();
                db.CreateTable<CompanyTable>();
                db.CreateTable<VehicleTable>();
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool ClearTablesForUserShifts()
        {
            try
            {
                db.Query<ShiftTable>("DROP TABLE [ShiftTable]");
                db.Query<DriveTable>("DROP TABLE [DriveTable]");
                db.Query<BreakTable>("DROP TABLE [BreakTable]");

                db.CreateTable<ShiftTable>();
                db.CreateTable<DriveTable>();
                db.CreateTable<BreakTable>();
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal void InsertUserVehicles(VehicleTable vehicle)
        {
            db.Insert(vehicle);
        }

        internal void InsertUserComapany(CompanyTable company)
        {
            db.Insert(company);
        }

        internal void VehicleOffine(VehicleTable vehicle)
        {
            var tbl = db.GetTableInfo("VehicleOffline");

            if (tbl == null)
                db.CreateTable<VehicleOffline>();

            VehicleOffline offline = new VehicleOffline();

            offline.VehicleKey = vehicle.Key;

            db.Insert(offline);
        }

        internal double TotalBeforeBreak()
        {
            //TODO: Code to retrieve hours since last gap of 24 hours
            List<ShiftTable> listOfShiftsForAmount = new List<ShiftTable>();
            double totalHours = 0;
            listOfShiftsForAmount = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] ORDER BY Key DESC LIMIT 20");
            for (int i = 1; i < listOfShiftsForAmount.Count; i++)
            {
                NoteTable currentStartNote = db.Get<NoteTable>(listOfShiftsForAmount[i].StartShiftNoteId);
                NoteTable currentEndNote = db.Get<NoteTable>(listOfShiftsForAmount[i].StopShiftNoteId);

                NoteTable previousStartNote = db.Get<NoteTable>(listOfShiftsForAmount[i - 1].StartShiftNoteId);
                NoteTable previousEndNote = db.Get<NoteTable>(listOfShiftsForAmount[i - 1].StopShiftNoteId);


                DateTime previous = new DateTime();
                DateTime current = new DateTime();

                //previous = DateTime.Parse(listOfShiftsForAmount[i - 1].StartTime);
                //current = DateTime.Parse(listOfShiftsForAmount[i].EndTime);

                previous = DateTime.Parse(previousStartNote.Date);
                current = DateTime.Parse(currentEndNote.Date);

                TimeSpan difference = new TimeSpan();
                difference = previous - current;
                if (difference.TotalHours > 24)
                {
                    if (previousEndNote.Date == null)
                    {
                        totalHours = (DateTime.Now.TimeOfDay - DateTime.Parse(previousStartNote.Date).TimeOfDay).TotalHours + totalHours;
                    }
                    else
                    {
                        totalHours = (DateTime.Parse(previousEndNote.Date).TimeOfDay - DateTime.Parse(previousStartNote.Date).TimeOfDay).TotalHours + totalHours;
                    }
                    return totalHours;
                }
                else
                {
                    TimeSpan amountOfTime = new TimeSpan();
                    if (previousEndNote.Date == null)
                    {
                        amountOfTime = DateTime.Now - DateTime.Parse(previousStartNote.Date);
                    }
                    else
                    {
                        amountOfTime = DateTime.Parse(previousEndNote.Date) - DateTime.Parse(previousStartNote.Date);
                    }
                    totalHours = totalHours + amountOfTime.TotalHours;
                }
            }
            return totalHours;
        }

        internal void InsertUserDrives(DriveTable drive)
        {
            //DriveTable drive = new DriveTable();
            //drive.ShiftKey = fullDrive.ShiftKey;
            //db.Insert(drive);

            //drive.VehicleKey = fullDrive.VehicleKey;
            //db.Update(drive);

            //drive.StartNoteKey = fullDrive.StartNoteKey;
            //db.Update(drive);

            //drive.EndNoteKey = fullDrive.EndNoteKey;
            //db.Update(drive);

            //drive.ActiveVehicle = fullDrive.ActiveVehicle;
            //db.Update(drive);

            db.Insert(drive);
        }

        internal List<ShiftTable> GetShiftsWeek(DateTime selectedDate)
        {
            List<ShiftTable> returnShifts = new List<ShiftTable>();
            int daysWithoutShifts = 0;
            while ((returnShifts.Count < 8))
            {
                List<ShiftTable> listOfShifts = new List<ShiftTable>();
                string dateString = selectedDate.Day + "/" + selectedDate.Month + "/" + selectedDate.Year;
                listOfShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartTime] LIKE '" + dateString + "%'");
                foreach (ShiftTable shift in listOfShifts)
                {
                    returnShifts.Add(shift);
                }
                if (listOfShifts.Count == 0)
                {
                    daysWithoutShifts++;
                }
                else
                {
                    daysWithoutShifts = 0;
                }
                if (daysWithoutShifts == 8)
                {
                    break;
                }
                selectedDate = selectedDate.AddDays(-1);
            }
            return returnShifts;
        }

        internal void InsertUserNotes(NoteTable note)
        {
            db.Insert(note);
        }

        internal int HoursTillReset()
        {
            //TODO: Code to retrieve time since last shift (Once it hits 24 hours, 70 hour a week will reset
            List<ShiftTable> listOfShifts = new List<ShiftTable>();
            listOfShifts = db.Query<ShiftTable>("SELECT * FROM[ShiftTable] ORDER BY Key DESC LIMIT 2");
            ShiftTable lastShift = new ShiftTable();
            if (listOfShifts.Count == 0)
            {
                return -1;
            }
            lastShift = listOfShifts[0];
            DateTime dateNow = new DateTime();
            DateTime dateOnLastShift = new DateTime();
            dateNow = DateTime.Now;

            NoteTable shiftNote = db.Get<NoteTable>(lastShift.StopShiftNoteId);

            string dateLastShiftString = shiftNote.Date;
            if (dateLastShiftString == null)
            {
                //Shift is still occuring, thus 0
                return 0;
            }
            dateOnLastShift = DateTime.Parse(dateLastShiftString);
            TimeSpan time = new TimeSpan();
            time = dateNow - dateOnLastShift;
            if (time.TotalDays >= 1)
            {
                //Have rested for longer/as long as 24 hours
                return -2;
            }
            int timeSinceReset = Int32.Parse(time.ToString().Remove(2));
            return timeSinceReset;
        }

        internal int InsertUserShifts(ShiftTable shift)
        {
            db.Insert(shift);

            return shift.Key;
        }

        internal void InsertUserBreaks(BreakTable breakTable)
        {
            db.Insert(breakTable);
        }

        internal List<DriveTable> GetUsedVehicles(ShiftTable currentShift)
        {
            List<DriveTable> usedVehicles = new List<DriveTable>();
            usedVehicles = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ShiftKey] == " + currentShift.Key + "");
            return usedVehicles;
        }


        internal void AddAmendments(List<AmendmentTable> listOfAmendments, ShiftTable currentShift = null, DriveTable currentVehicleInUse = null, List<NoteTable> listOfNotes = null, BreakTable currentBreak = null, NoteTable currentNote = null)
        {
            if (currentNote != null)
            {
                db.Update(currentNote);
            }

            if (currentBreak != null)
            {
                db.Update(currentBreak);
            }
            if (listOfNotes != null)
            {
                foreach (NoteTable note in listOfNotes)
                {
                    db.Update(note);
                }
            }
            if (currentShift != null)
            {
                db.Update(currentShift);
            }
            if (currentVehicleInUse != null)
            {
                db.Update(currentVehicleInUse);
            }

            foreach (AmendmentTable amendment in listOfAmendments)
            {
                db.Insert(amendment);
            }
        }

        internal void UserOffline(UserTable user)
        {
            var tbl = db.GetTableInfo("UserOffline");

            if (tbl == null)
                db.CreateTable<UserOffline>();

            UserOffline offline = new UserOffline();

            offline.UserKey = user.Id;

            db.Insert(offline);
        }

        internal List<ShiftTable> GetShifts(DateTime selectedDate)
        {
            List<ShiftTable> listOfShiftsToAdd = new List<ShiftTable>();
            List<ShiftTable> listOfShifts = new List<ShiftTable>();
            List<DateTime> listOfDates = new List<DateTime>();
            DateTime fromDate = selectedDate.AddDays(-7);
            selectedDate = selectedDate.AddDays(1);
            while (fromDate != selectedDate)
            {
                listOfDates.Add(fromDate);
                fromDate = fromDate.AddDays(1);
            }

            foreach (DateTime date in listOfDates)
            {
                string dateString = date.Day + "/" + date.Month + "/" + date.Year;
                listOfShiftsToAdd = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartTime] LIKE '" + dateString + "%'");
                if (listOfShiftsToAdd.Count != 0)
                {
                    listOfShifts.Add(listOfShiftsToAdd[0]);
                }
            }

            return listOfShifts;
        }

        internal async void SaveNote(string note, DateTime date, string location, int huboEntry = 0)
        {
            NoteTable newNote = new NoteTable();
            newNote.Note = note;
            newNote.StandAloneNote = true;
            newNote.Date = date.ToString();
            newNote.Location = location;
            //newNote.Time = time.ToString();
            newNote.Hubo = huboEntry;
            Geolocation geoLocation = new Geolocation();
            geoLocation = await GetLatAndLong();
            newNote.Longitude = geoLocation.Longitude;
            newNote.Latitude = geoLocation.Latitude;
            List<ShiftTable> currentShiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if ((currentShiftList.Count == 0) || (currentShiftList.Count > 1))
            {
                await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "UNABLE TO GET CURRENT SHIFT, THUS NOTE NOT SAVED", Resource.DisplayAlertOkay);
            }
            else
            {
                ShiftTable currentShift = currentShiftList[0];
                newNote.ShiftKey = currentShift.Key;
                db.Insert(newNote);
            }
        }

        internal List<NoteTable> GetNotesFromVehicles(List<DriveTable> listUsedVehicles)
        {
            List<NoteTable> notes = new List<NoteTable>();

            foreach (DriveTable driveItem in listUsedVehicles)
            {
                NoteTable startNote = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [Key] = " + driveItem.StartNoteKey)[0];
                NoteTable endNote = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [Key] = " + driveItem.EndNoteKey)[0];

                notes.Add(startNote);
                notes.Add(endNote);
            }

            return notes;
        }

        internal async void SaveNoteFromVehicle(string note, DateTime date, string location, int huboEntry, int vehicleKey)
        {

            List<ShiftTable> listOfShifts = new List<ShiftTable>();
            listOfShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if ((listOfShifts.Count == 0) || (listOfShifts.Count > 1))
            {
                await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "UNABLE TO GET CURRENT SHIFT, THUS NOTE NOT SAVED", Resource.DisplayAlertOkay);
            }

            else
            {
                NoteTable newNote = new NoteTable();
                newNote.Date = date.ToString();
                newNote.Location = location;
                newNote.Note = note;
                newNote.Hubo = huboEntry;
                newNote.ShiftKey = listOfShifts[0].Key;
                Geolocation geoLocation = new Geolocation();
                geoLocation = await GetLatAndLong();
                newNote.Longitude = geoLocation.Longitude;
                newNote.Latitude = geoLocation.Latitude;
                db.Insert(newNote);

                List<NoteTable> listOfNotes = new List<NoteTable>();
                listOfNotes = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [Date] == '" + date + "' AND [Location] == '" + location + "' AND [Note] == '" + note + "' AND [ShiftKey] == " + listOfShifts[0].Key);
                if (VehicleInUse())
                {
                    //TODO: CODE TO Turn off vehicle in use
                    List<DriveTable> listOfVehiclesInUse = new List<DriveTable>();
                    listOfVehiclesInUse = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ActiveVehicle] == 1");
                    if (listOfVehiclesInUse.Count == 0)
                    {
                        await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "CANNOT RETRIEVE VEHICLE IN USE", Resource.DisplayAlertOkay);
                        return;
                    }
                    DriveTable vehicleInUse = new DriveTable();
                    vehicleInUse = listOfVehiclesInUse[0];
                    vehicleInUse.ActiveVehicle = false;
                    vehicleInUse.EndNoteKey = listOfNotes[0].Key;
                    vehicleInUse.ShiftKey = listOfShifts[0].Key;
                    vehicleInUse.VehicleKey = vehicleKey;
                    db.Update(vehicleInUse);
                }

                else
                {
                    //TODO: Code to turn on vehicle in use
                    DriveTable newVehicleInUse = new DriveTable();
                    newVehicleInUse.ActiveVehicle = true;
                    newVehicleInUse.ShiftKey = listOfShifts[0].Key;
                    newVehicleInUse.StartNoteKey = listOfNotes[0].Key;
                    newVehicleInUse.VehicleKey = vehicleKey;
                    db.Insert(newVehicleInUse);
                }
            }
            MessagingCenter.Send<string>("UpdateActiveVehicle", "UpdateActiveVehicle");
            MessagingCenter.Send<string>("UpdateVehicleInUse", "UpdateVehicleInUse");

        }

        internal async void CollectGeolocation(int driveKey)
        {
            GeolocationTable geoInsert = new GeolocationTable();
            Geolocation geolocation = new Geolocation();

            geolocation = await GetLatAndLong();

            geoInsert.DriveKey = driveKey;
            geoInsert.TimeStamp = DateTime.Now.ToString();
            geoInsert.latitude = geolocation.Latitude;
            geoInsert.Longitude = geolocation.Longitude;

            db.Insert(geoInsert);

            List<GeolocationTable> testInsert = db.Query<GeolocationTable>("SELECT * FROM [GeolocationTable]");

            await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, testInsert[testInsert.Count - 1].TimeStamp, Resource.DisplayAlertOkay);
        }

        internal async void SaveNoteFromBreak(string note, DateTime date, string location, int huboEntry)
        {
            NoteTable newNote = new NoteTable();
            newNote.Note = note;
            newNote.Date = date.ToString();
            newNote.Location = location;
            newNote.Hubo = huboEntry;
            Geolocation geoLocation = new Geolocation();
            geoLocation = await GetLatAndLong();
            newNote.Longitude = geoLocation.Longitude;
            newNote.Latitude = geoLocation.Latitude;
            List<ShiftTable> currentShiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if ((currentShiftList.Count == 0) || (currentShiftList.Count > 1))
            {
                await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "UNABLE TO GET CURRENT SHIFT, THUS NOTE NOT SAVED", Resource.DisplayAlertOkay);
            }
            else
            {
                ShiftTable currentShift = currentShiftList[0];
                newNote.ShiftKey = currentShift.Key;
                db.Insert(newNote);
                List<NoteTable> notes = new List<NoteTable>();
                notes = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [Note] == '" + note + "' AND [ShiftKey] == " + currentShift.Key + "");
                if ((notes.Count == 0) || (notes.Count > 1))
                {
                    await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "UNABLE TO GET CURRENT NOTE", Resource.DisplayAlertOkay);
                    return;
                }
                List<BreakTable> listBreaks = new List<BreakTable>();
                listBreaks = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ActiveBreak] == 1");
                int noteKeyForBreak = notes[0].Key;
                //No active breaks, so starting break.
                if (listBreaks.Count == 0)
                {
                    StartBreak(noteKeyForBreak);
                }
                //Active Break, so stopping break.
                else if (listBreaks.Count == 1)
                {
                    StopBreak(noteKeyForBreak);
                }
            }
        }

        internal List<NoteTable> GetNotes(List<BreakTable> listOfBreaks)
        {
            List<NoteTable> listOfNotes = new List<NoteTable>();
            if (listOfBreaks.Count != 0 && listOfBreaks != null)
            {
                foreach (BreakTable breakItem in listOfBreaks)
                {
                    List<NoteTable> tempList = new List<NoteTable>();
                    tempList = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [Key] ==" + breakItem.StartNoteKey + " OR [Key] == " + breakItem.StopNoteKey);
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
            if (list.Count > 0)
            {
                return true;
            }
            return false;
        }

        internal bool VehicleInUse()
        {
            List<DriveTable> listVehiclesInUse = new List<DriveTable>();
            listVehiclesInUse = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ActiveVehicle] == 1");
            if (listVehiclesInUse.Count == 0)
            {
                return false;
            }
            else if (listVehiclesInUse.Count > 1)
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "MORE THAN ONE ACTIVE VEHICLE DETECTED", Resource.DisplayAlertOkay);
                return false;
            }
            return true;
        }

        internal async void StopVehicleInUse(string huboEntry)
        {
            List<DriveTable> listVehiclesInUse = new List<DriveTable>();
            NoteTable note = new NoteTable();
            listVehiclesInUse = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ActiveVehicle]==1");
            if ((listVehiclesInUse.Count == 0) || (listVehiclesInUse.Count > 1))
            {
                await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "MORE THAN ONE VEHICLE FOUND", Resource.DisplayAlertOkay);
                return;
            }
            DriveTable vehicleInUse = new DriveTable();
            vehicleInUse = listVehiclesInUse[0];
            vehicleInUse.ActiveVehicle = false;

            note.Date = DateTime.Now.ToString();
            note.ShiftKey = listVehiclesInUse[0].ShiftKey;
            note.Hubo = int.Parse(huboEntry);
            note.Note = "Drive Start";

            Geolocation location = new Geolocation();
            location = await GetLatAndLong();
            note.Latitude = location.Latitude;
            note.Longitude = location.Longitude;
            db.Insert(note);

            NoteTable currentNote = db.Query<NoteTable>("SELECT * FROM [NoteTable] ORDER BY [Key] DESC LIMIT 1")[0];

            vehicleInUse.EndNoteKey = currentNote.Key;

            db.Update(vehicleInUse);

            MessagingCenter.Send<string>("UpdateActiveVehicle", "UpdateActiveVehicle");
            MessagingCenter.Send<string>("UpdateVehicleInUse", "UpdateVehicleInUse");

            return;
        }

        internal bool NoBreaksActive()
        {
            List<BreakTable> listOfBreaks = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ActiveBreak] == 1");
            if (listOfBreaks.Count > 0)
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

        internal async void SetVehicleInUse(int key, string huboEntry)
        {
            DriveTable vehicleToUse = new DriveTable();
            NoteTable note = new NoteTable();
            List<ShiftTable> listOfActiveShifts = new List<ShiftTable>();

            listOfActiveShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift]==1");
            if ((listOfActiveShifts.Count == 0) || (listOfActiveShifts.Count > 1))
            {
                await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "UNABLE TO RETRIEVE ACTIVE SHIFT", Resource.DisplayAlertOkay);
                return;
            }
            vehicleToUse.ShiftKey = listOfActiveShifts[0].Key;
            vehicleToUse.VehicleKey = key;
            vehicleToUse.ActiveVehicle = true;

            note.Date = DateTime.Now.ToString();
            note.ShiftKey = listOfActiveShifts[0].Key;
            note.Hubo = int.Parse(huboEntry);
            note.Note = "Drive Start";

            Geolocation location = new Geolocation();
            location = await GetLatAndLong();
            note.Latitude = location.Latitude;
            note.Longitude = location.Longitude;
            db.Insert(note);

            NoteTable currentNote = db.Query<NoteTable>("SELECT * FROM [NoteTable] ORDER BY [Key] DESC LIMIT 1")[0];

            vehicleToUse.StartNoteKey = currentNote.Key;

            db.Insert(vehicleToUse);

            MessagingCenter.Send<string>("UpdateActiveVehicle", "UpdateActiveVehicle");
            MessagingCenter.Send<string>("UpdateVehicleInUse", "UpdateVehicleInUse");

        }

        internal bool CheckActiveShift()
        {
            List<ShiftTable> shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if (shiftList.Count == 0)
            {
                return false;
            }
            else if (shiftList.Count == 1)
            {
                return true;
            }
            Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "MORE THAN ONE ACTIVE SHIFT DISCOVERED", Resource.DisplayAlertOkay);
            return false;

        }

        internal bool CheckOnBreak()
        {
            List<BreakTable> breakList = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ActiveBreak] == 1");
            if (breakList.Count == 0)
            {
                return false;
            }
            else if (breakList.Count == 1)
            {
                return true;
            }

            Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "MORE THAN ONE ACTIVE BREAK DISCOVERED", Resource.DisplayAlertOkay);
            return false;
        }

        internal List<NoteTable> GetNotesFromVehicle(DriveTable currentVehicleInUse)
        {
            List<NoteTable> listOfNotes = new List<NoteTable>();
            listOfNotes = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [Key] == " + currentVehicleInUse.StartNoteKey + " OR [Key] == " + currentVehicleInUse.EndNoteKey + "");
            return listOfNotes;
        }

        internal VehicleTable GetCurrentVehicle()
        {
            //Retrieve currently used vehicle
            DriveTable currentVehicleInUse;
            List<DriveTable> currentVehicleInUseList = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ActiveVehicle] == 1");
            currentVehicleInUse = currentVehicleInUseList[0];

            //Retrieve vehicle details using currentvehicles key
            VehicleTable vehicle;
            List<VehicleTable> vehicleList = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [Key] == " + currentVehicleInUse.VehicleKey + "");
            vehicle = vehicleList[0];

            return vehicle;
        }

        internal bool VehicleActive()
        {
            List<DriveTable> currentVehicles = new List<DriveTable>();
            currentVehicles = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ActiveVehicle] == 1");
            if (currentVehicles.Count == 0)
            {
                return false;
            }
            else if (currentVehicles.Count == 1)
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
            currentBreak.ActiveBreak = false;
            currentBreak.StopNoteKey = noteKey;
            db.Update(currentBreak);
            return true;
        }

        internal void InsertVehicle(VehicleTable vehicleToAdd)
        {
            db.Insert(vehicleToAdd);
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
            if (CheckActiveShiftIsCorrect(activeShifts))
            {
                newBreak.ShiftKey = activeShifts[0].Key;
                newBreak.StartTime = DateTime.Now.ToString();
                newBreak.StartNoteKey = noteKey;
                newBreak.ActiveBreak = true;
                db.Insert(newBreak);
                return true;
            }
            return false;

        }

        private async Task<Geolocation> GetLatAndLong()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            Geolocation results = new Geolocation();

            try
            {
                var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

                results.Longitude = position.Longitude;
                results.Latitude = position.Latitude;
                return results;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, e.ToString(), Resource.DisplayAlertOkay);
                return results;
            }
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
            List<DriveTable> activeVehicles = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ActiveVehicle] == 1");
            foreach (DriveTable vehicle in activeVehicles)
            {
                if (currentVehicle.Key == vehicle.VehicleKey)
                {
                    vehicle.ActiveVehicle = true;
                    db.Update(vehicle);
                }
            }

        }
        internal void SetVehicleInactive()
        {
            List<DriveTable> activeVehicles = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ActiveVehicle] == 1");
            foreach (DriveTable vehicle in activeVehicles)
            {
                vehicle.ActiveVehicle = false;
                db.Update(vehicle);
            }
        }

        internal void UpdateVehicleInfo(VehicleTable editedVehicle)
        {
            db.Update(editedVehicle);
        }

        internal void Logout()
        {
            db.Query<UserTable>("DELETE FROM [UserTable]");
        }

        internal async void StartShift(string note, DateTime date, int hubo)
        {
            ShiftTable newShift = new ShiftTable();

            NoteTable newNote = new NoteTable();
            newNote.Date = date.ToString();
            newNote.Hubo = hubo;
            newNote.Note = note;
            newNote.StandAloneNote = false;

            newShift.ActiveShift = true;
            db.Insert(newShift);

            List<ShiftTable> activeShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if (CheckActiveShiftIsCorrect(activeShifts))
            {

                ShiftTable shift = activeShifts[0];
                UserTable user = db.Query<UserTable>("SELECT * FROM [UserTable]")[0];
                DriveTable drive = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ShiftKey] = " + shift.Key + " LIMIT 1")[0];
                VehicleTable vehicle = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [VehicleKey] = " + drive.VehicleKey + " LIMIT 1")[0];

                Geolocation geoLocation = new Geolocation();
                geoLocation = await GetLatAndLong();
                newNote.ShiftKey = shift.Key;
                newNote.Latitude = geoLocation.Latitude;
                newNote.Longitude = geoLocation.Longitude;
                db.Insert(newNote);

                RestAPI = new RestService();
                var result = await RestAPI.QueryShift(shift, false, newNote, user.Id, vehicle.CompanyId);

                if (result > 0)
                {
                    shift.ServerKey = result;
                    db.Update(shift);
                }
                else
                {
                    var tbl = db.GetTableInfo("ShiftOffline");

                    if (tbl == null)
                        db.CreateTable<ShiftOffline>();

                    ShiftOffline offline = new ShiftOffline();

                    offline.ShiftKey = shift.Key;

                    db.Insert(offline);
                }
                MessagingCenter.Send<string>("ShiftStart", "ShiftStart");
            }
        }

        internal ShiftTable GetCurrentShift()
        {
            List<ShiftTable> activeShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");

            if (CheckActiveShiftIsCorrect(activeShifts))
            {
                ShiftTable activeShift = activeShifts[0];
                return activeShift;
            }
            return null;
        }

        internal DriveTable GetCurrentDriveShift()
        {
            List<DriveTable> activeDrives = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ActiveVehicle] == TRUE LIMIT 1");

            DriveTable activeDrive = activeDrives[0];
            return activeDrive;
        }

        internal async void StopShift(string note, DateTime date, int hubo)
        {
            //Get current shift
            List<ShiftTable> activeShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if (CheckActiveShiftIsCorrect(activeShifts))
            {
                //TODO: Code to check that this shift had a vehicle assigned to it
                List<DriveTable> listOfUsedVehicles = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ShiftKey] == " + activeShifts[0].Key + "");
                if (listOfUsedVehicles.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Please select a vehicle before ending your shift", Resource.DisplayAlertOkay);
                }
                ShiftTable activeShift = activeShifts[0];
                activeShift.EndTime = date.ToString();
                activeShift.ActiveShift = false;
                db.Update(activeShift);

                Geolocation location = new Geolocation();
                location = await GetLatAndLong();

                NoteTable newNote = new NoteTable();
                newNote.Note = note;
                newNote.Date = date.ToString();
                newNote.Hubo = hubo;
                newNote.Latitude = location.Latitude;
                newNote.Longitude = location.Longitude;
                newNote.StandAloneNote = false;
                db.Insert(newNote);

                if (activeShift.ServerKey > 0)
                {
                    RestAPI = new RestService();
                    var result = await RestAPI.QueryShift(activeShift, true, newNote, 0, 0);

                    if (result < 0)
                    {
                        var tbl = db.GetTableInfo("ShiftOffline");

                        if (tbl == null)
                        {
                            db.CreateTable<ShiftOffline>();

                            ShiftOffline offline = new ShiftOffline();

                            offline.ShiftKey = activeShift.Key;
                            db.Insert(offline);
                        }
                        else
                        {
                            List<ShiftOffline> checkOffline = db.Query<ShiftOffline>("SELECT [ShiftKey] FROM [ShiftOffline]");
                            int num = 0;

                            foreach (ShiftOffline item in checkOffline)
                            {
                                if (item.ShiftKey == activeShift.Key)
                                {
                                    num++;
                                }
                            }

                            if (num == 0)
                            {
                                ShiftOffline offline = new ShiftOffline();

                                offline.ShiftKey = activeShift.Key;
                                db.Insert(offline);
                            }
                        }
                    }
                    MessagingCenter.Send<string>("ShiftEnd", "ShiftEnd");
                }
                await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to end shift", Resource.DisplayAlertOkay);
            }
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
            DriveTable driveTable = new DriveTable();
            NoteTable noteTableStart = new NoteTable();
            NoteTable noteTableEnd = new NoteTable();

            shiftTable.ActiveShift = false;
            db.Insert(shiftTable);

            List<ShiftTable> currentShiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] ORDER BY [Key] DESC LIMIT 1");
            if (currentShiftList.Count < 1 && currentShiftList.Count > 1)
            {
                return -1;
            }

            ShiftTable shiftKey = currentShiftList[0];
            driveTable.ShiftKey = shiftKey.Key;
            noteTableStart.ShiftKey = shiftKey.Key;
            noteTableEnd.ShiftKey = shiftKey.Key;

            noteTableStart.Note = "Start Shift";
            noteTableStart.Date = startDate.ToString();
            noteTableStart.Hubo = int.Parse(huboStart);
            noteTableStart.Location = locationStart;
            noteTableStart.StandAloneNote = false;
            db.Insert(noteTableStart);

            List<NoteTable> startNoteList = db.Query<NoteTable>("SELECT * FROM [NoteTable] ORDER BY [Key] DESC LIMIT 1");
            shiftKey.StartShiftNoteId = startNoteList[0].Key;

            noteTableEnd.Note = "End Shift";
            noteTableEnd.Date = endDate.ToString();
            noteTableEnd.Hubo = int.Parse(huboEnd);
            noteTableEnd.Location = locationEnd;
            noteTableEnd.StandAloneNote = false;
            db.Insert(noteTableEnd);

            List<NoteTable> EndNoteList = db.Query<NoteTable>("SELECT * FROM [NoteTable] ORDER BY [Key] DESC LIMIT 1");
            shiftKey.StartShiftNoteId = startNoteList[0].Key;

            db.Update(shiftKey);

            List<NoteTable> currentNoteList = db.Query<NoteTable>("SELECT * FROM [NoteTable] ORDER BY [Key] DESC LIMIT 2");
            if (currentNoteList.Count < 2 && currentNoteList.Count > 2)
            {
                db.Delete(shiftKey.Key);
                return -1;
            }

            NoteTable startNoteKey = currentNoteList[1];
            NoteTable endNoteKey = currentNoteList[0];
            driveTable.StartNoteKey = startNoteKey.Key;
            driveTable.EndNoteKey = endNoteKey.Key;

            driveTable.VehicleKey = vehicleKey.Key;
            driveTable.ActiveVehicle = false;

            db.Insert(driveTable);

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
            newBreak.ActiveBreak = false;
            newBreak.StartNoteKey = currentNoteList[1].Key;
            newBreak.StopNoteKey = currentNoteList[0].Key;

            db.Insert(newBreak);
        }

        internal void SaveVehicleInUse(int shiftKey, DateTime start, DateTime end, VehicleTable vehicleKey, string startHubo, string endHubo, string startLocation, string endLocation)
        {
            NoteTable vehicleStartNote = new NoteTable();
            NoteTable vehicleEndNote = new NoteTable();
            DriveTable vehicleInUse = new DriveTable();

            vehicleStartNote.Note = "Drive Start";
            vehicleStartNote.Date = start.ToString();
            vehicleStartNote.ShiftKey = shiftKey;
            vehicleStartNote.Hubo = int.Parse(startHubo);
            vehicleStartNote.Location = startLocation;
            vehicleStartNote.StandAloneNote = false;

            vehicleEndNote.Note = "Drive End";
            vehicleEndNote.Date = end.ToString();
            vehicleEndNote.ShiftKey = shiftKey;
            vehicleEndNote.Hubo = int.Parse(endHubo);
            vehicleEndNote.Location = endLocation;
            vehicleEndNote.StandAloneNote = false;

            db.Insert(vehicleStartNote);
            db.Insert(vehicleEndNote);

            List<NoteTable> currentNoteList = db.Query<NoteTable>("SELECT * FROM [NoteTable] ORDER BY [Key] DESC LIMIT 2");

            vehicleInUse.ShiftKey = shiftKey;
            vehicleInUse.VehicleKey = vehicleKey.Key;
            vehicleInUse.ActiveVehicle = false;
            vehicleInUse.StartNoteKey = currentNoteList[1].Key;
            vehicleInUse.EndNoteKey = currentNoteList[0].Key;

            db.Insert(vehicleInUse);
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

        internal List<ExportShift> GetExportShift()
        {
            ExportShift exportData = new ExportShift();

            List<ShiftTable> shiftList = new List<ShiftTable>();
            List<ExportShift> exportList = new List<ExportShift>();

            shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartTime] <= (SELECT DATE('now', '-7 day'))");

            foreach (ShiftTable shiftData in shiftList)
            {
                exportData.shiftCode = shiftData.Key.ToString();
                exportData.activeShift = shiftData.ActiveShift.ToString();

                exportList.Add(exportData);
            }

            return exportList;
        }

        internal List<ExportBreak> GetExportBreak()
        {
            ExportBreak exportData = new ExportBreak();

            List<BreakTable> breakList = new List<BreakTable>();
            List<ShiftTable> shiftList = new List<ShiftTable>();
            List<ExportBreak> exportList = new List<ExportBreak>();

            shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartTime] <= (SELECT DATE('now', '-7 day'))");

            foreach (ShiftTable shiftData in shiftList)
            {
                breakList = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ShiftKey] = " + shiftData.Key);

                foreach (BreakTable breakData in breakList)
                {
                    List<NoteTable> breakStartNotesList = db.Query<NoteTable>("SELECT [Note], [Hubo], [Location] FROM [NoteTable] WHERE [Key] = " + breakData.StartNoteKey + " AND [ShiftKey] = " + shiftData.Key);
                    List<NoteTable> breakEndNotesList = db.Query<NoteTable>("SELECT [Note], [Hubo], [Location] FROM [NoteTable] WHERE [Key] = " + breakData.StopNoteKey + " AND [ShiftKey] = " + shiftData.Key);

                    exportData.shiftCode = breakData.Key.ToString();
                    exportData.breakStart = breakData.StartTime;
                    exportData.breakEnd = breakData.EndTime;
                    exportData.activeBreak = breakData.ActiveBreak.ToString();
                    exportData.breakDetails = breakStartNotesList[0].Note;
                    exportData.breakStartHubo = breakStartNotesList[0].Hubo.ToString();
                    exportData.breakEndHubo = breakEndNotesList[0].Hubo.ToString();
                    exportData.breakStartLocation = breakStartNotesList[0].Location;
                    exportData.breakEndLocation = breakEndNotesList[0].Location;

                    exportList.Add(exportData);
                }
            }

            if (exportList.Count > 0)
            {
                return exportList;
            }
            else
            {
                return null;
            }
        }

        internal List<ExportNote> GetExportNote()
        {
            ExportNote exportData = new ExportNote();

            List<NoteTable> noteList = new List<NoteTable>();
            List<ShiftTable> shiftList = new List<ShiftTable>();
            List<ExportNote> exportList = new List<ExportNote>();

            shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartTime] <= (SELECT DATE('now', '-7 day'))");

            foreach (ShiftTable shiftData in shiftList)
            {
                noteList = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [ShiftKey] = " + shiftData.Key + " AND [StandAloneNote] = 1");

                foreach (NoteTable noteData in noteList)
                {
                    exportData.shiftCode = noteData.Key.ToString();
                    exportData.noteTime = noteData.Date;
                    exportData.noteDetails = noteData.Note;
                    exportData.noteHubo = noteData.Hubo.ToString();
                    exportData.noteLocation = noteData.Location;

                    exportList.Add(exportData);
                }
            }

            if (exportList.Count > 0)
            {
                return exportList;
            }
            else
            {
                return null;
            }
        }

        internal List<ExportVehicle> GetExportVehicle()
        {
            ExportVehicle exportData = new ExportVehicle();

            List<DriveTable> vehicleInUseList = new List<DriveTable>();
            List<VehicleTable> vehicleList = new List<VehicleTable>();
            List<ShiftTable> shiftList = new List<ShiftTable>();
            List<ExportVehicle> exportList = new List<ExportVehicle>();
            shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartTime] <= (SELECT DATE('now', '-7 day'))");

            foreach (ShiftTable shiftData in shiftList)
            {
                vehicleInUseList = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ShiftKey] = " + shiftData.Key);

                foreach (DriveTable vehicleInUseData in vehicleInUseList)
                {
                    vehicleList = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [Key] = " + vehicleInUseData.VehicleKey);
                    foreach (VehicleTable vehicleData in vehicleList)
                    {
                        List<NoteTable> locationStartNotesList = db.Query<NoteTable>("SELECT [Location] FROM [NoteTable] WHERE [Key] = " + vehicleInUseData.StartNoteKey + " AND [ShiftKey] = " + shiftData.Key);
                        List<NoteTable> locationEndNotesList = db.Query<NoteTable>("SELECT [Location] FROM [NoteTable] WHERE [Key] = " + vehicleInUseData.EndNoteKey + " AND [ShiftKey] = " + shiftData.Key);

                        exportData.vehicleMakeModel = vehicleData.MakeModel;
                        exportData.vehicleRego = vehicleData.Registration;
                        exportData.vehicleCompany = vehicleData.CompanyId.ToString();
                        exportData.currentVehicle = vehicleInUseData.ActiveVehicle.ToString();

                        exportData.huboStart = locationStartNotesList[0].Hubo.ToString();
                        exportData.huboEnd = locationEndNotesList[0].Hubo.ToString();
                        exportData.startLocation = locationStartNotesList[0].Location;
                        if (locationEndNotesList.Count != 0)
                        {
                            exportData.endLocation = locationEndNotesList[0].Location;
                        }

                        exportList.Add(exportData);
                    }
                }
            }

            return exportList;
        }
    }
}
