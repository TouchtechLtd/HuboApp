using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using Xamarin.Forms;
using Plugin.Geolocator;
using Acr.UserDialogs;

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
            db.CreateTable<GeolocationTable>();
            db.CreateTable<LicenceTable>();
            db.CreateTable<ShiftOffline>();
            db.CreateTable<DriveOffline>();
            db.CreateTable<BreakOffline>();
        }

        internal string GetUserToken()
        {
            List<UserTable> listUsers = db.Query<UserTable>("SELECT * FROM [UserTable]");
            string token = "";
            if(listUsers.Count != 0)
            {
                token = listUsers[0].Token;
            }

            return token;
        }

        internal List<LoadTextTable> GetLoadingText()
        {
            List<LoadTextTable> loadList = new List<LoadTextTable>();
            loadList = db.Query<LoadTextTable>("SELECT * FROM [LoadTextTable]");

            return loadList;
        }

        internal List<CompanyTable> GetCompanies(int id = 0)
        {
            List<CompanyTable> companies;

            if (id == 0)
                companies = db.Query<CompanyTable>("SELECT * FROM [CompanyTable]");
            else
                companies = db.Query<CompanyTable>("SELECT * FROM [CompanyTable] WHERE [Key] = " + id);


            return companies;
        }

        internal void InsertUser(UserTable user)
        {
            db.Insert(user);
            //db.Query<UserTable>("INSERT INTO [UserTable] (UserName, FirstName, LastName, Id, Token, DriverId) VALUES (" + user.UserName + "," + user.FirstName + "," + user.LastName + "," + user.Id + "," + user.Token + "," + user.DriverId + ");");
            List<UserTable> list = new List<UserTable>();
            list = db.Query<UserTable>("SELECT * FROM [UserTable]");
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
            vehicleDetails = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [Key] == " + vehicle.VehicleKey);
            if ((vehicleDetails.Count == 0) || (vehicleDetails.Count > 1))
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.GetMoreOneVehicle, Resource.DisplayAlertOkay);
            }
            return vehicleDetails[0];
        }

        internal void HideTip(string tipName)
        {
            List<TipTable> listOfTips = new List<TipTable>();
            listOfTips = db.Query<TipTable>("SELECT * FROM [TipTable] WHERE [TipName] == '" + tipName + "'");
            TipTable tip = listOfTips[0];
            tip.ActiveTip = 0;
            db.Update(tip);
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

            if (tbl == null)
                db.CreateTable<VehicleOffline>();

            VehicleOffline offline = new VehicleOffline();

            offline.VehicleKey = vehicle.Key;

            db.Insert(offline);
        }

        internal double TotalBeforeBreak()
        {
            //TODO: Code to retrieve hours since last gap of 24 hours
            List<ShiftTable> listOfShiftsForAmount = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] ORDER BY Key DESC LIMIT 20");
            double totalHours = 0;
            for (int i = 1; i < listOfShiftsForAmount.Count; i++)
            {
                DateTime previous = new DateTime();
                DateTime current = new DateTime();

                previous = DateTime.Parse(listOfShiftsForAmount[i - 1].StartDate);
                current = DateTime.Parse(listOfShiftsForAmount[i].EndDate);

                TimeSpan difference = new TimeSpan();
                difference = previous - current;
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
                    TimeSpan amountOfTime = new TimeSpan();
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
            //TODO: Code to retrieve time since last shift (Once it hits 24 hours, 70 hour a week will reset
            List<ShiftTable> listOfShifts = new List<ShiftTable>();
            listOfShifts = db.Query<ShiftTable>("SELECT * FROM[ShiftTable] ORDER BY Key DESC LIMIT 2");
            if (listOfShifts.Count == 0)
            {
                return -1;
            }
            ShiftTable lastShift = listOfShifts[0];
            DateTime dateNow = DateTime.Now;
            DateTime dateOnLastShift = new DateTime();

            if (lastShift.EndDate == null)
            {
                //Shift is still occuring, thus 0
                return 0;
            }
            dateOnLastShift = DateTime.Parse(lastShift.EndDate);
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
            usedVehicles = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ShiftKey] == " + currentShift.Key);
            return usedVehicles;
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
            while (fromDate.Date != selectedDate.Date)
            {
                listOfDates.Add(fromDate);
                fromDate = fromDate.AddDays(1);
            }

            foreach (DateTime date in listOfDates)
            {
                listOfShiftsToAdd = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartDate] LIKE '%" + date.Date.ToString("yyyy-MM-dd") + "%'");
                if (listOfShiftsToAdd.Count != 0)
                {
                    listOfShifts.AddRange(listOfShiftsToAdd);
                }
            }

            return listOfShifts;
        }

        internal async void SaveNote(string note, DateTime date)
        {
            NoteTable newNote = new NoteTable();
            newNote.Note = note;
            newNote.Date = date.ToString();

            List<ShiftTable> currentShiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
            if ((currentShiftList.Count == 0) || (currentShiftList.Count > 1))
            {
                await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.UnableToGetShift, Resource.DisplayAlertOkay);
            }
            else
            {
                ShiftTable currentShift = currentShiftList[0];
                newNote.ShiftKey = currentShift.Key;
                db.Insert(newNote);

                RestAPI = new RestService();
                int result = await RestAPI.InsertNote(newNote);

                switch (result)
                {
                    case -1:
                        await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to connect to the server", Resource.DisplayAlertOkay);
                        break;
                    case -2:
                        await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Internal server error", Resource.DisplayAlertOkay);
                        break;
                    default:
                        return;
                }

                var tbl = db.GetTableInfo("NoteOffline");

                if (tbl.Count == 0)
                    db.CreateTable<NoteOffline>();

                NoteOffline offline = new NoteOffline();

                offline.NoteKey = newNote.Key;

                db.Insert(offline);
            }
        }

        internal void InsertUserLicence(LicenceTable licence)
        {
            db.Insert(licence);
        }

        internal async Task<bool> SaveDrive(DateTime date, int vehicleKey = 0)
        {
            RestAPI = new RestService();
            bool invalidFormat = true;
            int hubo = 0;
            string promptTitle = "Current Odometer Reading: ";
            while (invalidFormat)
            {
                hubo = 0;
                PromptConfig huboPrompt = new PromptConfig();
                huboPrompt.IsCancellable = true;
                huboPrompt.Title = promptTitle;
                huboPrompt.SetInputMode(InputType.Number);
                PromptResult promptResult = await UserDialogs.Instance.PromptAsync(huboPrompt);
                bool isNumeric = int.TryParse(promptResult.Text, out hubo);
                if (promptResult.Ok && isNumeric)
                {
                    invalidFormat = false;
                }
                if (!promptResult.Ok)
                {
                    return false;
                }
                promptTitle = "Please entry a VALID odometer reading: ";
            }
            if (CheckActiveDriveShift())
            {
                await ReturnOffline();

                List<DriveTable> listOfVehiclesInUse = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ActiveVehicle] == 1");

                DriveTable drive = new DriveTable();
                drive = listOfVehiclesInUse[0];
                drive.EndDate = date.ToString();
                drive.EndHubo = hubo;
                drive.ActiveVehicle = false;
                db.Update(drive);

                if (drive.ServerId > 0)
                {
                    RestAPI = new RestService();
                    int result = await RestAPI.QueryDrive(true, drive);

                    switch (result)
                    {
                        case -1:
                            await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to connect to server", Resource.DisplayAlertOkay);
                            break;
                        case -2:
                            await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Internal Server Error", Resource.DisplayAlertOkay);
                            break;
                        default:
                            return true;
                    }
                }

                var tbl = db.GetTableInfo("DriveOffline");

                if (tbl.Count == 0)
                {
                    db.CreateTable<DriveOffline>();

                    DriveOffline offline = new DriveOffline();

                    offline.DriveKey = drive.Key;
                    offline.EndOffline = true;
                    offline.StartOffline = false;
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
                        DriveOffline offline = new DriveOffline();

                        offline.DriveKey = drive.Key;
                        offline.EndOffline = true;
                        offline.StartOffline = false;
                        db.Insert(offline);
                    }
                }
                return true;
            }
            else
            {
                List<ShiftTable> listOfShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
                if ((listOfShifts.Count == 0) || (listOfShifts.Count > 1))
                {
                    await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.UnableToGetShift, Resource.DisplayAlertOkay);
                    return false;
                }

                DriveTable newDrive = new DriveTable();
                newDrive.ActiveVehicle = true;
                newDrive.ShiftKey = listOfShifts[0].ServerKey;
                newDrive.StartDate = date.ToString();
                newDrive.VehicleKey = vehicleKey;
                newDrive.StartHubo = hubo;
                db.Insert(newDrive);

                RestAPI = new RestService();
                int result = await RestAPI.QueryDrive(false, newDrive);

                switch (result)
                {
                    case -1:
                        await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to connect to server", Resource.DisplayAlertOkay);
                        break;
                    case -2:
                        await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Internal Server Error", Resource.DisplayAlertOkay);
                        break;
                    default:
                        newDrive.ServerId = result;
                        db.Update(newDrive);

                        return true;
                }

                var tbl = db.GetTableInfo("DriveOffline");

                if (tbl == null)
                    db.CreateTable<DriveOffline>();

                DriveOffline offline = new DriveOffline();

                offline.DriveKey = newDrive.Key;
                offline.StartOffline = true;
                offline.EndOffline = false;

                db.Insert(offline);
                return true;
            }

        }

        internal bool CheckActiveDriveShift()
        {
            List<DriveTable> driveList = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ActiveVehicle] == 1");
            if (driveList.Count == 0)
            {
                return false;
            }
                return true;
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

        internal List<NoteTable> GetNotes()
        {
            List<NoteTable> listOfNotes = new List<NoteTable>();
            listOfNotes = db.Query<NoteTable>("SELECT * FROM [NoteTable]");

            return listOfNotes;
        }

        internal List<BreakTable> GetBreaks(DriveTable drive)
        {
            List<BreakTable> listOfBreaks = new List<BreakTable>();
            //listOfBreaks = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [DriveKey] == " + drive.Key);

            return listOfBreaks;
        }


        internal bool CheckLoggedIn()
        {
            List<UserTable> list = db.Query<UserTable>("SELECT * FROM [UserTable]");
            if (list.Count == 1)
                return true;
            else if (list.Count > 1)
            {
                Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.ErrorMoreOneUser, Resource.DisplayAlertOkay);

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
            Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.MoreOneActiveShift, Resource.DisplayAlertOkay);
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
            Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.MoreOneActiveBreak, Resource.DisplayAlertOkay);
            return false;
        }

        internal VehicleTable GetCurrentVehicle()
        {
            //Retrieve currently used vehicle
            List<DriveTable> currentVehicleInUseList = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ActiveVehicle] == 1");

            List<VehicleTable> allVehicleList = db.Query<VehicleTable>("SELECT * FROM [VehicleTable]");

            //Retrieve vehicle details using currentvehicles key
            List<VehicleTable> vehicleList = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [ServerKey] == " + currentVehicleInUseList[0].VehicleKey + "");
            VehicleTable vehicle = vehicleList[0];

            return vehicle;
        }

 

        internal async Task<bool> StopBreak()
        {
            Geolocation geoCords = new Geolocation();
            string location = "";
            using (UserDialogs.Instance.Loading("Getting Coordinates....", null, null, true, MaskType.Gradient))
            {
                RestAPI = new RestService();
                geoCords = await GetLatAndLong();
                location = await RestAPI.GetLocation(geoCords);
            }

            PromptConfig locationPrompt = new PromptConfig();
            locationPrompt.IsCancellable = true;
            locationPrompt.Title = "Current Location: ";
            locationPrompt.Text = location;
            PromptResult promptResult = await UserDialogs.Instance.PromptAsync(locationPrompt);

            if (promptResult.Ok && (promptResult.Text != ""))
            {
                List<BreakTable> currentBreaks = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ActiveBreak] == 1");
                if ((currentBreaks.Count == 0) || (currentBreaks.Count > 1))
                {
                    await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.UnableToGetBreak, Resource.DisplayAlertOkay);
                    return false;
                }

                BreakTable currentBreak = currentBreaks[0];
                currentBreak.EndDate = DateTime.Now.ToString();
                currentBreak.ActiveBreak = false;
                currentBreak.EndLocation = location;
                db.Update(currentBreak);

                if (currentBreak.ServerId > 0)
                {
                    RestAPI = new RestService();
                    int result = await RestAPI.QueryBreak(true, currentBreak);

                    switch (result)
                    {
                        case -1:
                            await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to connect to the server", Resource.DisplayAlertOkay);
                            break;
                        case -2:
                            await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Internal Server Error", Resource.DisplayAlertOkay);
                            break;
                        default:
                            return true;
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Invalid Server Key", Resource.DisplayAlertOkay);
                    return false;
                }

                var tbl = db.GetTableInfo("BreakOffline");

                if (tbl == null)
                {
                    db.CreateTable<BreakOffline>();
                    BreakOffline offline = new BreakOffline();

                    offline.BreakKey = currentBreak.Key;
                    offline.StartOffline = false;
                    offline.EndOffline = true;
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
                        BreakOffline offline = new BreakOffline();

                        offline.BreakKey = currentBreak.Key;
                        offline.StartOffline = false;
                        offline.EndOffline = true;
                        db.Insert(offline);
                    }
                }
            }
            return false;
        }

        internal VehicleTable InsertVehicle(VehicleTable vehicleToAdd)
        {
            db.Insert(vehicleToAdd);

            return vehicleToAdd;
        }

        internal bool Login(UserTable user)
        {
            db.Insert(user);
            return true;
        }

        internal async Task<bool> StartBreak()
        {
            Geolocation geoCords = new Geolocation();
            string location = "";
            using (UserDialogs.Instance.Loading("Getting Coordinates....", null, null, true, MaskType.Gradient))
            {
                RestAPI = new RestService();                
                geoCords = await GetLatAndLong();
                location = await RestAPI.GetLocation(geoCords);
            }

            PromptConfig locationPrompt = new PromptConfig();
            locationPrompt.IsCancellable = true;
            locationPrompt.Title = "Current Location: ";
            locationPrompt.Text = location;
            PromptResult promptResult = await UserDialogs.Instance.PromptAsync(locationPrompt);

            if (promptResult.Ok && (promptResult.Text != ""))
            {
                BreakTable newBreak = new BreakTable();
                List<ShiftTable> activeShift = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
                if (activeShift.Count > 0) 
                {
                    newBreak.ShiftKey = activeShift[0].ServerKey;
                    newBreak.StartDate = DateTime.Now.ToString();
                    newBreak.ActiveBreak = true;
                    newBreak.StartLocation = location;
                    db.Insert(newBreak);

                    RestAPI = new RestService();
                    int result = await RestAPI.QueryBreak(false, newBreak);

                    if (result > 0)
                    {
                        newBreak.ServerId = result;
                        db.Update(newBreak);
                    }
                    else
                    {
                        var tbl = db.GetTableInfo("BreakOffline");

                        if (tbl == null)
                            db.CreateTable<BreakOffline>();

                        BreakOffline offline = new BreakOffline();

                        offline.BreakKey = newBreak.Key;
                        offline.StartOffline = true;
                        offline.EndOffline = false;

                        db.Insert(offline);
                    }

                    return true;
                }
                return false;
            }
            return false;

        }

        public async Task<Geolocation> GetLatAndLong()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            Geolocation results = new Geolocation();

            try
            {
                var position = await locator.GetPositionAsync(timeoutMilliseconds: 100000);

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

        internal void UpdateVehicleInfo(VehicleTable editedVehicle)
        {
            db.Update(editedVehicle);
        }

        internal void Logout()
        {
            db.Query<UserTable>("DELETE FROM [UserTable]");
        }

        internal async Task<bool> StartShift()
        {
            RestAPI = new RestService();
            Geolocation geoCords = new Geolocation();
            geoCords = await GetLatAndLong();

            string location = await RestAPI.GetLocation(geoCords);

            PromptConfig locationPrompt = new PromptConfig();
            locationPrompt.IsCancellable = true;
            locationPrompt.Title = "Current Location: ";
            locationPrompt.Text = location;
            PromptResult promptResult = await UserDialogs.Instance.PromptAsync(locationPrompt);

            if(promptResult.Ok && (promptResult.Text != ""))
            {
                ShiftTable shift = new ShiftTable();
                shift.StartDate = DateTime.Now.ToString();
                shift.StartLat = geoCords.Latitude;
                shift.StartLong = geoCords.Longitude;
                shift.ActiveShift = true;
                shift.StartLocation = promptResult.Text;
                db.Insert(shift);

                List<UserTable> user = db.Query<UserTable>("SELECT * FROM [UserTable]");
                List<CompanyTable> company = db.Query<CompanyTable>("Select * FROM [CompanyTable]");

                var result = await RestAPI.QueryShift(shift, false, user[0].DriverId, company[0].Key);

                if (result > 0)
                {
                    shift.ServerKey = result;
                    db.Update(shift);
                }
                else
                {
                    var tbl = db.GetTableInfo("ShiftOffline");

                    if (tbl.Count == 0)
                        db.CreateTable<ShiftOffline>();

                    ShiftOffline offline = new ShiftOffline();

                    offline.ShiftKey = shift.Key;
                    offline.StartOffline = true;
                    offline.EndOffline = false;

                    db.Insert(offline);
                }
                return true;
            }
            await Application.Current.MainPage.DisplayAlert("Error", "Unable to start shift", "Understood");
            return false;
        }

        private bool CheckActiveShiftIsCorrect(bool isShift, List<ShiftTable> activeShifts = null, List<DriveTable> activeDrives = null)
        {
            if (isShift)
            {
                if (activeShifts.Count == 0)
                {
                    Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.NoActiveShifts, Resource.DisplayAlertOkay);
                    return false;
                }
                else if (activeShifts.Count > 1)
                {
                    Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.MoreOneActiveShift, Resource.DisplayAlertOkay);
                    return false;
                }
                return true;
            }
            else
            {
                if (activeDrives.Count == 0)
                {
                    Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.NoActiveDrives, Resource.DisplayAlertOkay);
                    return false;
                }
                else if (activeDrives.Count > 1)
                {
                    Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, Resource.MoreOneActiveDrive, Resource.DisplayAlertOkay);
                    return false;
                }
                return true;
            }
        }

        internal async Task<bool> StopShift(DateTime date)
        {
            RestAPI = new RestService();
            Geolocation geoCords = new Geolocation();
            geoCords = await GetLatAndLong();

            string location = await RestAPI.GetLocation(geoCords);

            PromptConfig locationPrompt = new PromptConfig();
            locationPrompt.IsCancellable = true;
            locationPrompt.Title = "Current Location: ";
            locationPrompt.Text = location;
            PromptResult promptResult = await UserDialogs.Instance.PromptAsync(locationPrompt);

            if (promptResult.Ok && (promptResult.Text != ""))
            {
                List<ShiftTable> activeShifts = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [ActiveShift] == 1");
                if (CheckActiveShiftIsCorrect(true, activeShifts, null))
                {
                    ShiftTable activeShift = activeShifts[0];
                    activeShift.EndDate = date.ToString();
                    activeShift.ActiveShift = false;
                    activeShift.EndLat = geoCords.Latitude;
                    activeShift.EndLong = geoCords.Longitude;
                    activeShift.EndLocation = promptResult.Text;
                    db.Update(activeShift);

                    if (activeShift.ServerKey > 0)
                    {
                        RestAPI = new RestService();
                        var result = await RestAPI.QueryShift(activeShift, true);

                        if (result > 0)
                        {
                            return true;
                        }
                    }

                    var tbl = db.GetTableInfo("ShiftOffline");

                    if (tbl.Count == 0)
                    {
                        db.CreateTable<ShiftOffline>();

                        ShiftOffline offline = new ShiftOffline();

                        offline.ShiftKey = activeShift.Key;
                        offline.StartOffline = false;
                        offline.EndOffline = true;
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
                            }
                        }

                        if (num == 0)
                        {
                            ShiftOffline offline = new ShiftOffline();

                            offline.ShiftKey = activeShift.Key;
                            offline.StartOffline = false;
                            offline.EndOffline = true;
                            db.Insert(offline);
                        }
                        return true;
                    }
                }
            }
            return false;

            
        }

        internal DriveTable GetCurrentDriveShift()
        {
            List<DriveTable> activeDrives = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ActiveVehicle] == 1 LIMIT 1");

            DriveTable activeDrive = activeDrives[0];
            return activeDrive;
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

        internal int SaveShift(DateTime shiftStart, DateTime shiftEnd, List<DriveTable> driveList)//, double latStart, double longStart, double latEnd, double longEnd)
        {
            ShiftTable shift = new ShiftTable();

            shift.StartDate = shiftStart.ToString();
            shift.EndDate = shiftEnd.ToString();
            //shift.StartLat = latStart;
            //shift.StartLong = longStart;
            //shift.EndLat = latEnd;
            //shift.EndLong = longEnd;
            shift.ActiveShift = false;
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

            List<ShiftTable> shiftList = new List<ShiftTable>();
            List<ExportShift> exportList = new List<ExportShift>();

            shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartDate] <= (SELECT DATE('now', '-7 day'))");

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

            List<BreakTable> breakList = new List<BreakTable>();
            List<ShiftTable> shiftList = new List<ShiftTable>();
            List<ExportBreak> exportList = new List<ExportBreak>();

            shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartDate] <= (SELECT DATE('now', '-7 day'))");

            foreach (ShiftTable shiftData in shiftList)
            {
                breakList = db.Query<BreakTable>("SELECT * FROM [BreakTable] WHERE [ShiftKey] = " + shiftData.Key);

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

            List<NoteTable> noteList = new List<NoteTable>();
            List<ShiftTable> shiftList = new List<ShiftTable>();
            List<ExportNote> exportList = new List<ExportNote>();

            shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartDate] <= (SELECT DATE('now', '-7 day'))");

            foreach (ShiftTable shiftData in shiftList)
            {
                noteList = db.Query<NoteTable>("SELECT * FROM [NoteTable] WHERE [ShiftKey] = " + shiftData.Key);

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

            List<DriveTable> driveList = new List<DriveTable>();
            List<VehicleTable> vehicleList = new List<VehicleTable>();
            List<ShiftTable> shiftList = new List<ShiftTable>();
            List<ExportVehicle> exportList = new List<ExportVehicle>();
            shiftList = db.Query<ShiftTable>("SELECT * FROM [ShiftTable] WHERE [StartDate] <= (SELECT DATE('now', '-7 day'))");

            foreach (ShiftTable shiftData in shiftList)
            {
                driveList = db.Query<DriveTable>("SELECT * FROM [DriveTable] WHERE [ShiftKey] = " + shiftData.Key);

                foreach (DriveTable driveData in driveList)
                {
                    vehicleList = db.Query<VehicleTable>("SELECT * FROM [VehicleTable] WHERE [Key] = " + driveData.VehicleKey);
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
                            int result = await RestAPI.QueryShift(shift, false, user.DriverId, drive.Key);

                            if (result > 0)
                            {
                                shift.ServerKey = result;
                                db.Update(shift);
                            }
                            else
                                uploadErrors++;
                        }
                        if (item.EndOffline)
                        {
                            int result = await RestAPI.QueryShift(shift, true);

                            if (result != 0)
                                uploadErrors++;
                        }

                        if (uploadErrors == 0)
                            db.Query<ShiftOffline>("DELETE FROM [ShiftOffline] WHERE [ShiftKey] = " + shift.Key);
                    }
                }

                List<ShiftOffline> empty = db.Query<ShiftOffline>("SELECT * FROM [ShiftOffline]");

                if (empty.Count == 0)
                    db.Query<ShiftOffline>("DROP TABLE [ShiftOffline]");
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
                            int result = await RestAPI.QueryDrive(false, drive);

                            if (result > 0)
                            {
                                drive.ServerId = result;
                                db.Update(drive);
                            }
                            else
                                uploadErrors++;
                        }
                        if (item.EndOffline)
                        {
                            int result = await RestAPI.QueryDrive(true, drive);

                            if (result != 0)
                                uploadErrors++;
                        }

                        if (uploadErrors == 0)
                            db.Query<DriveOffline>("DELETE FROM [DriveOffline] WHERE [DriveKey] = " + drive.Key);
                    }
                }

                List<DriveOffline> empty = db.Query<DriveOffline>("SELECT * FROM [DriveOffline]");

                if (empty.Count == 0)
                    db.Query<DriveOffline>("DROP TABLE [DriveOffline]");
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
                            int result = await RestAPI.QueryBreak(false, breakValue);

                            if (result > 0)
                            {
                                breakValue.ServerId = result;
                                db.Update(result);
                            }
                            else
                                uploadErrors++;
                        }
                        if (item.EndOffline)
                        {
                            int result = await RestAPI.QueryBreak(true, breakValue);

                            if (result != 0)
                                uploadErrors++;
                        }

                        if (uploadErrors == 0)
                            db.Query<BreakOffline>("DELETE FROM [BreakOffline] WHERE [BreakKey] = " + breakValue.Key);
                    }
                }

                List<BreakOffline> empty = db.Query<BreakOffline>("SELECT * FROM [BreakOffline]");

                if (empty.Count == 0)
                    db.Query<BreakOffline>("DROP TABLE [BreakOffline]");
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

                        int result = await RestAPI.InsertNote(note);

                        if (result > 0)
                        {
                            db.Query<NoteOffline>("DELETE FROM [NoteOffline] WHERE [NoteKey] = " + note.Key);
                        }
                    }
                }

                List<NoteOffline> empty = db.Query<NoteOffline>("SELECT * FROM [NoteOffline]");

                if (empty.Count == 0)
                    db.Query<NoteOffline>("DROP TABLE [NoteOffline]");
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

                        bool result = await RestAPI.QueryAddVehicle(vehicle);

                        if (result)
                        {
                            db.Query<VehicleOffline>("DELETE FROM [VehicleOffline] WHERE [VehicleKey] = " + vehicle.Key);
                        }
                    }
                }

                List<VehicleOffline> empty = db.Query<VehicleOffline>("SELECT * FROM [VehicleOffline]");

                if (empty.Count == 0)
                    db.Query<VehicleOffline>("DROP TABLE [VehicleOffline]");
            }

            var userChangesExist = db.GetTableInfo("UserOffline");

            if (userChangesExist.Count > 0)
            {
                List<UserOffline> userChanges = db.Query<UserOffline>("SELECT * FROM [UserOffline]");

                List<UserOffline> users = db.Query<UserOffline>("SELECT * FROM [UserOffline]");

                if (users != null)
                {
                    foreach (UserOffline item in users)
                    {
                        UserTable user = db.Query<UserTable>("SELECT * FROM [UserTable] WHERE [Key] = " + item.UserKey)[0];

                        bool result = await RestAPI.QueryUpdateProfile(user);

                        if (result)
                        {
                            db.Query<UserOffline>("DELETE FROM [VehicleOffline] WHERE [UserKey] = " + user.Id);
                        }
                    }
                }

                List<UserOffline> empty = db.Query<UserOffline>("SELECT * FROM [UserOffline]");

                if (empty.Count == 0)
                    db.Query<UserOffline>("DROP TABLE [VehicleOffline");
            }

            return true;
        }

    }
}
