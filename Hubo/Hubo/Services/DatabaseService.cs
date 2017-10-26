// <copyright file="DatabaseService.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Acr.UserDialogs;
    using Microsoft.ProjectOxford.Vision.Contract;
    using Plugin.Connectivity;
    using Plugin.Media;
    using Plugin.Media.Abstractions;
    using SQLite.Net;
    using Xamarin.Forms;

    public class DatabaseService
    {
        private SQLiteConnection db;
        private RestService restAPI;

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
            db.CreateTable<GeolocationTable>();
            db.CreateTable<LicenceTable>();
            db.CreateTable<ShiftOffline>();
            db.CreateTable<DriveOffline>();
            db.CreateTable<BreakOffline>();
            db.CreateTable<VehicleOffline>();
            db.CreateTable<NotificationTable>();
            db.CreateTable<DayShiftTable>();
            db.CreateTable<DayShiftOffline>();
        }

        public IEnumerable<string> Combinations(string input, char initialChar, string replacementChar)
        {
            var head = input[0] == initialChar // Do I have a `0`?
                ? new[] { initialChar.ToString(), replacementChar } // If so output both `"0"` & `"o"`
                : new[] { input[0].ToString() }; // Otherwise output the current character

            var tails = input.Length > 1 // Is there any more string?
                ? Combinations(input.Substring(1), initialChar, replacementChar) // Yes, recursively compute
                : new[] { string.Empty }; // Otherwise, output empty string

            // Now, join it up and return
            return
                from h in head
                from t in tails
                select h + t;
        }

        internal ShiftTable GetLastShift()
        {
            List<ShiftTable> listOfShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable]");
            return listOfShifts[listOfShifts.Count - 1];
        }

        internal List<ShiftTable> GetDayShifts(DateTime date)
        {
            List<ShiftTable> shifts = new List<ShiftTable>();

            shifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartDate] LIKE '%" + date.ToString("yyyy-MM-dd") + "%'");

            return shifts;
        }

        internal string GetUserToken()
        {
            List<UserTable> listUsers = db.Query<UserTable>("SELECT * FROM [UserTable]");
            string token = string.Empty;

            if (listUsers.Count != 0)
            {
                token = listUsers[0].Token;
            }

            return token;
        }

        internal List<LoadTextTable> GetLoadingText()
        {
            List<LoadTextTable> loadList = db.Query<LoadTextTable>("SELECT * FROM [LoadTextTable]");
            return loadList;
        }

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

        internal List<VehicleTable> LoadVehicle()
        {
            List<VehicleTable> vehicleDetails = db.Query<VehicleTable>("SELECT * FROM [VehicleTable]");
            return vehicleDetails;
        }

        internal void HideTip(string tipName)
        {
            List<TipTable> listOfTips = db.Query<TipTable>("SELECT * FROM [TipTable] WHERE [TipName] == '" + tipName + "'");
            TipTable tip = listOfTips[0];
            tip.ActiveTip = 0;
            db.Update(tip);
        }

        internal bool ShowTip(string tipName)
        {
            List<TipTable> listOfTips = db.Query<TipTable>("SELECT * FROM [TipTable] WHERE [TipName] == '" + tipName + "'");
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

        internal string GetShiftTimes()
        {
            List<ShiftTable> activeShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if (activeShifts.Count == 0 || activeShifts.Count > 1)
            {
                return null;
            }

            ShiftTable currentShift = activeShifts[0];

            DateTime startTime = default(DateTime);
            DateTime endTime = default(DateTime);

            startTime = DateTime.Parse(currentShift.StartDate);
            endTime = startTime + TimeSpan.FromHours(14);

            return endTime.ToString("h:mm tt");
        }

        internal VehicleTable GetVehicleById(int vehicleKey)
        {
            List<VehicleTable> listVehicles = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE Key = " + vehicleKey.ToString());
            return listVehicles[0];
        }

        internal VehicleTable GetVehicleByServerId(int vehicleKey)
        {
            List<VehicleTable> listVehicles = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE ServerKey = " + vehicleKey.ToString());
            return listVehicles[0];
        }

        internal List<ShiftTable> GetEditableShifts()
        {
            List<ShiftTable> shifts = new List<ShiftTable>();
            DateTime date = default(DateTime);
            date = DateTime.Now - TimeSpan.FromHours(24);

            shifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartDate] < '%" + date.ToString(Resource.DateFormat) + "%'");

            return shifts;
        }

        internal List<LicenceTable> GetLicenceInfo(int driverId)
        {
            List<LicenceTable> licenceList = new List<LicenceTable>();
            licenceList = db.Query<LicenceTable>("SELECT * FROM [LicenceTable] WHERE [DriverId] = " + driverId);

            return licenceList;
        }

        internal List<CompanyTable> GetCompanyInfo(int driverId)
        {
            List<CompanyTable> companyList = new List<CompanyTable>();
            companyList = db.Query<CompanyTable>("SELECT * FROM [CompanyTable] WHERE [DriverId] = " + driverId);

            return companyList;
        }

        internal void InsertUserVehicles(VehicleTable vehicle)
        {
            db.Insert(vehicle);
        }

        internal bool InsertUserCompany(CompanyTable company)
        {
            if (db.Insert(company) != 0)
            {
                return true;
            }

            return false;
        }

        internal void VehicleOffine(VehicleTable vehicle)
        {
            var tbl = db.GetTableInfo("VehicleOffline");

            if (tbl.Count == 0)
            {
                db.CreateTable<VehicleOffline>();
            }

            VehicleOffline offline = new VehicleOffline()
            {
                VehicleKey = vehicle.Key
            };
            db.Insert(offline);
        }

        internal DateTime GetBreakStart()
        {
            List<BreakTable> breaks = new List<BreakTable>();

            breaks = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ActiveBreak] = 1");

            if (breaks.Count > 1)
            {
                return DateTime.Now;
            }

            BreakTable breakItem = new BreakTable();
            breakItem = breaks[0];

            return DateTime.Parse(breakItem.StartDate);
        }

        internal double TotalSinceStart()
        {
            List<ShiftTable> listOfActiveShifts = new List<ShiftTable>();
            listOfActiveShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] = 1");

            if (listOfActiveShifts.Count != 1)
            {
                return -1;
            }

            TimeSpan previous = default(TimeSpan);
            TimeSpan current = DateTime.Now.TimeOfDay;

            previous = DateTime.Parse(listOfActiveShifts[0].StartDate).TimeOfDay;

            TimeSpan totalTime = current - previous;

            if (totalTime.TotalHours > 0)
            {
                return totalTime.TotalHours;
            }
            else
            {
                return 0;
            }
        }

        internal ShiftTable GetCurrentShift()
        {
            List<ShiftTable> listOfShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE ActiveShift == 1");
            if (listOfShifts.Count == 0 || listOfShifts.Count > 1)
            {
                return null;
            }

            return listOfShifts[0];
        }

        internal double NextBreak()
        {
            double nextBreak = 0.00;

            List<ShiftTable> activeShifts = new List<ShiftTable>();
            activeShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");

            if (activeShifts.Count != 1)
            {
                return -1;
            }

            ShiftTable activeShift = new ShiftTable();
            activeShift = activeShifts[0];

            List<BreakTable> breaks = new List<BreakTable>();
            breaks = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ShiftKey] == " + activeShift.Key + " ORDER BY [Key] DESC");

            TimeSpan hoursSince = default(TimeSpan);

            if (breaks.Count == 0)
            {
                hoursSince = TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30);
            }
            else
            {
                DateTime shiftStart = default(DateTime);
                shiftStart = DateTime.Parse(activeShift.StartDate);

                DateTime hadFullBreak = HadFullBreak(breaks);

                if (hadFullBreak > shiftStart)
                {
                    hoursSince = (hadFullBreak - shiftStart) + TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30);
                }
                else
                {
                    hoursSince = TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30);
                }
            }

            nextBreak = hoursSince.TotalHours;

            return nextBreak;
        }

        internal double GetTotalOfSeventy()
        {
            // Get the datetime of now, get last shift end date, if greater than 24 hours, then return 0, else iterate through all previous shifts until a 24 hour break is detected.
            DateTime dateTimeNow = DateTime.Now;

            List<ShiftTable> listOfShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] ORDER BY [StartDate] DESC");

            double totalTime = 0;

            if (listOfShifts.Count == 0)
            {
                return 0;
            }

            if (!listOfShifts[0].ActiveShift && (dateTimeNow - DateTime.Parse(listOfShifts[0].EndDate) > TimeSpan.FromHours(24)))
            {
                return 0;
            }

            DateTime reference = DateTime.Parse(listOfShifts[0].StartDate);

            if (!listOfShifts[0].ActiveShift)
            {
                totalTime = (DateTime.Parse(listOfShifts[0].EndDate) - DateTime.Parse(listOfShifts[0].StartDate)).TotalMinutes;
            }
            else
            {
                totalTime = (dateTimeNow - DateTime.Parse(listOfShifts[0].StartDate)).TotalMinutes;
            }

            for (int i = 1; i < listOfShifts.Count; i++)
            {
                if (!listOfShifts[i].ActiveShift)
                {
                    if ((reference - DateTime.Parse(listOfShifts[i].EndDate)) < TimeSpan.FromHours(24))
                    {
                        totalTime = totalTime + (DateTime.Parse(listOfShifts[i].EndDate) - DateTime.Parse(listOfShifts[i].StartDate)).TotalMinutes;
                        reference = DateTime.Parse(listOfShifts[i].StartDate);
                    }
                }
            }

            return totalTime / 60;
        }

        internal bool CheckOngoingNotification()
        {
            List<NotificationTable> notify = new List<NotificationTable>();
            notify = db.Query<NotificationTable>("SELECT * FROM [NotificationTable] WHERE [Category] = '" + NotificationCategory.Ongoing + "' AND [Canceled] = 0");

            if (notify.Count == 1)
            {
                return true;
            }

            return false;
        }

        internal double TotalBeforeBreak()
        {
            List<ShiftTable> listOfShiftsForAmount = new List<ShiftTable>();
            listOfShiftsForAmount = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] ORDER BY Key DESC LIMIT 20");
            double totalHours = 0;
            for (int i = 1; i < listOfShiftsForAmount.Count; i++)
            {
                DateTime previous = DateTime.Parse(listOfShiftsForAmount[i - 1].StartDate);
                DateTime current = DateTime.Parse(listOfShiftsForAmount[i].EndDate);

                TimeSpan difference = previous - current;
                if (difference.TotalHours > 24)
                {
                    if (listOfShiftsForAmount[i].EndDate == null)
                    {
                        totalHours = (DateTime.Now.TimeOfDay - DateTime.Parse(listOfShiftsForAmount[i - 1].StartDate).TimeOfDay).TotalHours + totalHours;
                    }
                    else
                    {
                        totalHours = (DateTime.Parse(listOfShiftsForAmount[i].EndDate).TimeOfDay - DateTime.Parse(listOfShiftsForAmount[i - 1].StartDate).TimeOfDay).TotalHours + totalHours;
                    }

                    return totalHours;
                }
                else
                {
                    TimeSpan amountOfTime;
                    if (listOfShiftsForAmount[i].EndDate == null)
                    {
                        amountOfTime = DateTime.Now - DateTime.Parse(listOfShiftsForAmount[i - 1].StartDate);
                    }
                    else
                    {
                        amountOfTime = DateTime.Parse(listOfShiftsForAmount[i].EndDate) - DateTime.Parse(listOfShiftsForAmount[i - 1].StartDate);
                    }

                    totalHours = totalHours + amountOfTime.TotalHours;
                }
            }

            return totalHours;
        }

        internal double GetLastShiftTime()
        {
            List<DayShiftTable> dayShifts = new List<DayShiftTable>();
            dayShifts = db.Query<DayShiftTable>("SELECT * FROM [DayShiftTable] WHERE [IsActive] = 1");

            if (dayShifts.Count == 0)
            {
                dayShifts = db.Query<DayShiftTable>("SELECT * FROM DayShiftTable ORDER BY [DayShiftStart] DESC");

                if (dayShifts.Count == 0)
                {
                    return 0;
                }
            }

            DateTime shiftStartTime = DateTime.Parse(dayShifts[0].DayShiftStart);

            if (shiftStartTime.AddHours(24) < DateTime.Now)
            {
                return 0;
            }
            else if (shiftStartTime.AddHours(14) < DateTime.Now)
            {
                double restTime = (shiftStartTime.AddHours(24) - DateTime.Now).TotalMinutes;

                return restTime;
            }
            else
            {
                return -1;
            }

            //List<ShiftTable> listOfShifts = new List<ShiftTable>();
            //listOfShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] ORDER BY [StartDate] DESC");

            //if (listOfShifts.Count == 0)
            //{
            //    return 0;
            //}

            //ShiftTable lastShift = listOfShifts[0];

            //if (lastShift.EndDate == null)
            //{
            //    return -1;
            //}

            //DateTime endTime = DateTime.Parse(lastShift.EndDate);

            //endTime = endTime.AddHours(10);

            //if (DateTime.Now > endTime)
            //{
            //    return 0;
            //}
            //else
            //{

            //    foreach (ShiftTable workingTable in listOfShifts)
            //    {
            //        // Last shift is greater than one we're iterating through by 10 hours, so this last shift is the one to calculate from
            //        if (workingTable.Key != lastShift.Key)
            //        {
            //            if (DateTime.Parse(workingTable.EndDate).AddHours(10) < DateTime.Parse(lastShift.StartDate))
            //            {
            //                break;
            //            }
            //            else
            //            {
            //                lastShift = workingTable;
            //            }
            //        }
            //    }
            //    endTime = DateTime.Parse(lastShift.StartDate);

            //    //Compare last shift before long break with end date of current shift, if greater than 14 hours, need to set rest for 10 hours from break

            //    if (endTime.AddHours(14) > DateTime.Now)
            //    {
            //        return 0;
            //    }
            //    else
            //    {
            //        //if(DateTime.Parse(listOfShifts[0].EndDate).AddHours(10) > endTime.AddHours(24))
            //        //{
            //        //    return (DateTime.Parse(listOfShifts[0].EndDate).AddHours(10) - DateTime.Now).TotalMinutes;
            //        //}
            //        //else
            //        //{
            //        //    return (endTime.AddHours(24) - DateTime.Now).TotalMinutes;
            //        //}

            //        return (DateTime.Parse(listOfShifts[0].EndDate).AddHours(10) - DateTime.Now).TotalMinutes;

            //    }

            //    //return (endTime.TimeOfDay - DateTime.Now.TimeOfDay).TotalMinutes;
            //}
        }

        internal DateTime GetDayShiftStart()
        {
            List<DayShiftTable> dayShifts = new List<DayShiftTable>();
            dayShifts = db.Query<DayShiftTable>("SELECT * FROM [DayShiftTable] WHERE [IsActive] = 1");

            return DateTime.Parse(dayShifts[0].DayShiftStart);
        }

        internal string GetNextBreakTime()
        {
            // TODO: Get latest Shift, look for latest break, if exists, 5.5 hours since last break, else 5.5 hours since shift start
            List<ShiftTable> listOfActiveShifts = new List<ShiftTable>();
            listOfActiveShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");

            if (listOfActiveShifts.Count == 1)
            {
                List<BreakTable> listBreaksOfShift = new List<BreakTable>();
                listBreaksOfShift = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ShiftKey] == " + listOfActiveShifts[0].ServerKey);

                DateTime start = default(DateTime);
                start = DateTime.Parse(listOfActiveShifts[0].StartDate);

                if (listBreaksOfShift.Count != 0)
                {
                    if (listBreaksOfShift[listBreaksOfShift.Count - 1].EndDate != null)
                    {
                        TimeSpan breakDuration = DateTime.Parse(listBreaksOfShift[listBreaksOfShift.Count - 1].EndDate) - DateTime.Parse(listBreaksOfShift[listBreaksOfShift.Count - 1].StartDate);
                        if (breakDuration.Minutes >= Constants.BREAK_DURATION_TRUCK)
                        {
                            start = DateTime.Parse(listBreaksOfShift[listBreaksOfShift.Count - 1].EndDate);
                        }
                    }
                    else
                    {
                        return "--:--";
                    }
                }

                DateTime breakTime = start.AddHours(5.5);
                return breakTime.ToString("h:mm tt");
            }

            return null;
        }

        internal List<DriveTable> GetDriveShifts(int key)
        {
            List<DriveTable> drives = new List<DriveTable>();
            drives = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ShiftKey] = " + key);

            return drives;
        }

        internal void InsertUserDrives(DriveTable drive)
        {
            db.Insert(drive);
        }

        internal void InsertUserNotes(NoteTable note)
        {
            db.Insert(note);
        }

        internal int HoursTillReset()
        {
            List<ShiftTable> listOfShifts = db.Query<ShiftTable>("SELECT * FROM[ShiftTable] ORDER BY Key DESC LIMIT 2");
            if (listOfShifts.Count == 0)
            {
                return -1;
            }

            ShiftTable lastShift = listOfShifts[0];
            DateTime dateNow = DateTime.Now;

            if (lastShift.EndDate == null)
            {
                // Shift is still occuring, thus 0
                return 0;
            }

            DateTime dateOnLastShift = DateTime.Parse(lastShift.EndDate);
            TimeSpan time = dateNow - dateOnLastShift;
            if (time.TotalDays >= 1)
            {
                // Have rested for longer/as long as 24 hours
                return -2;
            }

            int timeSinceReset = int.Parse(time.ToString().Remove(2));
            return timeSinceReset;
        }

        internal async Task<bool> StartOfflineDriveAsync(int hubo, string note, string location, int vehicleKey)
        {
            DateTime date = DateTime.Now;
            using (UserDialogs.Instance.Loading(Resource.StartingDrive, null, null, true, MaskType.Gradient))
            {
                List<ShiftTable> listOfShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
                if ((listOfShifts.Count == 0) || (listOfShifts.Count > 1))
                {
                    await UserDialogs.Instance.ConfirmAsync(Resource.UnableToGetShift, Resource.Alert, Resource.Okay);
                    return false;
                }

                // Was successful at entering
                DriveTable newDrive = new DriveTable()
                {
                    ActiveVehicle = true,
                    ShiftKey = listOfShifts[0].Key,
                    StartDate = date.ToString(Resource.DateFormat),
                    VehicleKey = vehicleKey,
                    StartHubo = hubo,
                    StartNote = note
                };
                db.Insert(newDrive);

                DriveOffline offlineDrive = new DriveOffline()
                {
                    DriveKey = newDrive.Key,
                    StartOffline = true
                };

                db.Insert(offlineDrive);

                return true;
            }
        }

        internal List<CompanyTable> GetCompanies(int id = 0)
        {
            List<CompanyTable> companies;

            if (id == 0)
            {
                companies = db.Query<CompanyTable>("SELECT * FROM [CompanyTable]");
            }
            else
            {
                companies = db.Query<CompanyTable>("SELECT * FROM [CompanyTable] WHERE [ServerId] = " + id);
            }

            return companies;
        }

        internal ShiftTable InsertUserShifts(ShiftTable shift)
        {
            db.Insert(shift);

            return shift;
        }

        internal void InsertUserBreaks(BreakTable breakTable)
        {
            db.Insert(breakTable);
        }

        internal void AddAmendments(List<AmendmentTable> listOfAmendments, ShiftTable currentShift = null, DriveTable currentVehicleInUse = null, BreakTable currentBreak = null, NoteTable currentNote = null)
        {
            if (currentNote != null)
            {
                db.Update(currentNote);
            }

            if (currentBreak != null)
            {
                db.Update(currentBreak);
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

        internal void UpdateUser(UserTable user)
        {
            db.Update(user);
        }

        internal void UserOffline(UserTable user)
        {
            var tbl = db.GetTableInfo("UserOffline");

            if (tbl.Count == 0)
            {
                db.CreateTable<UserOffline>();
            }

            UserOffline offline = new UserOffline()
            {
                UserKey = user.Id
            };
            db.Insert(offline);
        }

        internal List<ShiftTable> GetShifts(DateTime selectedDate)
        {
            List<ShiftTable> listOfShifts = new List<ShiftTable>();
            List<DateTime> listOfDates = new List<DateTime>();
            DateTime fromDate = selectedDate.AddDays(-7);
            selectedDate = selectedDate.AddDays(1);
            while (fromDate.Date != selectedDate.Date)
            {
                listOfDates.Add(fromDate);
                fromDate = fromDate.AddDays(1);
            }

            foreach (DateTime date in listOfDates)
            {
                List<ShiftTable> listOfShiftsToAdd = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartDate] LIKE '%" + date.Date.ToString("yyyy-MM-dd") + "%'");
                if (listOfShiftsToAdd.Count != 0)
                {
                    listOfShifts.AddRange(listOfShiftsToAdd);
                }
            }

            return listOfShifts;
        }

        internal async Task SaveNote(string note, DateTime date)
        {
            NoteTable newNote = new NoteTable()
            {
                Note = note,
                Date = date.ToString(Resource.DateFormat)
            };
            List<ShiftTable> currentShiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if ((currentShiftList.Count == 0) || (currentShiftList.Count > 1))
            {
                await UserDialogs.Instance.ConfirmAsync(Resource.UnableToGetShift, Resource.Alert, Resource.Okay);
            }
            else
            {
                ShiftTable currentShift = currentShiftList[0];
                newNote.ShiftKey = currentShift.Key;
                db.Insert(newNote);

                restAPI = new RestService();
                int result = await restAPI.InsertNote(newNote);

                switch (result)
                {
                    case -1:
                        await UserDialogs.Instance.ConfirmAsync(Resource.ConnectionError, Resource.Alert, Resource.Okay);
                        break;
                    case -2:
                        await UserDialogs.Instance.ConfirmAsync(Resource.ServerError, Resource.Alert, Resource.Okay);
                        break;
                    default:
                        return;
                }

                var tbl = db.GetTableInfo("NoteOffline");

                if (tbl.Count == 0)
                {
                    db.CreateTable<NoteOffline>();
                }

                NoteOffline offline = new NoteOffline()
                {
                    NoteKey = newNote.Key
                };
                db.Insert(offline);
            }
        }

        internal bool InsertUserLicence(LicenceTable licence)
        {
            if (db.Insert(licence) != 0)
            {
                return true;
            }

            return false;
        }

        internal async Task<int> GetRego()
        {
            VehicleTable vehicleToInsert = new VehicleTable();
            if (CrossConnectivity.Current.IsConnected)
            {
                await CrossMedia.Current.Initialize();

                MediaFile photo;

                if (CrossMedia.Current.IsCameraAvailable || CrossMedia.Current.IsTakePhotoSupported)
                {
                    photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Directory = "Rego",
                        Name = "Rego.jpg",
                        PhotoSize = PhotoSize.Small,
                        DefaultCamera = CameraDevice.Rear,
                        SaveToAlbum = false
                    });
                }
                else if (CrossMedia.Current.IsPickPhotoSupported)
                {
                    photo = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.Small
                    });
                }
                else
                {
                    await UserDialogs.Instance.ConfirmAsync(Resource.RegoError, Resource.Alert, Resource.Okay);
                    return -1;
                }

                using (UserDialogs.Instance.Loading(Resource.AddingVehicle, null, null, true, MaskType.Gradient))
                {
                    if (photo == null)
                    {
                        await UserDialogs.Instance.ConfirmAsync(Resource.RegoError, Resource.Alert, Resource.Okay);
                        return -1;
                    }

                    restAPI = new RestService();

                    OcrResults rego = await restAPI.GetRegoText(photo);
                    if (rego == null)
                    {
                        return -1;
                    }

                    List<string> regoList = new List<string>();
                    Regex r = new Regex("^[A-Za-z0-9Ø]*$");
                    foreach (Region region in rego.Regions)
                    {
                        foreach (Line line in region.Lines)
                        {
                            string fullLine = string.Empty;
                            foreach (Word word in line.Words)
                            {
                                word.Text = word.Text.Replace(".", string.Empty).Replace(",", string.Empty);

                                if (r.IsMatch(word.Text.ToString()))
                                {
                                    fullLine = fullLine + word.Text.ToString();
                                }
                            }

                            if (fullLine != string.Empty && fullLine.Trim().Length < 7)
                            {
                                List<string> listOfPossibilities = CheckPossiblities(fullLine);

                                foreach (string possibility in listOfPossibilities)
                                {
                                    regoList.Add(possibility);
                                }
                            }
                        }
                    }

                    restAPI = new RestService();
                    if (regoList.Count > 0)
                    {
                        string regoAnswer = await UserDialogs.Instance.ActionSheetAsync(Resource.VehicleQuestion, Resource.Cancel, Resource.InputOwnRego, null, regoList.ToArray());

                        if (regoAnswer == Resource.Cancel)
                        {
                            return -1;
                        }

                        if (regoAnswer != Resource.InputOwnRego)
                        {
                            vehicleToInsert.Registration = regoAnswer;
                            db.Insert(vehicleToInsert);
                            return vehicleToInsert.Key;
                        }
                    }
                }

                PromptResult regoResult = await UserDialogs.Instance.PromptAsync(Resource.RegoInput, Resource.Alert, Resource.Okay, Resource.Cancel);
                if (regoResult.Ok && regoResult.Text != string.Empty)
                {
                    vehicleToInsert.Registration = regoResult.Text;
                    db.Insert(vehicleToInsert);
                    return vehicleToInsert.Key;
                }
            }

            return -1;
        }

        //internal string GetFirstShiftStartTime()
        //{
        //    List<ShiftTable> listOfShifts = new List<ShiftTable>();
        //    listOfShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] ORDER BY [StartDate] DESC");

        //    if (listOfShifts.Count == 0)
        //    {
        //        return 0;
        //    }

        //    ShiftTable lastShift = listOfShifts[0];

        //    if (lastShift.EndDate == null)
        //    {
        //        return -1;
        //    }

        //    DateTime endTime = DateTime.Parse(lastShift.EndDate);

        //    endTime = endTime.AddHours(10);

        //    if (DateTime.Now > endTime)
        //    {
        //        return 0;
        //    }
        //    else
        //    {

        //        foreach (ShiftTable workingTable in listOfShifts)
        //        {
        //            // Last shift is greater than one we're iterating through by 10 hours, so this last shift is the one to calculate from
        //            if (workingTable.Key != lastShift.Key)
        //            {
        //                if (DateTime.Parse(workingTable.EndDate).AddHours(10) < DateTime.Parse(lastShift.StartDate))
        //                {
        //                    break;
        //                }
        //                else
        //                {
        //                    lastShift = workingTable;
        //                }
        //            }
        //        }
        //        endTime = DateTime.Parse(lastShift.StartDate);

        //        //Compare last shift before long break with end date of current shift, if greater than 14 hours, need to set rest for 10 hours from break

        //        if (endTime.AddHours(14) > DateTime.Now)
        //        {
        //            return 0;
        //        }
        //        else
        //        {
        //            //if(DateTime.Parse(listOfShifts[0].EndDate).AddHours(10) > endTime.AddHours(24))
        //            //{
        //            //    return (DateTime.Parse(listOfShifts[0].EndDate).AddHours(10) - DateTime.Now).TotalMinutes;
        //            //}
        //            //else
        //            //{
        //            //    return (endTime.AddHours(24) - DateTime.Now).TotalMinutes;
        //            //}

        //            return (DateTime.Parse(listOfShifts[0].EndDate).AddHours(10) - DateTime.Now).TotalMinutes;

        //        }

        //        //return (endTime.TimeOfDay - DateTime.Now.TimeOfDay).TotalMinutes;
        //    }
        //}

        internal List<string> CheckPossiblities(string fullLine)
        {
            List<string> fullSetPossibilities = new List<string>();
            fullLine = Regex.Replace(fullLine, "Ø", "0");
            fullLine = Regex.Replace(fullLine, "i", "1");
            fullLine = Regex.Replace(fullLine, "l", "1");
            fullSetPossibilities.Add(fullLine);
            if (fullLine.Contains("I"))
            {
                List<string> possibilities = Combinations(fullLine, 'I', "1").ToList();
                foreach (string possibility in possibilities)
                {
                    fullSetPossibilities.Add(possibility);
                }
            }

            return fullSetPossibilities.Distinct().ToList();
        }

        internal void InsertNote(NoteTable newNote)
        {
            db.Insert(newNote);
        }

        internal async Task<bool> StartDrive(int hubo, string note, string location, int vehicleKey)
        {
            restAPI = new RestService();
            DateTime date = DateTime.Now;
            using (UserDialogs.Instance.Loading(Resource.StartingDrive, null, null, true, MaskType.Gradient))
            {
                List<ShiftTable> listOfShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
                if ((listOfShifts.Count == 0) || (listOfShifts.Count > 1))
                {
                    await UserDialogs.Instance.ConfirmAsync(Resource.UnableToGetShift, Resource.Alert, Resource.Okay);
                    return false;
                }

                VehicleTable currentVehicle = db.Get<VehicleTable>(vehicleKey);

                // Was successful at entering
                DriveTable newDrive = new DriveTable()
                {
                    ActiveVehicle = true,
                    ShiftKey = listOfShifts[0].Key,
                    ServerShiftKey = listOfShifts[0].ServerKey,
                    StartDate = date.ToString(Resource.DateFormat),
                    ServerVehicleKey = currentVehicle.ServerKey,
                    VehicleKey = vehicleKey,
                    StartHubo = hubo,
                    StartNote = note
                };
                db.Insert(newDrive);

                if (await ReturnOffline())
                {
                    int result = await restAPI.QueryDrive(false, newDrive);

                    if (result > 0)
                    {
                        newDrive.ServerId = result;
                        db.Update(newDrive);
                        return true;
                    }
                }

                DriveOffline offlineDrive = new DriveOffline()
                {
                    DriveKey = newDrive.Key,
                    StartOffline = true
                };

                db.Insert(offlineDrive);

                return true;
            }
        }

        internal async Task<bool> StopDrive(int hubo, string note, string location)
        {
            restAPI = new RestService();
            DateTime date = DateTime.Now;
            using (UserDialogs.Instance.Loading(Resource.EndingDrive, null, null, true, MaskType.Gradient))
            {
                // await ReturnOffline();
                List<DriveTable> listOfVehiclesInUse = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ActiveVehicle] == 1");
                if (listOfVehiclesInUse.Count == 0)
                {
                    await UserDialogs.Instance.ConfirmAsync(Resource.UnableToGetVehicle, Resource.Alert, Resource.Okay);
                    return false;
                }

                DriveTable drive = listOfVehiclesInUse[0];

                //int distanceTravelled = hubo - drive.StartHubo;

                //while (!correctHubo)
                //{

                //if (distanceTravelled < 0)
                //{
                //    if (await UserDialogs.Instance.ConfirmAsync("Starting Odometer: " + drive.StartHubo + " \nis HIGHER than \nEnding Odometer: " + hubo, "Re-Enter Odometer", "Alert", "This is the correct odometer"))
                //    {
                //        return false;
                //    }
                //}
                //else if (!await UserDialogs.Instance.ConfirmAsync("Did you travel " + distanceTravelled.ToString() + "KM?", "Distance Travelled", "I did", "I did not"))
                //if (!await UserDialogs.Instance.ConfirmAsync("Did you travel " + distanceTravelled.ToString() + "KM?", "Distance Travelled", "I did", "I did not"))
                //{
                //    await UserDialogs.Instance.ConfirmAsync(Resource.Alert, Resource.InvalidHubo, Resource.GotIt);
                //    return false;
                //}
                //    else
                //    {
                //        correctHubo = true;
                //    }
                //}

                drive.ActiveVehicle = false;
                drive.EndDate = date.ToString(Resource.DateFormat);
                drive.EndHubo = hubo;
                drive.EndNote = note;
                db.Update(drive);

                if (await ReturnOffline())
                {
                    int result = await restAPI.QueryDrive(true, drive);

                    if (result > 0)
                    {
                        return true;
                    }
                }

                List<DriveOffline> listOfflineDrives = db.Query<DriveOffline>("SELECT * FROM [DriveOffline] WHERE [DriveKey] == " + drive.Key);

                if (listOfflineDrives.Count == 0)
                {
                    DriveOffline offline = new DriveOffline()
                    {
                        DriveKey = drive.Key,
                        EndOffline = true
                    };
                    db.Insert(offline);
                }
                else
                {
                    DriveOffline offline = listOfflineDrives[0];
                    offline.EndOffline = true;
                    db.Update(offline);
                }

                return true;
            }
        }

        internal bool CheckFullBreak()
        {
            List<ShiftTable> shifts = new List<ShiftTable>();
            shifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] = 1");

            if (shifts.Count == 1)
            {
                List<BreakTable> breaks = new List<BreakTable>();
                breaks = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ShiftKey] = " + shifts[0].Key);

                if (breaks.Count > 0)
                {
                    DateTime fullBreak = HadFullBreak(breaks);
                    DateTime shiftStart = DateTime.Parse(shifts[0].StartDate);
                    if (fullBreak > shiftStart)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        internal void CancelNotification(NotificationCategory notifyCategory, bool isTimed, bool fired = false)
        {
            List<NotificationTable> notify = new List<NotificationTable>();
            NotificationTable cancelNotify = new NotificationTable();

            if (isTimed)
            {
                notify = db.Query<NotificationTable>("SELECT * FROM [NotificationTable] WHERE [Category] = '" + notifyCategory + "' AND [Canceled] = 0 AND [Fired] = 0");

                if (notify.Count == 1)
                {
                    cancelNotify = notify[0];
                    cancelNotify.Canceled = true;

                    if (fired)
                    {
                        cancelNotify.Fired = true;
                    }

                    db.Update(cancelNotify);
                }
            }
            else
            {
                notify = db.Query<NotificationTable>("SELECT * FROM [NotificationTable] WHERE [Category] = '" + notifyCategory + "' AND [Canceled] = 0 AND [Fired] = 1");

                if (notify.Count == 1)
                {
                    cancelNotify = notify[0];
                    cancelNotify.Canceled = true;

                    db.Update(cancelNotify);
                }
            }
        }

        internal bool CheckTimedNotification(NotificationCategory notifyCategory, DateTime fireTime = default(DateTime))
        {
            List<NotificationTable> notify = new List<NotificationTable>();
            notify = db.Query<NotificationTable>("SELECT * FROM [NotificationTable] WHERE [Category] = '" + notifyCategory + "' AND [Canceled] = 0 AND [Fired] = 0");

            if (notify.Count == 1)
            {
                DateTime setFire = notify[0].TimeStamp + notify[0].FireTime;
                if (setFire != fireTime)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        internal void CreateNotification(string notificationText, bool isTimed, NotificationCategory notificationCategory, TimeSpan fireTime = default(TimeSpan))
        {
            NotificationTable newNotify = new NotificationTable()
            {
                Text = notificationText,
                Category = notificationCategory.ToString()
            };

            if (isTimed)
            {
                newNotify.FireTime = fireTime;
            }
            else
            {
                newNotify.Fired = true;
            }

            db.Insert(newNotify);
        }

        internal void InsertDrive(DriveTable newDrive)
        {
            db.Insert(newDrive);
        }

        internal async void CollectGeolocation(int driveKey)
        {
            GeolocationTable geoInsert = new GeolocationTable();
            RestService restApi = new RestService();
            Geolocation geolocation = await restApi.GetLatAndLong().ConfigureAwait(false);

            geoInsert.DriveKey = driveKey;
            geoInsert.TimeStamp = DateTime.Now.ToString(Resource.DateFormat);
            geoInsert.Latitude = geolocation.Latitude;
            geoInsert.Longitude = geolocation.Longitude;

            db.Insert(geoInsert);
        }

        internal List<NoteTable> GetNotes(int shiftKey)
        {
            List<NoteTable> listOfNotes = new List<NoteTable>();
            listOfNotes = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [ShiftKey] = " + shiftKey);
            return listOfNotes;
        }

        internal List<BreakTable> GetBreaks(ShiftTable shift)
        {
            List<BreakTable> listOfBreaks = new List<BreakTable>();
            listOfBreaks = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ShiftKey] == " + shift.Key);
            return listOfBreaks;
        }

        internal bool CheckLoggedIn()
        {
            List<UserTable> list = new List<UserTable>();
            list = db.Query<UserTable>("SELECT * FROM [UserTable]");
            if (list.Count == 1)
            {
                return true;
            }
            else if (list.Count > 1)
            {
                UserDialogs.Instance.AlertAsync(Resource.ErrorMoreOneUser, Resource.Alert, Resource.Okay);

                ClearTablesForNewUser();
            }

            return false;
        }

        internal List<VehicleTable> GetVehicles()
        {
            List<VehicleTable> vehiclesList = db.Query<VehicleTable>("SELECT * FROM [VehicleTable]");
            return vehiclesList;
        }

        internal bool CheckActiveShift()
        {
            List<ShiftTable> shiftList = new List<ShiftTable>();
            shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");

            if (shiftList.Count == 0)
            {
                return false;
            }
            else if (shiftList.Count == 1)
            {
                return true;
            }

            UserDialogs.Instance.AlertAsync(Resource.MoreOneActiveShift, Resource.Alert, Resource.Okay);
            return false;
        }

        internal int CheckOnBreak()
        {
            List<BreakTable> breakList = new List<BreakTable>();
            breakList = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ActiveBreak] == 1");
            if (breakList.Count == 0)
            {
                return 1;
            }
            else if (breakList.Count == 1)
            {
                return -1;
            }

            UserDialogs.Instance.AlertAsync(Resource.MoreOneActiveBreak, Resource.Alert, Resource.Okay);
            return -2;
        }

        internal VehicleTable GetCurrentVehicle()
        {
            // Retrieve currently used vehicle
            List<DriveTable> currentVehicleInUseList = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ActiveVehicle] == 1");

            List<VehicleTable> allVehicles = db.Query<VehicleTable>("SELECT * FROM [VehicleTable]");

            // Retrieve vehicle details using currentvehicles key
            List<VehicleTable> vehicleList = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [ServerKey] == " + currentVehicleInUseList[0].ServerVehicleKey);
            if (vehicleList.Count == 0)
            {
                vehicleList = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [Key] == " + currentVehicleInUseList[0].VehicleKey);
            }
            currentVehicleInUseList[0].VehicleKey = vehicleList[0].Key;
            db.Update(currentVehicleInUseList[0]);

            VehicleTable vehicle = vehicleList[0];

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

            UserDialogs.Instance.AlertAsync(Resource.MoreOneActiveVehicle, Resource.Alert, Resource.Okay);
            return false;
        }

        internal async Task<bool> StopBreak(string location, string note)
        {
            using (UserDialogs.Instance.Loading(Resource.StoppingBreak, null, null, true, MaskType.Gradient))
            {
                List<BreakTable> currentBreaks = new List<BreakTable>();
                currentBreaks = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ActiveBreak] == 1");
                if ((currentBreaks.Count == 0) || (currentBreaks.Count > 1))
                {
                    await UserDialogs.Instance.AlertAsync(Resource.UnableToGetBreak, Resource.Alert, Resource.Okay);
                    return false;
                }

                BreakTable currentBreak = currentBreaks[0];
                currentBreak.EndDate = DateTime.Now.ToString(Resource.DateFormat);
                currentBreak.ActiveBreak = false;
                currentBreak.EndLocation = location;
                currentBreak.EndNote = note;
                db.Update(currentBreak);

                if (await ReturnOffline())
                {
                    restAPI = new RestService();
                    int result = await restAPI.QueryBreak(true, currentBreak);

                    if (result > 0)
                    {
                        return true;
                    }
                }

                List<BreakOffline> listOfflineBreaks = db.Query<BreakOffline>("SELECT * FROM [BreakOffline] WHERE [BreakKey] == " + currentBreak.Key);
                if (listOfflineBreaks.Count > 0)
                {
                    BreakOffline offline = listOfflineBreaks[0];
                    offline.EndOffline = true;
                    db.Update(offline);
                }
                else
                {
                    BreakOffline offline = new BreakOffline()
                    {
                        BreakKey = currentBreak.Key,
                        StartOffline = false,
                        EndOffline = true
                    };
                    db.Insert(offline);
                }

                return true;
            }
        }

        internal VehicleTable InsertVehicle(VehicleTable vehicleToAdd)
        {
            db.Insert(vehicleToAdd);

            return vehicleToAdd;
        }

        internal async Task<bool> InsertVehicle(int vehicleKey)
        {
            restAPI = new RestService();
            VehicleTable vehicleToInsert = db.Get<VehicleTable>(vehicleKey);

            int vehicleServerKey = await restAPI.InsertVehicle(vehicleToInsert);
            if (vehicleServerKey > 0)
            {
                vehicleToInsert.ServerKey = vehicleServerKey;
                db.Update(vehicleToInsert);
                return true;
            }
            else
            {
                VehicleOffline offlineVehicle = new VehicleOffline()
                {
                    VehicleKey = vehicleKey
                };
                db.Insert(offlineVehicle);
                return false;
            }
        }

        internal bool Login(UserTable user)
        {
            db.Insert(user);
            return true;
        }

        internal async Task<bool> StartBreak(string location, string note)
        {
            using (UserDialogs.Instance.Loading(Resource.StartingBreak, null, null, true, MaskType.Gradient))
            {
                BreakTable newBreak = new BreakTable();
                List<ShiftTable> activeShift = new List<ShiftTable>();
                activeShift = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");

                if (CheckActiveShiftIsCorrect(true, activeShift))
                {
                    newBreak.ServerShiftKey = activeShift[0].ServerKey;
                    newBreak.ShiftKey = activeShift[0].Key;
                    newBreak.StartDate = DateTime.Now.ToString(Resource.DateFormat);
                    newBreak.ActiveBreak = true;
                    newBreak.StartLocation = location;
                    newBreak.StartNote = note;
                    db.Insert(newBreak);

                    if (await ReturnOffline())
                    {
                        restAPI = new RestService();
                        int result = await restAPI.QueryBreak(false, newBreak);

                        if (result > 0)
                        {
                            newBreak.ServerId = result;
                            db.Update(newBreak);
                            return true;
                        }
                    }

                    BreakOffline offline = new BreakOffline()
                    {
                        BreakKey = newBreak.Key,
                        StartOffline = true
                    };
                    db.Insert(offline);

                    return true;
                }

                return false;
            }
        }

        internal bool CheckTenHourBreak()
        {
            List<ShiftTable> shifts = new List<ShiftTable>();
            shifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable]");

            if (shifts.Count < 1)
            {
                return true;
            }

            ShiftTable shift = new ShiftTable();
            shift = shifts[shifts.Count - 1];

            DateTime end = DateTime.Parse(shift.EndDate);
            DateTime now = DateTime.Now;

            if ((now - end) < TimeSpan.FromHours(10))
            {
                return false;
            }

            return true;
        }

        internal List<QuestionModel> GetChecklistHealthSafety()
        {
            List<QuestionModel> questions = new List<QuestionModel>
            {
                new QuestionModel() { Question = "Have you consumed any alcohol or drugs prior to this shift?", YesCorrect = false },
                new QuestionModel() { Question = "Have you had sufficient sleep prior to this shift?", YesCorrect = true },
                new QuestionModel() { Question = "Do you feel tired?", YesCorrect = false },
                new QuestionModel() { Question = "Have you operated a motor vehicle or machinery in excess of 1 hour during your 10 hour break?", YesCorrect = false },
                new QuestionModel() { Question = "Do you have the correct PPE with you?", YesCorrect = true }
            };
            return questions;
        }

        internal List<string> GetChecklist()
        {
            List<string> questions = new List<string>
            {
                "Current COF / Rego?",
                "Correct RUC for shift?",
                "TSL Number displayed?",
                "RT/MDT?",
                "Oil levels?",
                "Coolant levels?",
                "Warning lights / buzzers?",
                "Lights / Indicators / Reflectors?",
                "Windscreen / Wipers?",
                "Steering / Controls?",
                "Tyres / Wheels / Rims",
                "Tow Coupling / 5th Wheel?",
                "Ramps?",
                "Safety poles / Cables?",
                "Load Security Equipment?"
            };
            return questions;
        }

        internal void UpdateVehicleInfo(VehicleTable editedVehicle)
        {
            db.Update(editedVehicle);
        }

        internal void Logout()
        {
            ClearTablesForNewUser();
            ClearTablesForUserShifts();
        }

        internal async Task<bool> StartShift(string location, string note, Geolocation geoCoords)
        {
            restAPI = new RestService();
            DayShiftTable currentDayShift = new DayShiftTable();
            List<UserTable> user = new List<UserTable>();

            using (UserDialogs.Instance.Loading(Resource.StartingShift, null, null, true, MaskType.Gradient))
            {
                List<DayShiftTable> dayShifts = new List<DayShiftTable>();

                dayShifts = db.Query<DayShiftTable>("SELECT * FROM DayShiftTable WHERE IsActive = 1");
                user = db.Query<UserTable>("SELECT * FROM [UserTable]");

                if (dayShifts.Count > 1)
                {
                    return false;
                }
                else if (dayShifts.Count == 0)
                {
                    DayShiftTable newDay = new DayShiftTable()
                    {
                        DayShiftStart = DateTime.Now.ToString(Resource.DateFormat),
                        ServerKey = await restAPI.NewDayShift(user[0].DriverId),
                        IsActive = true
                    };

                    db.Insert(newDay);

                    if (newDay.ServerKey < 1)
                    {
                        DayShiftOffline dayOffline = new DayShiftOffline()
                        {
                            Key = newDay.Key
                        };

                        db.Insert(dayOffline);
                    }

                    currentDayShift = newDay;
                }
                else
                {
                    if ((DateTime.Now - DateTime.Parse(dayShifts[0].DayShiftStart)) > TimeSpan.FromHours(14))
                    {
                        DayShiftTable oldDay = new DayShiftTable();
                        oldDay = dayShifts[0];
                        oldDay.IsActive = false;

                        db.Update(oldDay);

                        DayShiftTable newDay = new DayShiftTable()
                        {
                            DayShiftStart = DateTime.Now.ToString(Resource.DateFormat),
                            ServerKey = await restAPI.NewDayShift(user[0].DriverId),
                            IsActive = true
                        };

                        db.Insert(newDay);

                        if (newDay.ServerKey < 1)
                        {
                            DayShiftOffline dayOffline = new DayShiftOffline()
                            {
                                Key = newDay.Key
                            };

                            db.Insert(dayOffline);
                        }

                        currentDayShift = newDay;
                    }
                    else
                    {
                        currentDayShift = dayShifts[0];
                    }
                }
            }

            if ((DateTime.Now - DateTime.Parse(currentDayShift.DayShiftStart)) > TimeSpan.FromHours(14) && (DateTime.Now - DateTime.Parse(currentDayShift.DayShiftStart)) < TimeSpan.FromHours(24))
            {
                await UserDialogs.Instance.AlertAsync(Resource.ShortBreakBetweenShifts, Resource.Alert, Resource.Okay);
            }

            using (UserDialogs.Instance.Loading(Resource.StartingShift, null, null, true, MaskType.Gradient))
            {
                ShiftTable shift = new ShiftTable()
                {
                    StartDate = DateTime.Now.ToString(Resource.DateFormat),
                    StartLat = geoCoords.Latitude,
                    StartLong = geoCoords.Longitude,
                    StartLocation = location,
                    ActiveShift = true,
                    StartNote = note,
                    DayShiftKey = currentDayShift.Key
                };
                db.Insert(shift);

                List<CompanyTable> company = new List<CompanyTable>();

                company = db.Query<CompanyTable>("SELECT * FROM [CompanyTable]");

                if (await ReturnOffline())
                {
                    var result = await restAPI.QueryShift(shift, false, user[0].DriverId, company[0].ServerId, currentDayShift.ServerKey);

                    if (result > 0)
                    {
                        shift.ServerKey = result;
                        db.Update(shift);
                        return true;
                    }
                }

                ShiftOffline offline = new ShiftOffline()
                {
                    ShiftKey = shift.Key,
                    StartOffline = true
                };

                db.Insert(offline);

                return true;
            }
        }

        internal DriveTable GetCurrentDriveShift()
        {
            List<DriveTable> activeDrives = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ActiveVehicle] = 1");

            DriveTable activeDrive = activeDrives[0];
            return activeDrive;
        }

        internal async Task<bool> StopShift(string location, string note, Geolocation geoCoords)
        {
            restAPI = new RestService();

            using (UserDialogs.Instance.Loading(Resource.StoppingShift, null, null, true, MaskType.Gradient))
            {
                // Get current shift
                List<ShiftTable> activeShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
                if (CheckActiveShiftIsCorrect(true, activeShifts, null))
                {
                    ShiftTable activeShift = activeShifts[0];
                    activeShift.EndDate = DateTime.Now.ToString(Resource.DateFormat);
                    activeShift.ActiveShift = false;
                    activeShift.EndLat = geoCoords.Latitude;
                    activeShift.EndLong = geoCoords.Longitude;
                    activeShift.EndLocation = location;
                    activeShift.EndNote = note;
                    db.Update(activeShift);

                    if (await ReturnOffline())
                    {
                        restAPI = new RestService();
                        var result = await restAPI.QueryShift(activeShift, true);

                        if (result > 0)
                        {
                            return true;
                        }
                    }

                    // TODO: Look for an instance of this shift in the table
                    List<ShiftOffline> listOfflineShifts = db.Query<ShiftOffline>("SELECT * FROM [ShiftOffline] WHERE [ShiftKey] == " + activeShift.Key);

                    if (listOfflineShifts.Count == 0)
                    {
                        ShiftOffline offline = new ShiftOffline()
                        {
                            ShiftKey = activeShift.Key,
                            StartOffline = false,
                            EndOffline = true
                        };
                        db.Insert(offline);
                    }
                    else
                    {
                        ShiftOffline offline = listOfflineShifts[0];
                        offline.EndOffline = true;
                        db.Update(offline);
                    }

                    return true;
                }
            }

            return false;
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

        internal int SaveShift(DateTime shiftStart, DateTime shiftEnd, List<DriveTable> driveList) // , double latStart, double longStart, double latEnd, double longEnd)
        {
            ShiftTable shift = new ShiftTable()
            {
                StartDate = shiftStart.ToString(Resource.DateFormat),
                EndDate = shiftEnd.ToString(Resource.DateFormat),
                ActiveShift = false
            };
            db.Insert(shift);

            foreach (DriveTable drive in driveList)
            {
                drive.ShiftKey = shift.Key;
                db.Insert(drive);
            }

            return shift.Key;
        }

        internal void SaveBreak(BreakTable newBreak)
        {
            db.Insert(newBreak);
        }

        internal List<ExportShift> GetExportShift()
        {
            ExportShift exportData = new ExportShift();
            List<ExportShift> exportList = new List<ExportShift>();

            List<ShiftTable> shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartDate] <= (SELECT DATE('now', '-7 day'))");

            foreach (ShiftTable shiftData in shiftList)
            {
                exportData.shiftCode = shiftData.Key.ToString();
                exportData.activeShift = shiftData.ActiveShift.ToString();
                exportData.shiftStart = shiftData.StartDate;
                exportData.shiftEnd = shiftData.EndDate;
                exportData.startLat = shiftData.StartLat.ToString();
                exportData.startLong = shiftData.StartLong.ToString();
                exportData.endLat = shiftData.EndLat.ToString();
                exportData.endLong = shiftData.EndLong.ToString();

                exportList.Add(exportData);
            }

            return exportList;
        }

        internal List<ExportBreak> GetExportBreak()
        {
            ExportBreak exportData = new ExportBreak();

            List<ExportBreak> exportList = new List<ExportBreak>();

            List<ShiftTable> shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartDate] <= (SELECT DATE('now', '-7 day'))");

            foreach (ShiftTable shiftData in shiftList)
            {
                List<BreakTable> breakList = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ShiftKey] = " + shiftData.Key);

                foreach (BreakTable breakData in breakList)
                {
                    exportData.shiftCode = breakData.Key.ToString();
                    exportData.breakStart = breakData.StartDate;
                    exportData.breakEnd = breakData.EndDate;
                    exportData.activeBreak = breakData.ActiveBreak.ToString();
                    exportData.breakStartLocation = breakData.StartLocation;
                    exportData.breakEndLocation = breakData.EndLocation;

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
            List<ExportNote> exportList = new List<ExportNote>();
            List<ShiftTable> shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartDate] <= (SELECT DATE('now', '-7 day'))");

            foreach (ShiftTable shiftData in shiftList)
            {
                List<NoteTable> noteList = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [ShiftKey] = " + shiftData.Key);

                foreach (NoteTable noteData in noteList)
                {
                    exportData.shiftCode = noteData.Key.ToString();
                    exportData.noteTime = noteData.Date;
                    exportData.noteDetails = noteData.Note;

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

            List<ExportVehicle> exportList = new List<ExportVehicle>();
            List<ShiftTable> shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartDate] <= (SELECT DATE('now', '-7 day'))");

            foreach (ShiftTable shiftData in shiftList)
            {
                List<DriveTable> driveList = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ShiftKey] = " + shiftData.Key);

                foreach (DriveTable driveData in driveList)
                {
                    List<VehicleTable> vehicleList = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [Key] = " + driveData.VehicleKey);
                    foreach (VehicleTable vehicleData in vehicleList)
                    {
                        exportData.vehicleMakeModel = vehicleData.MakeModel;
                        exportData.vehicleRego = vehicleData.Registration;
                        exportData.vehicleCompany = vehicleData.CompanyId.ToString();
                        exportData.currentVehicle = driveData.ActiveVehicle.ToString();

                        exportData.huboStart = driveData.StartHubo.ToString();
                        exportData.huboEnd = driveData.EndHubo.ToString();

                        exportList.Add(exportData);
                    }
                }
            }

            return exportList;
        }

        internal async Task<bool> ReturnOffline()
        {
            restAPI = new RestService();

            List<ShiftOffline> listOfflineShifts = db.Query<ShiftOffline>("SELECT * FROM [ShiftOffline]");
            List<BreakOffline> listOfflineBreaks = db.Query<BreakOffline>("SELECT * FROM [BreakOffline]");
            List<DriveOffline> listOfflineDrives = db.Query<DriveOffline>("SELECT * FROM [DriveOffline]");
            List<VehicleOffline> listOfflineVehicles = db.Query<VehicleOffline>("SELECT * FROM [VehicleOffline]");
            List<DayShiftOffline> listOfflineDayShifts = db.Query<DayShiftOffline>("SELECT * FROM [DayShiftOffline]");

            List<UserTable> user = db.Query<UserTable>("SELECT * FROM [UserTable]");
            List<CompanyTable> company = db.Query<CompanyTable>("SELECT * FROM [CompanyTable]");

            if (user[0] == null || company[0] == null)
            {
                return false;
            }

            if (listOfflineVehicles.Count > 0)
            {
                foreach (VehicleOffline offlineVehicle in listOfflineVehicles)
                {
                    VehicleTable currentVehicle = db.Get<VehicleTable>(offlineVehicle.VehicleKey);
                    int vehicleServerKey = await restAPI.InsertVehicle(currentVehicle);

                    if (vehicleServerKey < 1)
                    {
                        return false;
                    }

                    currentVehicle.ServerKey = vehicleServerKey;
                    db.Update(currentVehicle);
                    db.Delete(offlineVehicle);
                }
            }

            List<DayShiftTable> dayShifts = new List<DayShiftTable>();

            if (listOfflineDayShifts.Count > 0)
            {
                foreach (DayShiftOffline item in listOfflineDayShifts)
                {
                    DayShiftTable currentDayShift = db.Get<DayShiftTable>(item.Key);

                    int dayShiftKey = await restAPI.NewDayShift(user[0].DriverId);

                    if (dayShiftKey < 1)
                    {
                        return false;
                    }

                    currentDayShift.ServerKey = dayShiftKey;
                    db.Update(currentDayShift);

                    dayShifts.Add(currentDayShift);
                }
            }

            if (listOfflineShifts.Count > 0)
            {
                foreach (ShiftOffline offlineShift in listOfflineShifts)
                {
                    ShiftTable currentShift = db.Get<ShiftTable>(offlineShift.ShiftKey);
                    DayShiftTable relatedDayShift = db.Get<DayShiftTable>(currentShift.DayShiftKey);

                    if (offlineShift.StartOffline)
                    {
                        int startResult = await restAPI.QueryShift(currentShift, false, user[0].DriverId, company[0].ServerId, relatedDayShift.ServerKey);
                        if (startResult < 1)
                        {
                            return false;
                        }

                        currentShift.ServerKey = startResult;
                        db.Update(currentShift);
                    }

                    if (offlineShift.EndOffline)
                    {
                        int stopResult = await restAPI.QueryShift(currentShift, true);
                        if (stopResult < 1)
                        {
                            return false;
                        }
                    }

                    db.Delete(offlineShift);
                }
            }

            if (listOfflineDrives.Count > 0)
            {
                foreach (DriveOffline offlineDrive in listOfflineDrives)
                {
                    DriveTable currentDrive = db.Get<DriveTable>(offlineDrive.DriveKey);
                    if (currentDrive.ServerVehicleKey == 0)
                    {
                        VehicleTable currentVehicle = db.Get<VehicleTable>(currentDrive.VehicleKey);
                        currentDrive.ServerVehicleKey = currentVehicle.ServerKey;
                    }
                    ShiftTable currentShift = db.Get<ShiftTable>(currentDrive.ShiftKey);
                    currentDrive.ServerShiftKey = currentShift.ServerKey;
                    if (offlineDrive.StartOffline)
                    {
                        int startDriveResult = await restAPI.QueryDrive(false, currentDrive);
                        if (startDriveResult < 0)
                        {
                            return false;
                        }

                        currentDrive.ServerId = startDriveResult;
                        db.Update(currentDrive);
                    }

                    if (offlineDrive.EndOffline)
                    {
                        int stopDriveResult = await restAPI.QueryDrive(true, currentDrive);
                        if (stopDriveResult < 0)
                        {
                            return false;
                        }
                    }

                    db.Delete(offlineDrive);
                }
            }

            if (listOfflineBreaks.Count > 0)
            {
                foreach (BreakOffline offlineBreak in listOfflineBreaks)
                {
                    BreakTable currentBreak = db.Get<BreakTable>(offlineBreak.BreakKey);
                    ShiftTable currentShift = db.Get<ShiftTable>(currentBreak.ShiftKey);
                    currentBreak.ServerShiftKey = currentShift.ServerKey;
                    if (offlineBreak.StartOffline)
                    {
                        int startBreakResult = await restAPI.QueryBreak(false, currentBreak);
                        if (startBreakResult < 1)
                        {
                            return false;
                        }

                        currentBreak.ServerId = startBreakResult;
                    }

                    if (offlineBreak.EndOffline)
                    {
                        int endBreakResult = await restAPI.QueryBreak(true, currentBreak);
                        if (endBreakResult < 1)
                        {
                            return false;
                        }
                    }

                    db.Update(currentBreak);
                    db.Delete(offlineBreak);
                }
            }

            return true;
        }

        private DateTime HadFullBreak(List<BreakTable> breaks)
        {
            List<DateTime> endBreaks = new List<DateTime>();
            DateTime start = default(DateTime);
            DateTime end = default(DateTime);

            foreach (BreakTable item in breaks)
            {
                start = DateTime.Parse(item.StartDate);
                end = DateTime.Parse(item.EndDate);

                if ((end - start) >= TimeSpan.FromMinutes(30))
                {
                    endBreaks.Add(end);
                }
            }

            if (endBreaks.Count > 0)
            {
                return endBreaks[endBreaks.Count - 1];
            }

            return DateTime.Now - TimeSpan.FromDays(2);
        }

        private bool CheckActiveShiftIsCorrect(bool isShift, List<ShiftTable> activeShifts = null, List<DriveTable> activeDrives = null)
        {
            if (isShift)
            {
                if (activeShifts.Count == 0)
                {
                    UserDialogs.Instance.AlertAsync(Resource.NoActiveShifts, Resource.Alert, Resource.Okay);
                    return false;
                }
                else if (activeShifts.Count > 1)
                {
                    UserDialogs.Instance.AlertAsync(Resource.MoreOneActiveShift, Resource.Alert, Resource.Okay);
                    return false;
                }

                return true;
            }
            else
            {
                if (activeDrives.Count == 0)
                {
                    UserDialogs.Instance.AlertAsync(Resource.NoActiveDrives, Resource.Alert, Resource.Okay);
                    return false;
                }
                else if (activeDrives.Count > 1)
                {
                    UserDialogs.Instance.AlertAsync(Resource.MoreOneActiveDrive, Resource.Alert, Resource.Okay);
                    return false;
                }

                return true;
            }
        }

        private async Task<int> HuboPrompt(int oldHubo)
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
                    Title = promptTitle,
                    Text = oldHubo.ToString()
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
    }
}
