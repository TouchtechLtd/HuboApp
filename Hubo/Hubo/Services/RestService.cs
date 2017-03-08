﻿// <copyright file="RestService.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.ProjectOxford.Vision;
    using Microsoft.ProjectOxford.Vision.Contract;
    using ModernHttpClient;
    using Newtonsoft.Json;
    using Plugin.Media.Abstractions;

    internal class RestService
    {
        private readonly HttpClient client;
        private readonly DatabaseService db;
        private string accessToken;

        public RestService()
        {
            client = new HttpClient(new NativeMessageHandler());
            this.db = new DatabaseService();
            accessToken = "Bearer " + db.GetUserToken();
        }

        internal async Task<bool> Login(string username, string password)
        {
            string url = GetBaseUrl() + Constants.REST_URL_LOGIN;
            string contentType = Constants.CONTENT_TYPE;

            if (!db.ClearTablesForNewUser())
            {
                return false;
            }

            LoginRequestModel loginModel = new LoginRequestModel();

            // loginModel.usernameOrEmailAddress = username;
            // loginModel.password = password;
            loginModel.usernameOrEmailAddress = "bsuarez";
            loginModel.password = "tazmania";

            string json = JsonConvert.SerializeObject(loginModel);

            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                LoginUserResponse result = JsonConvert.DeserializeObject<LoginUserResponse>(response.Content.ReadAsStringAsync().Result);

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

                    accessToken = "Bearer " + db.GetUserToken();

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

        internal async Task<string> GetLocation(Geolocation geoCoordinates)
        {
            if (geoCoordinates != null)
            {
                var googleGet = new HttpRequestMessage(HttpMethod.Get, Constants.REST_URL_GOOGLEAPI + "latlng=" + geoCoordinates.Latitude + "," + geoCoordinates.Longitude + "&key=" + Configuration.GoogleMapsApiKey);
                var googleResponse = await client.SendAsync(googleGet);
                string address;

                if (googleResponse.IsSuccessStatusCode)
                {
                    GoogleGeoResponse geoResponse = JsonConvert.DeserializeObject<GoogleGeoResponse>(googleResponse.Content.ReadAsStringAsync().Result);

                    if (geoResponse.Results[0] != null)
                    {
                        address = geoResponse.Results[0].FormattedAddress;
                        return address;
                    }
                }
            }

            return string.Empty;
        }

        internal async Task<int> GetShifts(int id)
        {
            string urlShift = GetBaseUrl() + Constants.REST_URL_GETSHIFTDETAILS;
            string urlDrive = GetBaseUrl() + Constants.REST_URL_GETDRIVEDETAILS;
            string urlBreak = GetBaseUrl() + Constants.REST_URL_GETBREAKDETAILS;
            string urlNote = GetBaseUrl() + Constants.REST_URL_GETNOTEDETAILS;

            if (!db.ClearTablesForUserShifts())
            {
                return -3;
            }

            if (id < 0)
            {
                return -2;
            }

            var shiftGet = new HttpRequestMessage(HttpMethod.Get, urlShift);
            shiftGet.Headers.Add("DriverId", id.ToString());

            var shiftResponse = await client.SendAsync(shiftGet);

            if (shiftResponse.IsSuccessStatusCode)
            {
                LoginShiftResponse shiftDetails = JsonConvert.DeserializeObject<LoginShiftResponse>(shiftResponse.Content.ReadAsStringAsync().Result);

                if (shiftDetails.Success)
                {
                    ShiftTable shift = new ShiftTable();

                    foreach (ShiftResponseModel shiftItem in shiftDetails.Shifts)
                    {
                        shift.ServerKey = shiftItem.Id;
                        shift.StartDate = shiftItem.StartDate;
                        shift.EndDate = shiftItem.EndDate;
                        shift.StartLat = shiftItem.StartLocationLat;
                        shift.StartLong = shiftItem.StartLocationLong;
                        shift.EndLat = shiftItem.EndLocationLat;
                        shift.EndLong = shiftItem.EndLocationLong;
                        shift.ActiveShift = shiftItem.IsActive;

                        ShiftTable shiftId = new ShiftTable();
                        shiftId = db.InsertUserShifts(shift);

                        if (shiftId == null)
                        {
                            return -4;
                        }

                        var noteGet = new HttpRequestMessage(HttpMethod.Get, urlNote);
                        noteGet.Headers.Add("ShiftId", shiftId.ServerKey.ToString());

                        var noteResponse = await client.SendAsync(noteGet);

                        if (noteResponse.IsSuccessStatusCode)
                        {
                            LoginNoteResponse noteDetails = JsonConvert.DeserializeObject<LoginNoteResponse>(noteResponse.Content.ReadAsStringAsync().Result);

                            if (noteDetails.Success)
                            {
                                NoteTable note = new NoteTable();

                                foreach (NoteResponseModel noteItem in noteDetails.Notes)
                                {
                                    note.Note = noteItem.NoteText;
                                    note.Date = noteItem.TimeStamp;
                                    note.ShiftKey = shiftId.Key;

                                    db.InsertUserNotes(note);
                                }
                            }
                            else
                            {
                                return 1;
                            }
                        }
                        else
                        {
                            return 1;
                        }

                        var driveGet = new HttpRequestMessage(HttpMethod.Get, urlDrive);
                        driveGet.Headers.Add("ShiftId", shiftId.ServerKey.ToString());

                        var driveResponse = await client.SendAsync(driveGet);

                        if (driveResponse.IsSuccessStatusCode)
                        {
                            LoginDriveResponse driveDetails = JsonConvert.DeserializeObject<LoginDriveResponse>(driveResponse.Content.ReadAsStringAsync().Result);

                            if (driveDetails.Success)
                            {
                                DriveTable drive = new DriveTable();

                                foreach (DriveResponseModel driveItem in driveDetails.Drives)
                                {
                                    drive.ServerId = driveItem.Id;
                                    drive.ShiftKey = shiftId.Key;
                                    drive.StartDate = driveItem.StartDrivingDateTime;
                                    drive.EndDate = driveItem.StopDrivingDateTime;
                                    drive.StartHubo = driveItem.StartHubo;
                                    drive.EndHubo = driveItem.StopHubo;
                                    drive.ActiveVehicle = driveItem.IsActive;
                                    drive.VehicleKey = driveItem.VehicleId;
                                    db.InsertUserDrives(drive);
                                }
                            }
                            else
                            {
                                return 2;
                            }
                        }
                        else
                        {
                            return 2;
                        }

                        var breakGet = new HttpRequestMessage(HttpMethod.Get, urlBreak);
                        breakGet.Headers.Add("ShiftId", shiftId.ServerKey.ToString());

                        var breakResponse = await client.SendAsync(breakGet);

                        if (breakResponse.IsSuccessStatusCode)
                        {
                            LoginBreakResponse breakDetails = JsonConvert.DeserializeObject<LoginBreakResponse>(breakResponse.Content.ReadAsStringAsync().Result);

                            if (breakDetails.Success)
                            {
                                BreakTable breakTable = new BreakTable();

                                foreach (BreakResponseModel breakItem in breakDetails.Breaks)
                                {
                                    breakTable.ShiftKey = shiftId.Key;
                                    breakTable.ActiveBreak = breakItem.IsActive;
                                    breakTable.StartDate = breakItem.StartBreakDateTime;
                                    breakTable.EndDate = breakItem.StopBreakDateTime;
                                    breakTable.StartLocation = breakItem.StartBreakLocation;
                                    breakTable.EndLocation = breakItem.StopBreakLocation;
                                    breakTable.ServerId = breakItem.Id;

                                    db.InsertUserBreaks(breakTable);
                                }
                            }
                            else
                            {
                                return 3;
                            }
                        }
                        else
                        {
                            return 3;
                        }
                    }

                    return 4;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }

        internal async Task<int> GetUser(UserTable user)
        {
            string urlUser = GetBaseUrl() + Constants.REST_URL_GETUSERDETAILS;
            string urlCompany = GetBaseUrl() + Constants.REST_URL_GETCOMPANYDETAILS;
            string urlVehicle = GetBaseUrl() + Constants.REST_URL_GETVEHICLEDETAILS;

            if (user.Id < 0)
            {
                return -2;
            }

            var userGet = new HttpRequestMessage(HttpMethod.Get, urlUser);
            userGet.Headers.Add("UserId", user.Id.ToString());
            client.DefaultRequestHeaders.Add("Authorization", accessToken);

            var userResponse = await client.SendAsync(userGet);

            if (userResponse.IsSuccessStatusCode)
            {
                LoginUserDetailsResponse userDetails = JsonConvert.DeserializeObject<LoginUserDetailsResponse>(userResponse.Content.ReadAsStringAsync().Result);

                if (userDetails.Success)
                {
                    user.Phone = userDetails.Result.DriverInfo.PhoneNumber;
                    user.LicenceNumber = userDetails.Result.DriverInfo.LicenceNumber;
                    user.Address1 = userDetails.Result.DriverInfo.Address1;
                    user.Address2 = userDetails.Result.DriverInfo.Address2;
                    user.Address3 = userDetails.Result.DriverInfo.Address3;
                    user.PostCode = userDetails.Result.DriverInfo.PostCode;
                    user.City = userDetails.Result.DriverInfo.City;
                    user.Country = userDetails.Result.DriverInfo.Country;
                    db.UpdateUser(user);

                    foreach (LicenceResponseModel licenceItem in userDetails.Result.ListOfLicences)
                    {
                        LicenceTable licence = new LicenceTable();
                        licence.DriverId = user.DriverId;
                        licence.LicenceNumber = user.LicenceNumber;
                        licence.LicenceVersion = int.Parse(licenceItem.Class);
                        licence.Endorsements = licenceItem.Endorsement;

                        db.InsertUserLicence(licence);
                    }

                    var companyGet = new HttpRequestMessage(HttpMethod.Get, urlCompany);
                    companyGet.Headers.Add("DriverId", user.DriverId.ToString());

                    var companyResponse = await client.SendAsync(companyGet);

                    if (companyResponse.IsSuccessStatusCode)
                    {
                        LoginCompanyResponse companyDetails = JsonConvert.DeserializeObject<LoginCompanyResponse>(companyResponse.Content.ReadAsStringAsync().Result);

                        if (companyDetails.Success)
                        {
                            foreach (CompanyTable companyItem in companyDetails.Companies)
                            {
                                companyItem.DriverId = user.DriverId;
                                db.InsertUserComapany(companyItem);
                            }
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    else
                    {
                        return 2;
                    }

                    var vehicleGet = new HttpRequestMessage(HttpMethod.Get, urlVehicle);
                    vehicleGet.Headers.Add("DriverId", user.DriverId.ToString());

                    var vehicleResponse = await client.SendAsync(vehicleGet);

                    if (vehicleResponse.IsSuccessStatusCode)
                    {
                        LoginVehicleResponse vehicleDetails = JsonConvert.DeserializeObject<LoginVehicleResponse>(vehicleResponse.Content.ReadAsStringAsync().Result);

                        if (vehicleDetails.Success)
                        {
                            foreach (VehicleResponseModel vehicleItem in vehicleDetails.Vehicles)
                            {
                                VehicleTable vehicle = new VehicleTable();
                                vehicle.Registration = vehicleItem.Rego;
                                vehicle.MakeModel = vehicleItem.MakeModel;
                                vehicle.FleetNumber = vehicleItem.FleetNumber;
                                vehicle.CompanyId = vehicleItem.CompanyId;
                                vehicle.ServerKey = vehicleItem.Id;
                                db.InsertUserVehicles(vehicle);
                            }
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        return 1;
                    }

                    return 3;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }

        internal async Task<OcrResults> GetRegoText(MediaFile photo)
        {
            OcrResults rego;
            var ocrClient = new VisionServiceClient("a2642181157b4664a1f6defc36dfabeb");

            using (var photoStream = photo.GetStream())
            {
                try
                {
                    rego = await ocrClient.RecognizeTextAsync(photoStream);
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, e.Message.ToString(), Resource.DisplayAlertOkay);
                    return null;
                }

                return rego;
            }
        }

        internal async Task<bool> QueryUpdateProfile(UserTable user)
        {
            string url = GetBaseUrl() + Constants.REST_URL_ADDVEHICLE;
            string contentType = Constants.CONTENT_TYPE;

            string json = JsonConvert.SerializeObject(user);
            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                // db.UpdateUserInfo();
                return true;
            }
            else
            {
                // db.UserOffline();
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
            vehicleModel.fleetNumber = vehicle.FleetNumber;
            vehicleModel.companyId = vehicle.CompanyId;

            string json = JsonConvert.SerializeObject(vehicleModel);
            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                QueryShiftResponse result = JsonConvert.DeserializeObject<QueryShiftResponse>(response.Content.ReadAsStringAsync().Result);

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

        internal async Task<int> QueryShift(ShiftTable shift, bool shiftStarted, int userId = 0, int companyId = 0)
        {
            string contentType = Constants.CONTENT_TYPE;
            string url;

            string json;
            client.DefaultRequestHeaders.Add("Authorization", accessToken);

            if (shiftStarted)
            {
                url = GetBaseUrl() + Constants.REST_URL_ADDSHIFTEND;
                EndShiftModel shiftModel = new EndShiftModel();

                shiftModel.id = shift.ServerKey;
                shiftModel.endDate = shift.EndDate;
                shiftModel.endLocationLat = shift.EndLat;
                shiftModel.endLocationLong = shift.EndLong;
                shiftModel.endLocation = shift.EndLocation;

                json = JsonConvert.SerializeObject(shiftModel);
            }
            else
            {
                url = GetBaseUrl() + Constants.REST_URL_ADDSHIFTSTART;

                StartShiftModel shiftModel = new StartShiftModel();

                shiftModel.driverId = userId;
                shiftModel.companyId = companyId;
                shiftModel.startDate = shift.StartDate;
                shiftModel.startLocationLat = shift.StartLat;
                shiftModel.startLocationLong = shift.StartLong;
                shiftModel.startLocation = shift.StartLocation;

                json = JsonConvert.SerializeObject(shiftModel);
            }

            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                QueryShiftResponse result = JsonConvert.DeserializeObject<QueryShiftResponse>(response.Content.ReadAsStringAsync().Result);
                if (result.Success)
                {
                    if (!shiftStarted)
                    {
                        if (result.Result > 0)
                        {
                            return result.Result;
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to register shift, please try again", Resource.DisplayAlertOkay);
                            return -2;
                        }
                    }
                    else
                    {
                        return 0;
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

        internal async Task<int> QueryDrive(bool driveStarted, DriveTable drive, int serverShift = -1)
        {
            string url;
            string contentType = Constants.CONTENT_TYPE;
            string json;
            client.DefaultRequestHeaders.Add("Authorization", accessToken);

            if (!driveStarted)
            {
                url = GetBaseUrl() + Constants.REST_URL_ADDDRIVESTART;

                DriveStartModel driveModel = new DriveStartModel();
                driveModel.shiftId = serverShift;
                driveModel.startDrivingDateTime = drive.StartDate;
                driveModel.vehicleId = drive.VehicleKey;
                driveModel.startHubo = drive.StartHubo;

                json = JsonConvert.SerializeObject(driveModel);
            }
            else
            {
                url = GetBaseUrl() + Constants.REST_URL_ADDDRIVEEND;

                DriveEndModel driveModel = new DriveEndModel();
                driveModel.id = drive.ServerId;
                driveModel.stopDrivingDateTime = drive.EndDate;
                driveModel.stopHubo = drive.EndHubo;

                json = JsonConvert.SerializeObject(driveModel);
            }

            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                QueryDriveResponse result = JsonConvert.DeserializeObject<QueryDriveResponse>(response.Content.ReadAsStringAsync().Result);

                if (result.Success)
                {
                    if (!driveStarted)
                    {
                        if (result.Result > 0)
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
                        return 0;
                    }
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

        internal async Task<int> QueryBreak(bool breakStarted, BreakTable breakTable)
        {
            client.DefaultRequestHeaders.Add("Authorization", accessToken);
            string url;
            string contentType = Constants.CONTENT_TYPE;
            string json;

            if (!breakStarted)
            {
                url = GetBaseUrl() + Constants.REST_URL_ADDBREAKSTART;

                BreakStartModel breakModel = new BreakStartModel();
                breakModel.shiftId = breakTable.ShiftKey;
                breakModel.startBreakDateTime = breakTable.StartDate;
                breakModel.startBreakLocation = breakTable.StartLocation;

                json = JsonConvert.SerializeObject(breakModel);
            }
            else
            {
                url = GetBaseUrl() + Constants.REST_URL_ADDBREAKEND;

                BreakEndModel breakModel = new BreakEndModel();
                breakModel.id = breakTable.ServerId;
                breakModel.stopBreakDateTime = breakTable.EndDate;
                breakModel.stopBreakLocation = breakTable.EndLocation;

                json = JsonConvert.SerializeObject(breakModel);
            }

            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                QueryBreakResponse result = JsonConvert.DeserializeObject<QueryBreakResponse>(response.Content.ReadAsStringAsync().Result);

                if (result.Success)
                {
                    if (!breakStarted)
                    {
                        if (result.Result > 0)
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
                        return 0;
                    }
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

        internal async Task<int> InsertGeoData(List<GeolocationTable> geolocation)
        {
            string url = GetBaseUrl() + Constants.REST_URL_INSERTGEODATA;
            string contentType = Constants.CONTENT_TYPE;

            List<InsertGeoModel> modelList = new List<InsertGeoModel>();

            foreach (GeolocationTable item in geolocation)
            {
                InsertGeoModel geoModel = new InsertGeoModel();
                geoModel.drivingShiftId = item.DriveKey;
                geoModel.timeStamp = item.TimeStamp;
                geoModel.latitude = item.Longitude;
                geoModel.longitude = item.Longitude;

                modelList.Add(geoModel);
            }

            string json = JsonConvert.SerializeObject(modelList);
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
            else
            {
                return -1;
            }
        }

        internal async Task<int> InsertNote(NoteTable note)
        {
            string url = GetBaseUrl() + Constants.REST_URL_INSERTNOTE;
            string contentType = Constants.CONTENT_TYPE;

            InsertNoteModel noteModel = new InsertNoteModel();
            noteModel.shiftId = note.ShiftKey;
            noteModel.noteText = note.Note;
            noteModel.timeStamp = note.Date;

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

        internal async Task<int> ExportData(string emailAddress, string emailBody)
        {
            // TODO: make sure offline data is synced first
            string url = GetBaseUrl() + Constants.REST_URL_EXPORTDATA;
            string contentType = Constants.CONTENT_TYPE;

            ExportModel export = new ExportModel();
            export.email = emailAddress;
            export.body = emailBody;

            string json = JsonConvert.SerializeObject(export);
            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                ExportResponse result = JsonConvert.DeserializeObject<ExportResponse>(response.Content.ReadAsStringAsync().Result);

                if (result.Success)
                {
                    return result.Result;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -2;
            }
        }

        internal async Task<int> RegisterUser(UserTable newUser, string password)
        {
            string url = GetBaseUrl() + Constants.REST_URL_REGISTERUSER;
            string contentType = Constants.CONTENT_TYPE;

            RegisterModel register = new RegisterModel();
            register.firstName = newUser.FirstName;
            register.lastName = newUser.LastName;
            register.email = newUser.Email;
            register.password = password;

            string json = JsonConvert.SerializeObject(register);
            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                RegisterResponse result = JsonConvert.DeserializeObject<RegisterResponse>(response.Content.ReadAsStringAsync().Result);

                if (result.Success)
                {
                }
            }

            return -1;
        }

        private string GetBaseUrl()
        {
            return "http://test.triotech.co.nz/huboportal/api";
        }
    }
}