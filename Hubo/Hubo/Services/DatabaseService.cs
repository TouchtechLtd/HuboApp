﻿// <copyright file="DatabaseService.cs" company="TrioTech">
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
            List<UserTable> listUser = db.Query<UserTable>("SELECT * FROM [UserTable]");
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

        internal List<ShiftTable> GetEditableShifts()
        {
            List<ShiftTable> shifts = new List<ShiftTable>();
            DateTime date = default(DateTime);
            date = DateTime.Now - TimeSpan.FromHours(24);

            shifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartDate] < '%" + date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "%'");

            return shifts;
        }

        internal List<LicenceTable> GetLicenceInfo(int driverId)
        {
            List<LicenceTable> licenceList = db.Query<LicenceTable>("SELECT * FROM [LicenceTable] WHERE [DriverId] = " + driverId);

            return licenceList;
        }

        internal List<CompanyTable> GetCompanyInfo(int driverId)
        {
            List<CompanyTable> companyList = db.Query<CompanyTable>("SELECT * FROM [CompanyTable] WHERE [DriverId] = " + driverId);

            return companyList;
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

            if (listOfActiveShifts.Count > 1)
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

            if (breaks.Count == 0)
            {
                TimeSpan hoursSinceStart = default(TimeSpan);

                hoursSinceStart = TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30);

                nextBreak = hoursSinceStart.TotalHours;

                return nextBreak;
            }

            BreakTable lastBreak = new BreakTable();
            lastBreak = breaks[0];

            DateTime driveStart = default(DateTime);
            driveStart = DateTime.Parse(activeShift.StartDate);

            DateTime breakEnd = default(DateTime);
            breakEnd = DateTime.Parse(lastBreak.EndDate);

            TimeSpan hoursSinceBreak = default(TimeSpan);

            hoursSinceBreak = (breakEnd - driveStart) + TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30);

            nextBreak = hoursSinceBreak.TotalHours;

            return nextBreak;
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

        internal string GetLastShiftTime()
        {
            List<ShiftTable> listOfShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable]");
            if (listOfShifts.Count == 0)
            {
                return "You are rested. Start your shift when you can";
            }

            ShiftTable lastShift = listOfShifts[listOfShifts.Count - 1];

            DateTime endTime = DateTime.Parse(lastShift.EndDate);

            endTime = endTime.AddHours(14);

            if (DateTime.Now > endTime)
            {
                return "You are rested. Start your shift when you can";
            }
            else
            {
                return "Your 14 hour rest break ends at: " + endTime.ToString("h: mm tt dddd");
            }
        }

        internal string GetNextBreakTime()
        {
            // TODO: Get latest Shift, look for latest break, if exists, 5.5 hours since last break, else 5.5 hours since shift start
            List<ShiftTable> listOfActiveShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if (listOfActiveShifts.Count == 1)
            {
                List<BreakTable> listBreaksOfShift = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ShiftKey] == " + listOfActiveShifts[0].ServerKey);
                DateTime start = default(DateTime);
                start = DateTime.Parse(listOfActiveShifts[0].StartDate);

                if (listBreaksOfShift.Count != 0)
                {
                    TimeSpan breakDuration = DateTime.Parse(listBreaksOfShift[listBreaksOfShift.Count - 1].EndDate) - DateTime.Parse(listBreaksOfShift[listBreaksOfShift.Count - 1].StartDate);
                    if (breakDuration.Minutes >= Constants.BREAK_DURATION_TRUCK)
                    {
                        start = DateTime.Parse(listBreaksOfShift[listBreaksOfShift.Count - 1].EndDate);
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
            using (UserDialogs.Instance.Loading("Starting Drive...", null, null, true, MaskType.Gradient))
            {
                List<ShiftTable> listOfShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
                if ((listOfShifts.Count == 0) || (listOfShifts.Count > 1))
                {
                    await UserDialogs.Instance.ConfirmAsync(Resource.UnableToGetShift, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                    return false;
                }

                // Was successful at entering
                DriveTable newDrive = new DriveTable()
                {
                    ActiveVehicle = true,
                    ShiftKey = listOfShifts[0].Key,
                    StartDate = date.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    LocalKey = vehicleKey,
                    StartHubo = hubo,
                    StartNote = note
                };
                db.Insert(newDrive);

                DriveOffline offlineDrive = new DriveOffline()
                {
                    DriveKey = newDrive.Key
                };

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
                companies = db.Query<CompanyTable>("SELECT * FROM [CompanyTable] WHERE [Key] = " + id);
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
                Date = date.ToString("yyyy-MM-dd HH:mm:ss.fff")
            };
            List<ShiftTable> currentShiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if ((currentShiftList.Count == 0) || (currentShiftList.Count > 1))
            {
                await UserDialogs.Instance.ConfirmAsync(Resource.UnableToGetShift, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
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
                        await UserDialogs.Instance.ConfirmAsync("Unable to connect to the server", Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                        break;
                    case -2:
                        await UserDialogs.Instance.ConfirmAsync("Internal server error", Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
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

        internal void InsertUserLicence(LicenceTable licence)
        {
            db.Insert(licence);
        }

        internal async Task<int> GetRego()
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
                await UserDialogs.Instance.ConfirmAsync("Unable to get photo of registration plate, Please pick a vehicle from the list", Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                return -1;
            }

            if (photo == null)
            {
                await UserDialogs.Instance.ConfirmAsync("Unable to get photo of registration plate, Please pick a vehicle from the list", Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                return -1;
            }

            OcrResults rego;
            restAPI = new RestService();

            rego = await restAPI.GetRegoText(photo);
            if (rego == null)
            {
                return -1;
            }

            List<string> regoList = new List<string>();
            Regex r = new Regex("^[A-Za-z0-9\u03B8]*$");
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

            VehicleTable vehicleToInsert = new VehicleTable();
            restAPI = new RestService();
            if (regoList.Count > 0)
            {
                string regoAnswer = await UserDialogs.Instance.ActionSheetAsync("Are any of these your vehicle?", "Cancel", Resource.InputOwnRego, null, regoList.ToArray());
                if (regoAnswer != Resource.InputOwnRego)
                {
                    vehicleToInsert.Registration = regoAnswer;
                    db.Insert(vehicleToInsert);
                    return vehicleToInsert.Key;
                }

                if (regoAnswer == "Cancel")
                {
                    return -1;
                }
            }

            PromptResult regoResult = await UserDialogs.Instance.PromptAsync("Please input your Rego", "Alert", "Okay", "Cancel");
            if (regoResult.Ok && regoResult.Text != string.Empty)
            {
                vehicleToInsert.Registration = regoResult.Text;
                db.Insert(vehicleToInsert);
                return vehicleToInsert.Key;
            }

            return -1;
        }

        internal List<string> CheckPossiblities(string fullLine)
        {
            List<string> fullSetPossibilities = new List<string>();
            fullLine = Regex.Replace(fullLine, "\u03B8", "0");
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
            using (UserDialogs.Instance.Loading("Starting Drive...", null, null, true, MaskType.Gradient))
            {
                List<ShiftTable> listOfShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
                if ((listOfShifts.Count == 0) || (listOfShifts.Count > 1))
                {
                    await UserDialogs.Instance.ConfirmAsync(Resource.UnableToGetShift, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                    return false;
                }

                VehicleTable currentVehicle = db.Get<VehicleTable>(vehicleKey);

                // Was successful at entering
                DriveTable newDrive = new DriveTable()
                {
                    ActiveVehicle = true,
                    ShiftKey = listOfShifts[0].Key,
                    StartDate = date.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    VehicleKey = currentVehicle.ServerKey,
                    LocalKey = vehicleKey,
                    StartHubo = hubo,
                    StartNote = note
                };
                db.Insert(newDrive);

                int result = await restAPI.QueryDrive(false, newDrive, listOfShifts[0].ServerKey);

                if (result > 0)
                {
                    newDrive.ServerId = result;
                    db.Update(newDrive);
                }
                else
                {
                    DriveOffline offlineDrive = new DriveOffline()
                    {
                        DriveKey = newDrive.Key
                    };
                }

                return true;
            }
        }

        internal async Task<bool> StopDrive(int hubo, string note, string location)
        {
            restAPI = new RestService();
            DateTime date = DateTime.Now;
            using (UserDialogs.Instance.Loading("Ending Drive...", null, null, true, MaskType.Gradient))
            {
                // await ReturnOffline();
                List<DriveTable> listOfVehiclesInUse = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ActiveVehicle] == 1");
                if (listOfVehiclesInUse.Count == 0)
                {
                    await UserDialogs.Instance.ConfirmAsync(Resource.UnableToGetVehicle, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                    return false;
                }

                DriveTable drive = listOfVehiclesInUse[0];

                int distanceTravelled = hubo - drive.StartHubo;

                if (distanceTravelled < 0)
                {
                    if (await UserDialogs.Instance.ConfirmAsync("Starting Odometer: " + drive.StartHubo + " \nis HIGHER than \nEnding Odometer: " + hubo, "Re-Enter Odometer", "Alert", "This is the correct odometer"))
                    {
                        return false;
                    }
                }

                if (!await UserDialogs.Instance.ConfirmAsync("Did you travel " + distanceTravelled.ToString() + "KM?", "Distance Travelled", "I did", "I did not"))
                {
                    await UserDialogs.Instance.ConfirmAsync("Alert", "Please enter the correct ending odometer", "Got It");
                    return false;
                }

                drive.ActiveVehicle = false;
                drive.EndDate = date.ToString("yyyy-MM-dd HH:mm:ss.fff");
                drive.EndHubo = hubo;
                drive.EndNote = note;
                db.Update(drive);

                List<GeolocationTable> locationUpload = db.Query<GeolocationTable>("SELECT * FROM [GeolocationTable] WHERE [DriveKey] = " + drive.Key);

                if (locationUpload != null)
                {
                    int locationResult = await restAPI.InsertGeoData(locationUpload);

                    switch (locationResult)
                    {
                        case -1:
                            await UserDialogs.Instance.ConfirmAsync("Unable to connect to server", Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                            break;
                        case -2:
                            await UserDialogs.Instance.ConfirmAsync("Internal Server Error", Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                            break;
                        default:
                            db.Query<Geolocation>("DELETE FROM [GeolocationTable]");
                            break;
                    }
                }

                if (drive.ServerId > 0)
                {
                    int result = await restAPI.QueryDrive(true, drive);

                    switch (result)
                    {
                        case -1:
                            await UserDialogs.Instance.ConfirmAsync("Unable to connect to server", Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                            break;
                        case -2:
                            await UserDialogs.Instance.ConfirmAsync("Internal Server Error", Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                            break;
                        default:
                            return true;
                    }
                }

                var tbl = db.GetTableInfo("DriveOffline");

                if (tbl.Count == 0)
                {
                    db.CreateTable<DriveOffline>();

                    DriveOffline offline = new DriveOffline()
                    {
                        DriveKey = drive.Key,
                        EndOffline = true,
                        StartOffline = false
                    };
                    db.Insert(offline);
                }
                else
                {
                    List<DriveOffline> checkOffline = db.Query<DriveOffline>("SELECT [DriveKey] FROM [DriveOffline]");
                    int num = 0;

                    foreach (DriveOffline item in checkOffline)
                    {
                        if (item.DriveKey == drive.Key)
                        {
                            item.EndOffline = true;
                            db.Update(item);
                            num++;
                        }
                    }

                    if (num == 0)
                    {
                        DriveOffline offline = new DriveOffline()
                        {
                            DriveKey = drive.Key,
                            EndOffline = true,
                            StartOffline = false
                        };
                        db.Insert(offline);
                    }
                }

                return true;
            }
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
            geoInsert.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
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
                UserDialogs.Instance.ConfirmAsync(Resource.ErrorMoreOneUser, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);

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
            List<ShiftTable> shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if (shiftList.Count == 0)
            {
                return false;
            }
            else if (shiftList.Count == 1)
            {
                return true;
            }

            UserDialogs.Instance.ConfirmAsync(Resource.MoreOneActiveShift, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
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

            UserDialogs.Instance.ConfirmAsync(Resource.MoreOneActiveBreak, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
            return -2;
        }

        internal VehicleTable GetCurrentVehicle()
        {
            // Retrieve currently used vehicle
            List<DriveTable> currentVehicleInUseList = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ActiveVehicle] == 1");

            // Retrieve vehicle details using currentvehicles key
            List<VehicleTable> vehicleList = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [Key] == " + currentVehicleInUseList[0].LocalKey);
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

            UserDialogs.Instance.ConfirmAsync(Resource.MoreOneActiveVehicle, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
            return false;
        }

        internal async Task<bool> StopBreak(string location, string note)
        {
            using (UserDialogs.Instance.Loading("Stopping Break...", null, null, true, MaskType.Gradient))
            {
                List<BreakTable> currentBreaks = new List<BreakTable>();
                currentBreaks = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ActiveBreak] == 1");
                if ((currentBreaks.Count == 0) || (currentBreaks.Count > 1))
                {
                    await UserDialogs.Instance.ConfirmAsync(Resource.UnableToGetBreak, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                    return false;
                }

                BreakTable currentBreak = currentBreaks[0];
                currentBreak.EndDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                currentBreak.ActiveBreak = false;
                currentBreak.EndLocation = location;
                currentBreak.EndNote = note;
                db.Update(currentBreak);

                if (currentBreak.ServerId > 0)
                {
                    restAPI = new RestService();
                    int result = await restAPI.QueryBreak(true, currentBreak);

                    switch (result)
                    {
                        case -1:
                            await UserDialogs.Instance.ConfirmAsync("Unable to connect to the server", Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                            break;
                        case -2:
                            await UserDialogs.Instance.ConfirmAsync("Internal Server Error", Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                            break;
                        default:
                            return true;
                    }
                }
                else
                {
                    await UserDialogs.Instance.ConfirmAsync("Invalid Server Key", Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                    return false;
                }

                var tbl = db.GetTableInfo("BreakOffline");

                if (tbl.Count == 0)
                {
                    db.CreateTable<BreakOffline>();
                    BreakOffline offline = new BreakOffline()
                    {
                        BreakKey = currentBreak.Key,
                        StartOffline = false,
                        EndOffline = true
                    };
                    db.Insert(offline);
                }
                else
                {
                    List<BreakOffline> checkOffline = db.Query<BreakOffline>("SELECT [BreakKey] FROM [BreakOffline]");
                    int num = 0;

                    foreach (BreakOffline item in checkOffline)
                    {
                        if (item.BreakKey == currentBreak.Key)
                        {
                            item.EndOffline = true;
                            db.Update(item);
                            num++;
                        }
                    }

                    if (num == 0)
                    {
                        BreakOffline offline = new BreakOffline()
                        {
                            BreakKey = currentBreak.Key,
                            StartOffline = false,
                            EndOffline = true
                        };
                        db.Insert(offline);
                    }
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
            using (UserDialogs.Instance.Loading("Starting Break...", null, null, true, MaskType.Gradient))
            {
                BreakTable newBreak = new BreakTable();
                List<ShiftTable> activeShift = new List<ShiftTable>();
                activeShift = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");

                if (CheckActiveShiftIsCorrect(true, activeShift))
                {
                    newBreak.ShiftKey = activeShift[0].ServerKey;
                    newBreak.StartDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    newBreak.ActiveBreak = true;
                    newBreak.StartLocation = location;
                    newBreak.StartNote = note;
                    db.Insert(newBreak);

                    restAPI = new RestService();
                    int result = await restAPI.QueryBreak(false, newBreak);

                    if (result > 0)
                    {
                        newBreak.ServerId = result;
                        db.Update(newBreak);
                    }

                    var tbl = db.GetTableInfo("BreakOffline");

                    if (tbl == null)
                    {
                        db.CreateTable<BreakOffline>();
                    }

                    BreakOffline offline = new BreakOffline()
                    {
                        BreakKey = newBreak.Key,
                        StartOffline = true,
                        EndOffline = false
                    };
                    db.Insert(offline);

                    return true;
                }

                return false;
            }
        }

        internal List<string> GetChecklist()
        {
            List<string> questions = new List<string>
            {
                "This is a test",
                "This is a test",
                "This is a test",
                "This is a test"
            };
            return questions;
        }

        internal void UpdateVehicleInfo(VehicleTable editedVehicle)
        {
            db.Update(editedVehicle);
        }

        internal void Logout()
        {
            db.Query<UserTable>("DELETE FROM [UserTable]");
        }

        internal async Task<bool> StartShift(string location, string note, Geolocation geoCoords)
        {
            restAPI = new RestService();

            using (UserDialogs.Instance.Loading("Starting Shift...", null, null, true, MaskType.Gradient))
            {
                ShiftTable shift = new ShiftTable()
                {
                    StartDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    StartLat = geoCoords.Latitude,
                    StartLong = geoCoords.Longitude,
                    StartLocation = location,
                    ActiveShift = true,
                    StartNote = note
                };
                db.Insert(shift);

                List<UserTable> user = new List<UserTable>();
                List<CompanyTable> company = new List<CompanyTable>();

                user = db.Query<UserTable>("SELECT * FROM [UserTable]");
                company = db.Query<CompanyTable>("SELECT * FROM [CompanyTable]");

                var result = await restAPI.QueryShift(shift, false, user[0].DriverId, company[0].Key);

                if (result > 0)
                {
                    shift.ServerKey = result;
                    db.Update(shift);
                }
                else
                {
                    var tbl = db.GetTableInfo("ShiftOffline");

                    if (tbl.Count == 0)
                    {
                        db.CreateTable<ShiftOffline>();
                    }

                    ShiftOffline offline = new ShiftOffline()
                    {
                        ShiftKey = shift.Key,
                        StartOffline = true,
                        EndOffline = false
                    };
                    db.Insert(offline);
                }

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

            using (UserDialogs.Instance.Loading("Stopping Shift...", null, null, true, MaskType.Gradient))
            {
                // Get current shift
                List<ShiftTable> activeShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
                if (CheckActiveShiftIsCorrect(true, activeShifts, null))
                {
                    ShiftTable activeShift = activeShifts[0];
                    activeShift.EndDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    activeShift.ActiveShift = false;
                    activeShift.EndLat = geoCoords.Latitude;
                    activeShift.EndLong = geoCoords.Longitude;
                    activeShift.EndLocation = location;
                    activeShift.EndNote = note;
                    db.Update(activeShift);

                    if (activeShift.ServerKey > 0)
                    {
                        restAPI = new RestService();
                        var result = await restAPI.QueryShift(activeShift, true);

                        if (result > 0)
                        {
                            MessagingCenter.Send<string>("ShiftEnd", "ShiftEnd");
                            return true;
                        }
                    }

                    var tbl = db.GetTableInfo("ShiftOffline");

                    if (tbl.Count == 0)
                    {
                        db.CreateTable<ShiftOffline>();

                        ShiftOffline offline = new ShiftOffline()
                        {
                            ShiftKey = activeShift.Key,
                            StartOffline = false,
                            EndOffline = true
                        };
                        db.Insert(offline);
                        return true;
                    }
                    else
                    {
                        List<ShiftOffline> checkOffline = db.Query<ShiftOffline>("SELECT [ShiftKey] FROM [ShiftOffline]");
                        int num = 0;

                        foreach (ShiftOffline item in checkOffline)
                        {
                            if (item.ShiftKey == activeShift.Key)
                            {
                                item.EndOffline = true;
                                db.Update(item);
                                num++;
                                return true;
                            }
                        }

                        if (num == 0)
                        {
                            ShiftOffline offline = new ShiftOffline()
                            {
                                ShiftKey = activeShift.Key,
                                StartOffline = false,
                                EndOffline = true
                            };
                            db.Insert(offline);
                            return true;
                        }
                    }

                    await UserDialogs.Instance.ConfirmAsync(Resource.ErrorEndShift, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                    return false;
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
                StartDate = shiftStart.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                EndDate = shiftEnd.ToString("yyyy-MM-dd HH:mm:ss.fff"),
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
            var shiftExists = db.GetTableInfo("ShiftOffline");

            if (shiftExists.Count > 0)
            {
                List<ShiftOffline> shifts = db.Query<ShiftOffline>("SELECT * FROM [ShiftOffline]");

                if (shifts != null)
                {
                    int uploadErrors = 0;
                    foreach (ShiftOffline item in shifts)
                    {
                        UserTable user = db.Query<UserTable>("SELECT * FROM [UserTable]")[0];
                        ShiftTable shift = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [Key] = " + item.ShiftKey)[0];
                        DriveTable drive = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ShiftKey] = " + item.ShiftKey + " LIMIT 1")[0];

                        if (item.StartOffline)
                        {
                            int result = await restAPI.QueryShift(shift, false, user.DriverId, drive.Key);

                            if (result > 0)
                            {
                                shift.ServerKey = result;
                                db.Update(shift);
                            }
                            else
                            {
                                uploadErrors++;
                            }
                        }

                        if (item.EndOffline)
                        {
                            int result = await restAPI.QueryShift(shift, true);

                            if (result != 0)
                            {
                                uploadErrors++;
                            }
                        }

                        if (uploadErrors == 0)
                        {
                            db.Query<ShiftOffline>("DELETE FROM [ShiftOffline] WHERE [ShiftKey] = " + shift.Key);
                        }
                    }
                }

                List<ShiftOffline> empty = db.Query<ShiftOffline>("SELECT * FROM [ShiftOffline]");

                if (empty.Count == 0)
                {
                    db.Query<ShiftOffline>("DROP TABLE [ShiftOffline]");
                }
            }

            var driveExists = db.GetTableInfo("DriveOffline");

            if (driveExists.Count > 0)
            {
                List<DriveOffline> drives = db.Query<DriveOffline>("SELECT * FROM [DriveOffline]");

                if (drives != null)
                {
                    int uploadErrors = 0;
                    foreach (DriveOffline item in drives)
                    {
                        DriveTable drive = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [Key] = " + item.DriveKey)[0];

                        if (item.StartOffline)
                        {
                            int result = await restAPI.QueryDrive(false, drive);

                            if (result > 0)
                            {
                                drive.ServerId = result;
                                db.Update(drive);
                            }
                            else
                            {
                                uploadErrors++;
                            }
                        }

                        if (item.EndOffline)
                        {
                            int result = await restAPI.QueryDrive(true, drive);

                            if (result != 0)
                            {
                                uploadErrors++;
                            }
                        }

                        if (uploadErrors == 0)
                        {
                            db.Query<DriveOffline>("DELETE FROM [DriveOffline] WHERE [DriveKey] = " + drive.Key);
                        }
                    }
                }

                List<DriveOffline> empty = db.Query<DriveOffline>("SELECT * FROM [DriveOffline]");

                if (empty.Count == 0)
                {
                    db.Query<DriveOffline>("DROP TABLE [DriveOffline]");
                }
            }

            var breakExists = db.GetTableInfo("BreakOffline");

            if (breakExists.Count > 0)
            {
                List<BreakOffline> breaks = db.Query<BreakOffline>("SELECT * FROM [BreakOffline]");

                if (breaks != null)
                {
                    int uploadErrors = 0;

                    foreach (BreakOffline item in breaks)
                    {
                        BreakTable breakValue = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [Key] = " + item.BreakKey)[0];

                        if (item.StartOffline)
                        {
                            int result = await restAPI.QueryBreak(false, breakValue);

                            if (result > 0)
                            {
                                breakValue.ServerId = result;
                                db.Update(breakValue);
                            }
                            else
                            {
                                uploadErrors++;
                            }
                        }

                        if (item.EndOffline)
                        {
                            int result = await restAPI.QueryBreak(true, breakValue);

                            if (result != 0)
                            {
                                uploadErrors++;
                            }
                        }

                        if (uploadErrors == 0)
                        {
                            db.Query<BreakOffline>("DELETE FROM [BreakOffline] WHERE [BreakKey] = " + breakValue.Key);
                        }
                    }
                }

                List<BreakOffline> empty = db.Query<BreakOffline>("SELECT * FROM [BreakOffline]");

                if (empty.Count == 0)
                {
                    db.Query<BreakOffline>("DROP TABLE [BreakOffline]");
                }
            }

            var noteExists = db.GetTableInfo("NoteOffline");

            if (noteExists.Count > 0)
            {
                List<NoteOffline> notes = db.Query<NoteOffline>("SELECT * FROM [NoteOffline]");

                if (notes != null)
                {
                    foreach (NoteOffline item in notes)
                    {
                        NoteTable note = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [Key] = " + item.NoteKey)[0];

                        int result = await restAPI.InsertNote(note);

                        if (result > 0)
                        {
                            db.Query<NoteOffline>("DELETE FROM [NoteOffline] WHERE [NoteKey] = " + note.Key);
                        }
                    }
                }

                List<NoteOffline> empty = db.Query<NoteOffline>("SELECT * FROM [NoteOffline]");

                if (empty.Count == 0)
                {
                    db.Query<NoteOffline>("DROP TABLE [NoteOffline]");
                }
            }

            var vehicleExists = db.GetTableInfo("VehicleOffline");

            if (vehicleExists.Count > 0)
            {
                List<VehicleOffline> vehicles = db.Query<VehicleOffline>("SELECT * FROM [VehicleOffline]");

                if (vehicles != null)
                {
                    foreach (VehicleOffline item in vehicles)
                    {
                        VehicleTable vehicle = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [Key] = " + item.VehicleKey)[0];

                        bool result = await restAPI.QueryAddVehicle(vehicle);

                        if (result)
                        {
                            db.Query<VehicleOffline>("DELETE FROM [VehicleOffline] WHERE [VehicleKey] = " + vehicle.Key);
                        }
                    }
                }

                List<VehicleOffline> empty = db.Query<VehicleOffline>("SELECT * FROM [VehicleOffline]");

                if (empty.Count == 0)
                {
                    db.Query<VehicleOffline>("DROP TABLE [VehicleOffline]");
                }
            }

            var userChangesExist = db.GetTableInfo("UserOffline");

            if (userChangesExist.Count > 0)
            {
                List<UserOffline> users = db.Query<UserOffline>("SELECT * FROM [UserOffline]");

                if (users != null)
                {
                    foreach (UserOffline item in users)
                    {
                        UserTable user = db.Query<UserTable>("SELECT * FROM [UserTable] WHERE [Key] = " + item.UserKey)[0];

                        bool result = await restAPI.QueryUpdateProfile(user);

                        if (result)
                        {
                            db.Query<UserOffline>("DELETE FROM [VehicleOffline] WHERE [UserKey] = " + user.Id);
                        }
                    }
                }

                List<UserOffline> empty = db.Query<UserOffline>("SELECT * FROM [UserOffline]");

                if (empty.Count == 0)
                {
                    db.Query<UserOffline>("DROP TABLE [VehicleOffline");
                }
            }

            return true;
        }

        private bool CheckActiveShiftIsCorrect(bool isShift, List<ShiftTable> activeShifts = null, List<DriveTable> activeDrives = null)
        {
            if (isShift)
            {
                if (activeShifts.Count == 0)
                {
                    UserDialogs.Instance.ConfirmAsync(Resource.NoActiveShifts, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                    return false;
                }
                else if (activeShifts.Count > 1)
                {
                    UserDialogs.Instance.ConfirmAsync(Resource.MoreOneActiveShift, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                    return false;
                }

                return true;
            }
            else
            {
                if (activeDrives.Count == 0)
                {
                    UserDialogs.Instance.ConfirmAsync(Resource.NoActiveDrives, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                    return false;
                }
                else if (activeDrives.Count > 1)
                {
                    UserDialogs.Instance.ConfirmAsync(Resource.MoreOneActiveDrive, Resource.DisplayAlertTitle, Resource.DisplayAlertOkay);
                    return false;
                }

                return true;
            }
        }
    }
}
