// <copyright file="RestService.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Acr.UserDialogs;
    using Microsoft.ProjectOxford.Vision;
    using Microsoft.ProjectOxford.Vision.Contract;
    using Newtonsoft.Json;
    using Plugin.Media.Abstractions;

    internal class RestService
    {
        private readonly DatabaseService db;
        private string accessToken;

        public RestService()
        {
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

            LoginRequestModel loginModel = new LoginRequestModel()
            {
                // loginModel.usernameOrEmailAddress = username;
                // loginModel.password = password;
                UsernameOrEmailAddress = "ben@triotech.co.nz",
                Password = "tazmania"
            };
            string json = JsonConvert.SerializeObject(loginModel);

            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            HttpResponseMessage response;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    response = await client.PostAsync(url, content);
                }
                catch
                {
                    return false;
                }
            }

            if (response.IsSuccessStatusCode)
            {
                LoginUserResponse result = JsonConvert.DeserializeObject<LoginUserResponse>(response.Content.ReadAsStringAsync().Result);

                if (result.Success)
                {
                    UserTable user = new UserTable()
                    {
                        Id = result.Result.Id,
                        UserName = username,
                        Token = result.Result.Token,
                        DriverId = result.Result.DriverId,
                        Email = result.Result.EmailAddress,
                        FirstName = result.Result.FirstName,
                        LastName = result.Result.Surname
                    };
                    db.InsertUser(user);

                    accessToken = "Bearer " + db.GetUserToken();

                    return true;
                }
                else
                {
                    await UserDialogs.Instance.AlertAsync(Resource.InvalidLogin, Resource.Alert, Resource.Okay);
                    return false;
                }
            }
            else
            {
                await UserDialogs.Instance.AlertAsync(Resource.ConnectionError, Resource.Alert, Resource.Okay);
                return false;
            }
        }

        internal async Task<string> GetLocation(Geolocation geoCoordinates)
        {
            if (geoCoordinates != null)
            {
                var googleGet = new HttpRequestMessage(HttpMethod.Get, Constants.REST_URL_GOOGLEAPI + "latlng=" + geoCoordinates.Latitude + "," + geoCoordinates.Longitude + "&key=" + Configuration.GoogleMapsApiKey);

                HttpResponseMessage googleResponse;

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        googleResponse = await client.SendAsync(googleGet);
                    }
                    catch
                    {
                        // Is Offline, so return empty
                        return string.Empty;
                    }
                }

                if (googleResponse.IsSuccessStatusCode)
                {
                    GoogleGeoResponse geoResponse = JsonConvert.DeserializeObject<GoogleGeoResponse>(googleResponse.Content.ReadAsStringAsync().Result);

                    if (geoResponse.Results[0] != null)
                    {
                        List<GoogleAddressComponents> addressComponents = geoResponse.Results[0].GoogleAddressComponents;

                        // Number + Street Name + City
                        return addressComponents[0].ShortName + " " + addressComponents[1].ShortName + ", " + addressComponents[2].ShortName;
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
            HttpResponseMessage shiftResponse;
            HttpResponseMessage noteResponse;
            HttpResponseMessage driveResponse;
            HttpResponseMessage breakResponse;

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

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", accessToken);
                try
                {
                    shiftResponse = await client.SendAsync(shiftGet);
                }
                catch
                {
                    return -1;
                }
            }

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
                        shift.StartLocation = shiftItem.StartLocation;
                        shift.EndLocation = shiftItem.EndLocation;

                        ShiftTable shiftId = new ShiftTable();
                        shiftId = db.InsertUserShifts(shift);

                        if (shiftId == null)
                        {
                            return -4;
                        }

                        var noteGet = new HttpRequestMessage(HttpMethod.Get, urlNote);
                        noteGet.Headers.Add("ShiftId", shiftId.ServerKey.ToString());

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", accessToken);
                            try
                            {
                                noteResponse = await client.SendAsync(noteGet);
                            }
                            catch
                            {
                                return -1;
                            }
                        }

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

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", accessToken);
                            try
                            {
                               driveResponse = await client.SendAsync(driveGet);
                            }
                            catch
                            {
                                return -1;
                            }
                        }

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
                                    drive.ServerVehicleKey = driveItem.VehicleId;
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

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", accessToken);
                            try
                            {
                                breakResponse = await client.SendAsync(breakGet);
                            }
                            catch
                            {
                                return -1;
                            }
                        }

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

            HttpResponseMessage userResponse;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", accessToken);
                try
                {
                    userResponse = await client.SendAsync(userGet);
                }
                catch
                {
                    return -1;
                }
            }

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
                        LicenceTable licence = new LicenceTable()
                        {
                            DriverId = user.DriverId,
                            LicenceNumber = user.LicenceNumber,
                            LicenceVersion = int.Parse(licenceItem.Class),
                            Endorsements = licenceItem.Endorsement
                        };
                        db.InsertUserLicence(licence);
                    }

                    var companyGet = new HttpRequestMessage(HttpMethod.Get, urlCompany);
                    companyGet.Headers.Add("DriverId", user.DriverId.ToString());

                    HttpResponseMessage companyResponse;

                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", accessToken);
                        try
                        {
                           companyResponse = await client.SendAsync(companyGet);
                        }
                        catch
                        {
                            return -1;
                        }
                    }

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

                    HttpResponseMessage vehicleResponse;

                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", accessToken);
                        try
                        {
                            vehicleResponse = await client.SendAsync(vehicleGet);
                        }
                        catch
                        {
                            return -1;
                        }
                    }

                    if (vehicleResponse.IsSuccessStatusCode)
                    {
                        LoginVehicleResponse vehicleDetails = JsonConvert.DeserializeObject<LoginVehicleResponse>(vehicleResponse.Content.ReadAsStringAsync().Result);

                        if (vehicleDetails.Success)
                        {
                            foreach (VehicleResponseModel vehicleItem in vehicleDetails.Vehicles)
                            {
                                VehicleTable vehicle = new VehicleTable()
                                {
                                    Registration = vehicleItem.Rego,
                                    MakeModel = vehicleItem.MakeModel,
                                    FleetNumber = vehicleItem.FleetNumber,
                                    CompanyId = vehicleItem.CompanyId,
                                    ServerKey = vehicleItem.Id
                                };
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
            var ocrClient = new VisionServiceClient(Resource.VisionKey);
            using (var photoStream = photo.GetStream())
            {
                try
                {
                    rego = await ocrClient.RecognizeTextAsync(photoStream);
                }
                catch (Exception e)
                {
                    await UserDialogs.Instance.AlertAsync(e.Message.ToString(), Resource.Alert, Resource.Okay);
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

            HttpResponseMessage response;

            using (HttpClient client = new HttpClient())
            {
                response = await client.PostAsync(url, content);
            }

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

            VehicleRequestModel vehicleModel = new VehicleRequestModel()
            {
                registrationNo = vehicle.Registration,
                makeModel = vehicle.MakeModel,
                fleetNumber = vehicle.FleetNumber,
                companyId = vehicle.CompanyId
            };
            string json = JsonConvert.SerializeObject(vehicleModel);
            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            HttpResponseMessage response;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    response = await client.PostAsync(url, content);
                }
                catch
                {
                    return false;
                }
            }

            if (response.IsSuccessStatusCode)
            {
                QueryShiftResponse result = JsonConvert.DeserializeObject<QueryShiftResponse>(response.Content.ReadAsStringAsync().Result);

                if (result.Success)
                {
                    return true;
                }
                else
                {
                    await UserDialogs.Instance.AlertAsync(Resource.RegisterVehicleError, Resource.Alert, Resource.Okay);
                    return false;
                }
            }
            else
            {
                await UserDialogs.Instance.AlertAsync(Resource.ConnectionError, Resource.Alert, Resource.Okay);
                return false;
            }
        }

        internal async Task<bool> QueryRegoPhoto(MediaFile photoStream)
        {
            string contentType = "application/jpeg";
            string url = GetBaseUrl() + Constants.REST_URL_REGOPHOTO;

            var stream = photoStream.GetStream();
            var bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes, 0, (int)stream.Length);
            string base64 = System.Convert.ToBase64String(bytes);

            string json = JsonConvert.SerializeObject(base64);
            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            HttpResponseMessage response;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", accessToken);
                try
                {
                    response = await client.PostAsync(url, content);
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        internal async Task<int> QueryShift(ShiftTable shift, bool shiftStarted, int userId = 0, int companyId = 0)
        {
            string contentType = Constants.CONTENT_TYPE;
            string url;

            string json;

            if (shiftStarted)
            {
                url = GetBaseUrl() + Constants.REST_URL_ADDSHIFTEND;
                EndShiftModel shiftModel = new EndShiftModel()
                {
                    id = shift.ServerKey,
                    endDate = shift.EndDate,
                    endLocationLat = shift.EndLat,
                    endLocationLong = shift.EndLong,
                    endLocation = shift.EndLocation,
                    endNote = shift.EndNote
                };
                json = JsonConvert.SerializeObject(shiftModel);
            }
            else
            {
                url = GetBaseUrl() + Constants.REST_URL_ADDSHIFTSTART;

                StartShiftModel shiftModel = new StartShiftModel()
                {
                    driverId = userId,
                    companyId = companyId,
                    startDate = shift.StartDate,
                    startLocationLat = shift.StartLat,
                    startLocationLong = shift.StartLong,
                    startLocation = shift.StartLocation,
                    startNote = shift.StartNote
                };
                json = JsonConvert.SerializeObject(shiftModel);
            }

            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            HttpResponseMessage response;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", accessToken);
                try
                {
                    response = await client.PostAsync(url, content);
                }
                catch
                {
                    return -2;
                }
            }

            if (response.IsSuccessStatusCode)
            {
                QueryShiftResponse result = JsonConvert.DeserializeObject<QueryShiftResponse>(response.Content.ReadAsStringAsync().Result);
                if (result.Success)
                {
                    if (result.Result > 0)
                    {
                        return result.Result;
                    }

                    return 0;
                }
                else
                {
                    await UserDialogs.Instance.AlertAsync(Resource.RegisterShiftError, Resource.Alert, Resource.Okay);
                    return -2;
                }
            }
            else
            {
                await UserDialogs.Instance.AlertAsync(Resource.ConnectionError, Resource.Alert, Resource.Okay);
                return -2;
            }
        }

        internal async Task<int> QueryDrive(bool driveStarted, DriveTable drive)
        {
            string url;
            string contentType = Constants.CONTENT_TYPE;
            string json;
            HttpResponseMessage response;
            if (!driveStarted)
            {
                url = GetBaseUrl() + Constants.REST_URL_ADDDRIVESTART;

                DriveStartModel driveModel = new DriveStartModel()
                {
                    shiftId = drive.ServerShiftKey,
                    startDrivingDateTime = drive.StartDate,
                    vehicleId = drive.ServerVehicleKey,
                    startHubo = drive.StartHubo,
                    startNote = drive.StartNote
                };
                json = JsonConvert.SerializeObject(driveModel);
            }
            else
            {
                url = GetBaseUrl() + Constants.REST_URL_ADDDRIVEEND;

                DriveEndModel driveModel = new DriveEndModel()
                {
                    id = drive.ServerId,
                    stopDrivingDateTime = drive.EndDate,
                    stopHubo = drive.EndHubo,
                    endNote = drive.EndNote
                };
                json = JsonConvert.SerializeObject(driveModel);
            }

            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", accessToken);
                try
                {
                    response = await client.PostAsync(url, content);
                }
                catch
                {
                    return -1;
                }
            }

            if (response.IsSuccessStatusCode)
            {
                QueryDriveResponse result = JsonConvert.DeserializeObject<QueryDriveResponse>(response.Content.ReadAsStringAsync().Result);

                if (result.Success)
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
            string url;
            string contentType = Constants.CONTENT_TYPE;
            string json;
            HttpResponseMessage response;

            if (!breakStarted)
            {
                url = GetBaseUrl() + Constants.REST_URL_ADDBREAKSTART;

                BreakStartModel breakModel = new BreakStartModel()
                {
                    shiftId = breakTable.ServerShiftKey,
                    startBreakDateTime = breakTable.StartDate,
                    startBreakLocation = breakTable.StartLocation,
                    startNote = breakTable.StartNote
                };
                json = JsonConvert.SerializeObject(breakModel);
            }
            else
            {
                url = GetBaseUrl() + Constants.REST_URL_ADDBREAKEND;

                BreakEndModel breakModel = new BreakEndModel()
                {
                    id = breakTable.ServerId,
                    stopBreakDateTime = breakTable.EndDate,
                    stopBreakLocation = breakTable.EndLocation,
                    endNote = breakTable.EndNote
                };
                json = JsonConvert.SerializeObject(breakModel);
            }

            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", accessToken);
                try
                {
                    response = await client.PostAsync(url, content);
                }
                catch
                {
                    return -1;
                }
            }

            if (response.IsSuccessStatusCode)
            {
                QueryBreakResponse result = JsonConvert.DeserializeObject<QueryBreakResponse>(response.Content.ReadAsStringAsync().Result);

                if (result.Success)
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
            HttpResponseMessage response;

            List<InsertGeoModel> modelList = new List<InsertGeoModel>();

            foreach (GeolocationTable item in geolocation)
            {
                InsertGeoModel geoModel = new InsertGeoModel()
                {
                    drivingShiftId = item.DriveKey,
                    timeStamp = item.TimeStamp,
                    latitude = item.Longitude,
                    longitude = item.Longitude
                };
                modelList.Add(geoModel);
            }

            string json = JsonConvert.SerializeObject(modelList);
            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", accessToken);
                try
                {
                    response = await client.PostAsync(url, content);
                }
                catch
                {
                    return -1;
                }
            }

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

        internal async Task<int> InsertVehicle(VehicleTable vehicleToInsert)
        {
            string url = GetBaseUrl() + Constants.REST_URL_ADDVEHICLE;
            string contentType = Constants.CONTENT_TYPE;
            HttpResponseMessage response;

            VehicleInsertModel vehicleInsertModel = new VehicleInsertModel()
            {
                RegistrationNo = vehicleToInsert.Registration,
                IsManuallyEntered = true
            };

            string json = JsonConvert.SerializeObject(vehicleInsertModel);
            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", accessToken);
                try
                {
                    response = await client.PostAsync(url, content);
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }

            if (response.IsSuccessStatusCode)
            {
                InsertVehicleResponse result = JsonConvert.DeserializeObject<InsertVehicleResponse>(response.Content.ReadAsStringAsync().Result);
                if (result.Success)
                {
                    return result.Result;
                }

                return -1;
            }

            return -1;
        }

        internal async Task<int> InsertNote(NoteTable note)
        {
            string url = GetBaseUrl() + Constants.REST_URL_INSERTNOTE;
            string contentType = Constants.CONTENT_TYPE;
            HttpResponseMessage response;

            InsertNoteModel noteModel = new InsertNoteModel()
            {
                shiftId = note.ShiftKey,
                noteText = note.Note,
                timeStamp = note.Date
            };
            string json = JsonConvert.SerializeObject(noteModel);
            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            using (HttpClient client = new HttpClient())
            {
                response = await client.PostAsync(url, content);
            }

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
            HttpResponseMessage response;

            ExportModel export = new ExportModel()
            {
                email = emailAddress,
                body = emailBody
            };
            string json = JsonConvert.SerializeObject(export);
            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            using (HttpClient client = new HttpClient())
            {
                response = await client.PostAsync(url, content);
            }

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
            HttpResponseMessage response;

            RegisterModel register = new RegisterModel()
            {
                firstName = newUser.FirstName,
                lastName = newUser.LastName,
                email = newUser.Email,
                password = password
            };
            string json = JsonConvert.SerializeObject(register);
            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            using (HttpClient client = new HttpClient())
            {
                response = await client.PostAsync(url, content);
            }

            if (response.IsSuccessStatusCode)
            {
                RegisterResponse result = JsonConvert.DeserializeObject<RegisterResponse>(response.Content.ReadAsStringAsync().Result);

                if (result.Success)
                {
                }
            }

            return -1;
        }

        internal async Task<Geolocation> GetLatAndLong()
        {
            Application.Locator.DesiredAccuracy = 100;

            Geolocation results = new Geolocation();

            try
            {
                var position = await Application.Locator.GetPositionAsync(timeoutMilliseconds: 4000);

                results.Longitude = position.Longitude;
                results.Latitude = position.Latitude;
                return results;
            }
            catch
            {
                return results;
            }
        }

        private string GetBaseUrl()
        {
            return Resource.SiteURL;
        }
    }
}