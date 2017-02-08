using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using ModernHttpClient;
using System.Net.Http;
using System.Text;
using System.Dynamic;

namespace Hubo
{
    class RestService
    {
        HttpClient client;
        DatabaseService db;

        public RestService()
        {
            client = new HttpClient(new NativeMessageHandler());
            this.db = new DatabaseService();
        }

        internal async Task<bool> Login(string username, string password)
        {
            string url = GetBaseUrl() + Constants.REST_URL_LOGIN;
            string contentType = Constants.CONTENT_TYPE;

            LoginRequestModel loginModel = new LoginRequestModel();
            //loginModel.usernameOrEmailAddress = username;
            //loginModel.password = password;
            loginModel.usernameOrEmailAddress = "ben@triotech.co.nz";
            loginModel.password = "tazmania";

            string json = JsonConvert.SerializeObject(loginModel);

            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                LoginUserResponse result = new LoginUserResponse();
                result = JsonConvert.DeserializeObject<LoginUserResponse>(response.Content.ReadAsStringAsync().Result);

                if (result.Success)
                {
                    UserTable user = new UserTable();
                    user.Id = result.Result.Id;
                    user.UserName = username;
                    user.Token = result.Result.Token;
                    user.DriverId = result.Result.DriverId;
                    user.Email = result.Result.EmailAddress;
                    user.FirstName = result.Result.FirstName;
                    user.LastName = result.Result.Surname;
                    db.InsertUser(user);

                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.Token);

                    int totalDetails = await GetUser(user, user.Token);
                    switch (totalDetails)
                    {
                        case -3:
                            await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to clear tables for new user", Resource.DisplayAlertOkay);
                            return false;
                        case -2:
                            await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Invalid User Details", Resource.DisplayAlertOkay);
                            return false;
                        case -1:
                            await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to get user, company and vehicle details", Resource.DisplayAlertOkay);
                            return false;
                        case 1:
                            await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to get all vehicle details", Resource.DisplayAlertOkay);
                            return false;
                        case 2:
                            await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to get all company and vehicle details", Resource.DisplayAlertOkay);
                            return false;
                        case 3:
                            break;
                        default:
                            return false;
                    }

                    int totalShifts = await GetShifts(user.DriverId, user.Token);
                    switch (totalShifts)
                    {
                        case -4:
                            await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to process user shifts", Resource.DisplayAlertOkay);
                            return false;
                        case -3:
                            await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to clear tables for new user", Resource.DisplayAlertOkay);
                            return false;
                        case -2:
                            await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Invalid User Details", Resource.DisplayAlertOkay);
                            return false;
                        case -1:
                            await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to get user, company and vehicle details", Resource.DisplayAlertOkay);
                            return false;
                        case 3:
                            break;
                        default:
                            return false;
                    }

                    return true;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Username/Password is incorrect, please try again", Resource.DisplayAlertOkay);
                    return false;
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "There was an error communicating with the server", Resource.DisplayAlertOkay);
                return false;
            }
        }

        private async Task<int> GetShifts(int id, string token)
        {
            string urlShift = GetBaseUrl() + Constants.REST_URL_GETSHIFTDETAILS;
            string urlDrive = GetBaseUrl() + Constants.REST_URL_GETDRIVEDETAILS;
            string urlBreak = GetBaseUrl() + Constants.REST_URL_GETBREAKDETAILS;
            string urlNote = GetBaseUrl() + Constants.REST_URL_GETNOTEDETAILS;

            if (!db.ClearTablesForUserShifts())
                return -3;

            if (id < 0)
                return -2;

            var shiftGet = new HttpRequestMessage(HttpMethod.Get, urlShift);
            shiftGet.Headers.Add("DriverId", id.ToString());

            var shiftResponse = await client.SendAsync(shiftGet);

            if (shiftResponse.IsSuccessStatusCode)
            {
                LoginShiftResponse shiftDetails = new LoginShiftResponse();
                shiftDetails = JsonConvert.DeserializeObject<LoginShiftResponse>(shiftResponse.Content.ReadAsStringAsync().Result);

                ShiftTable shift = new ShiftTable();
                NoteTable startNote = new NoteTable();
                NoteTable endNote = new NoteTable();

                foreach (ShiftResponseModel shiftItem in shiftDetails.Shifts)
                {
                    shift.ActiveShift = shiftItem.State;
                    shift.StartTime = shiftItem.StartDate;
                    shift.EndTime = shiftItem.EndDate;
                    shift.ServerKey = shiftItem.ServerKey;

                    int shiftId = db.InsertUserShifts(shift);

                    if (shiftId < 0)
                        return -4;

                    var noteGet = new HttpRequestMessage(HttpMethod.Get, urlNote);
                    noteGet.Headers.Add("ShiftId", shiftId.ToString());

                    var noteResponse = await client.SendAsync(noteGet);

                    if (noteResponse.IsSuccessStatusCode)
                    {
                        LoginNoteResponse noteDetails = new LoginNoteResponse();
                        noteDetails = JsonConvert.DeserializeObject<LoginNoteResponse>(noteResponse.Content.ReadAsStringAsync().Result);

                        NoteTable note = new NoteTable();

                        shift.StartShiftNoteId = noteDetails.Notes[0].Id;

                        if (!shift.ActiveShift)
                            shift.StopShiftNoteId = noteDetails.Notes[noteDetails.Notes.Count - 1].Id;

                        foreach (NoteResponseModel noteItem in noteDetails.Notes)
                        {
                            note.Note = noteItem.NoteText;
                            note.Date = noteItem.TimeStamp;
                            note.ShiftKey = noteItem.ShiftId;
                            note.Hubo = noteItem.Hubo;
                            note.StandAloneNote = noteItem.StandAloneNote;

                            db.InsertUserNotes(note);
                        }

                        var driveGet = new HttpRequestMessage(HttpMethod.Get, urlDrive);
                        driveGet.Headers.Add("ShiftId", shiftId.ToString());

                        var driveResponse = await client.SendAsync(driveGet);

                        if (driveResponse.IsSuccessStatusCode)
                        {
                            LoginDriveResponse driveDetails = new LoginDriveResponse();
                            driveDetails = JsonConvert.DeserializeObject<LoginDriveResponse>(driveResponse.Content.ReadAsStringAsync().Result);

                            DriveTable drive = new DriveTable();

                            foreach (DriveResponseModel driveItem in driveDetails.Drives)
                            {
                                drive.ShiftKey = shiftId;
                                drive.VehicleKey = driveItem.VehicleId;
                                drive.ActiveVehicle = driveItem.State;

                                List<int> driveNotes = new List<int>();

                                for (int i = 0; i < noteDetails.Notes.Count - 1; i++)
                                {
                                    if (noteDetails.Notes[i].DrivingShiftId == driveItem.Id && noteDetails.Notes[i].BreakId == 0)
                                        driveNotes.Add(i);
                                }

                                drive.StartNoteKey = noteDetails.Notes[driveNotes[0]].Id;

                                if (!drive.ActiveVehicle)
                                    drive.EndNoteKey = noteDetails.Notes[driveNotes[1]].Id;

                                db.InsertUserDrives(drive);

                                var breakGet = new HttpRequestMessage(HttpMethod.Get, urlBreak);
                                breakGet.Headers.Add("DriveShiftId", drive.Key.ToString());

                                var breakResponse = await client.SendAsync(breakGet);

                                if (breakResponse.IsSuccessStatusCode)
                                {
                                    LoginBreakResponse breakDetails = new LoginBreakResponse();
                                    breakDetails = JsonConvert.DeserializeObject<LoginBreakResponse>(breakResponse.Content.ReadAsStringAsync().Result);

                                    BreakTable breakTable = new BreakTable();

                                    foreach (BreakResponseModel breakItem in breakDetails.Breaks)
                                    {
                                        breakTable.ShiftKey = shiftId;
                                        breakTable.ActiveBreak = breakItem.State;

                                        List<int> breakNotes = new List<int>();

                                        for (int i = 0; i < noteDetails.Notes.Count - 1; i++)
                                        {
                                            if (noteDetails.Notes[i].BreakId == breakItem.Id)
                                                breakNotes.Add(i);
                                        }

                                        breakTable.StartNoteKey = noteDetails.Notes[breakNotes[0]].Id;
                                        breakTable.StartTime = noteDetails.Notes[breakNotes[0]].TimeStamp;

                                        if (!breakTable.ActiveBreak)
                                        {
                                            breakTable.StopNoteKey = noteDetails.Notes[breakNotes[0]].Id;
                                            breakTable.EndTime = noteDetails.Notes[breakNotes[1]].TimeStamp;
                                        }

                                        db.InsertUserBreaks(breakTable);
                                    }
                                }
                                else
                                {
                                    return 3;
                                }
                            }
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    else
                    {
                        return 1;
                    }
                }

                return 4;
            }
            else
            {
                return -1;
            }
        }

        internal async Task<int> GetUser(UserTable user, string token)
        {
            string urlUser = GetBaseUrl() + Constants.REST_URL_GETUSERDETAILS;
            string urlCompany = GetBaseUrl() + Constants.REST_URL_GETCOMPANYDETAILS;
            string urlVehicle = GetBaseUrl() + Constants.REST_URL_GETVEHICLEDETAILS;

            if (!db.ClearTablesForNewUser())
                return -3;

            if (user.Id < 0)
                return -2;

            var userGet = new HttpRequestMessage(HttpMethod.Get, urlUser);
            userGet.Headers.Add("UserId", user.Id.ToString());

            var userResponse = await client.SendAsync(userGet);

            if (userResponse.IsSuccessStatusCode)
            {
                LoginUserDetailsResponse userDetails = new LoginUserDetailsResponse();
                userDetails = JsonConvert.DeserializeObject<LoginUserDetailsResponse>(userResponse.Content.ReadAsStringAsync().Result);

                CompanyTable userCompany = new CompanyTable();
                VehicleTable userVehicles = new VehicleTable();

                user.Phone = userDetails.Result.PhoneNumber;
                user.Address1 = userDetails.Result.Address1;
                user.Address2 = userDetails.Result.Address2;
                user.Address3 = userDetails.Result.Address3;
                user.PostCode = userDetails.Result.PostCode;
                user.City = userDetails.Result.City;
                user.Country = userDetails.Result.Country;

                db.UpdateUser(user);

                var companyGet = new HttpRequestMessage(HttpMethod.Get, urlCompany);
                companyGet.Headers.Add("DriverId", user.DriverId.ToString());

                var companyResponse = await client.SendAsync(companyGet);

                if (companyResponse.IsSuccessStatusCode)
                {
                    LoginCompanyResponse companyDetails = new LoginCompanyResponse();
                    companyDetails = JsonConvert.DeserializeObject<LoginCompanyResponse>(companyResponse.Content.ReadAsStringAsync().Result);

                    foreach (CompanyTable companyItem in companyDetails.Companies)
                    {
                        companyItem.DriverId = user.DriverId;
                        db.InsertUserComapany(companyItem);

                        var vehicleGet = new HttpRequestMessage(HttpMethod.Get, urlVehicle);
                        vehicleGet.Headers.Add("CompanyId", companyItem.Key.ToString());

                        var vehicleResponse = await client.SendAsync(vehicleGet);

                        if (vehicleResponse.IsSuccessStatusCode)
                        {
                            LoginVehicleResponse vehicleDetails = new LoginVehicleResponse();
                            vehicleDetails = JsonConvert.DeserializeObject<LoginVehicleResponse>(vehicleResponse.Content.ReadAsStringAsync().Result);

                            foreach (VehicleTable vehicleItem in vehicleDetails.Vehicles)
                            {
                                db.InsertUserVehicles(vehicleItem);
                            }
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
                else
                {
                    return 2;
                }

                return 3;
            }
            else
            {
                return -1;
            }
        }

        private string GetBaseUrl()
        {
            return "http://test.triotech.co.nz/huboportal/api";
        }

        internal async Task<bool> QueryUpdateUserInfo(UserTable user)
        {
            //TODO: Code to communicate with server to update user info
            string url = GetBaseUrl() + Constants.REST_URL_ADDVEHICLE;
            string contentType = Constants.CONTENT_TYPE;

            dynamic vehicleModel = new ExpandoObject();
            vehicleModel.registrationNo = user.DriverId;

            string json = JsonConvert.SerializeObject(vehicleModel);
            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                db.UpdateUserInfo(user);
                return true;
            }
            else
            {
                db.UserOffline(user);
                return false;
            }
        }

        internal async Task<bool> QueryAddVehicle(VehicleTable vehicle)
        {
            string url = GetBaseUrl() + Constants.REST_URL_ADDVEHICLE;
            string contentType = Constants.CONTENT_TYPE;

            VehicleRequestModel vehicleModel = new VehicleRequestModel();
            vehicleModel.registrationNo = vehicle.Registration;
            vehicleModel.makeModel = vehicle.MakeModel;
            vehicleModel.startingOdometer = vehicle.StartingOdometer;
            vehicleModel.currentOdometer = vehicle.StartingOdometer;
            vehicleModel.companyId = 1;

            string json = JsonConvert.SerializeObject(vehicleModel);
            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                UserResponse result = new UserResponse();
                result = JsonConvert.DeserializeObject<UserResponse>(response.Content.ReadAsStringAsync().Result);
                if (result.Success)
                {
                    return true;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to register vehicle, please try again", Resource.DisplayAlertOkay);
                    return false;
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "There was an error communicating with the server", Resource.DisplayAlertOkay);
                return false;
            }
        }

        internal async Task<int> QueryShift(ShiftTable shift, bool shiftStarted, NoteTable note, int userId, int companyId)
        {
            string contentType = Constants.CONTENT_TYPE;
            string url;

            string json;

            if (shiftStarted)
            {
                url = GetBaseUrl() + Constants.REST_URL_ADDSHIFTEND;
                EndShiftModel shiftModel = new EndShiftModel();

                shiftModel.id = shift.ServerKey;
                shiftModel.endDate = note.Date;
                shiftModel.endLocationLat = note.Latitude;
                shiftModel.endLocationLong = note.Longitude;

                json = JsonConvert.SerializeObject(shiftModel);
            }
            else
            {
                url = GetBaseUrl() + Constants.REST_URL_ADDSHIFTSTART;

                StartShiftModel shiftModel = new StartShiftModel();

                shiftModel.driverId = userId;
                shiftModel.companyId = companyId;
                shiftModel.startDate = note.Date;
                shiftModel.startLocationLat = note.Latitude;
                shiftModel.startLocationLong = note.Longitude;

                json = JsonConvert.SerializeObject(shiftModel);
            }

            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);


            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                UserResponse result = new UserResponse();
                result = JsonConvert.DeserializeObject<UserResponse>(response.Content.ReadAsStringAsync().Result);
                if (result.Success)
                {
                    if (int.Parse(result.Result) > 0)
                    {
                        return int.Parse(result.Result);
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to register shift, please try again", Resource.DisplayAlertOkay);
                        return -2;
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to register shift, please try again", Resource.DisplayAlertOkay);
                    return -2;
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "There was an error communicating with the server", Resource.DisplayAlertOkay);
                return -2;
            }
        }

        internal async Task<int> QueryDrive(bool driveStarted, DriveTable drive, string date)
        {
            if (!driveStarted)
            {
                string url = GetBaseUrl() + Constants.REST_URL_ADDDRIVESTART;
                string contentType = Constants.CONTENT_TYPE;

                DriveModel driveModel = new DriveModel();
                driveModel.shiftId = drive.ShiftKey;
                driveModel.timeStamp = date;
                driveModel.vehicleId = drive.VehicleKey;

                string json = JsonConvert.SerializeObject(driveModel);
                HttpContent content = new StringContent(json, Encoding.UTF8, contentType);
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                string url = GetBaseUrl() + Constants.REST_URL_ADDDRIVEEND;

                var stopDrivePut = new HttpRequestMessage(HttpMethod.Put, url);
                stopDrivePut.Headers.Add("DriverShiftId", drive.Key.ToString());

                var response = await client.SendAsync(stopDrivePut);

                if (response.IsSuccessStatusCode)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        internal async Task<int> QueryBreak(bool breakStarted, int driveShiftId, NoteTable note)
        {
            if (!breakStarted)
            {
                string url = GetBaseUrl() + Constants.REST_URL_ADDDRIVESTART;
                string contentType = Constants.CONTENT_TYPE;

                BreakModel driveModel = new BreakModel();
                driveModel.driveShiftId = driveShiftId;
                driveModel.timeStamp = note.Date;
                driveModel.geoDataId = 4;

                string json = JsonConvert.SerializeObject(driveModel);
                HttpContent content = new StringContent(json, Encoding.UTF8, contentType);
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                string url = GetBaseUrl() + Constants.REST_URL_ADDDRIVEEND;

                var stopDrivePut = new HttpRequestMessage(HttpMethod.Put, url);
                stopDrivePut.Headers.Add("DriverShiftId", driveShiftId.ToString());

                var response = await client.SendAsync(stopDrivePut);

                if (response.IsSuccessStatusCode)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        internal async Task<int> InsertGeoData(int driveShiftId, NoteTable note)
        {
            string url = GetBaseUrl() + Constants.REST_URL_INSERTGEODATA;
            string contentType = Constants.CONTENT_TYPE;

            InsertGeoModel geoModel = new InsertGeoModel();
            geoModel.drivingShiftId = driveShiftId;
            geoModel.timeStamp = note.Date;
            geoModel.latitude = note.Latitude;
            geoModel.longitude = note.Longitude;

            string json = JsonConvert.SerializeObject(geoModel);
            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                InsertGeoResponse result = JsonConvert.DeserializeObject<InsertGeoResponse>(response.Content.ReadAsStringAsync().Result);

                if (result.Success)
                {
                    return result.Result;
                }
                else
                {
                    return -2;
                }
            }

            return -1;
        }

        internal async Task<int> InsertNote(int breakId, int drivingShiftId, int geoData, NoteTable note)
        {
            string url = GetBaseUrl() + Constants.REST_URL_INSERTNOTE;
            string contentType = Constants.CONTENT_TYPE;

            InsertNoteModel noteModel = new InsertNoteModel();
            noteModel.shiftId = note.ShiftKey;
            noteModel.breakId = breakId;
            noteModel.drivingShiftId = drivingShiftId;
            noteModel.noteText = note.Note;
            noteModel.geoDataLink = geoData;
            noteModel.timeStamp = note.Date;
            noteModel.hubo = note.Hubo;

            string json = JsonConvert.SerializeObject(noteModel);
            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                InsertNoteResponse result = JsonConvert.DeserializeObject<InsertNoteResponse>(response.Content.ReadAsStringAsync().Result);

                if (result.Success)
                {
                    return result.Result;
                }
                else
                {
                    return -2;
                }
            }
            else
            {
                return -1;
            }
        }
    }
}