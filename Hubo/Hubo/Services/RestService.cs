﻿using System;
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
            using (var client = new HttpClient())
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
                    UserResponse result = new UserResponse();
                    result = JsonConvert.DeserializeObject<UserResponse>(response.Content.ReadAsStringAsync().Result);

                    if (result.Success)
                    {
                        //UserTable user = new UserTable();
                        //user.User = username;
                        //user.Token = result.Result;

                        //int totalDetails = await GetUser("result.Id");
                        //switch (totalDetails)
                        //{
                        //    case -2:
                        //        await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Invalid User Details", Resource.DisplayAlertOkay);
                        //        return false;
                        //    case -1:
                        //        await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to get user, company and vehicle details", Resource.DisplayAlertOkay);
                        //        return false;
                        //    case 1:
                        //        await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to get all vehicle details", Resource.DisplayAlertOkay);
                        //        return false;
                        //    case 2:
                        //        await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to get all company and vehicle details", Resource.DisplayAlertOkay);
                        //        return false;
                        //    case 3:
                        //        return true;
                        //}
                        UserTable newUser = new UserTable();
                        newUser.Address = "dfsdfa";
                        newUser.CompanyId = 1;
                        newUser.DriverId = 1;
                        newUser.Email = "ben@triotech.co.nz";
                        newUser.Endorsements = "3";
                        newUser.FirstName = "Ben";
                        newUser.LastName = "Suarez-Brodie";
                        newUser.Token = result.Result;
                        newUser.Phone = "0278851100";
                        newUser.LicenseVersion = "2";
                        newUser.License = "DJK432";
                        db.InsertUser(newUser);
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
        }

        internal Task<bool> ImportTips()
        {
            throw new NotImplementedException();
        }

        internal Task<bool> ImportLoadText()
        {
            throw new NotImplementedException();
        }

        internal async Task<int> GetUser(int id)
        {
            string urlUser = GetBaseUrl() + Constants.REST_URL_GETUSERDETAILS;
            string urlCompany = GetBaseUrl() + Constants.REST_URL_GETCOMPANYDETAILS;
            string urlVehicle = GetBaseUrl() + Constants.REST_URL_GETVEHICLEDETAILS;
            string contentType = Constants.CONTENT_TYPE;

            if (id < 0)
                return -2;

            UserRequestModel userModel = new UserRequestModel();
            userModel.id = id;

            string userJson = JsonConvert.SerializeObject(userModel);

            HttpContent userContent = new StringContent(userJson, Encoding.UTF8, contentType);

            var userResponse = await client.PostAsync(urlUser, userContent);

            if (userResponse.IsSuccessStatusCode)
            {
                LoginUserResponse userDetails = new LoginUserResponse();
                userDetails = JsonConvert.DeserializeObject<LoginUserResponse>(userResponse.Content.ReadAsStringAsync().Result);

                UserTable user = new UserTable();
                CompanyTable userCompany = new CompanyTable();
                VehicleTable userVehicles = new VehicleTable();

                CompanyDetailModel companyModel = new CompanyDetailModel();

                user.Id = userDetails.DriverId;
                user.FirstName = userDetails.DriverFirstName;
                user.LastName = userDetails.DriverSurname;
                user.License = userDetails.LicenceNo;
                user.LicenseVersion = userDetails.LicenceVersion.ToString();
                user.Phone = userDetails.MobilePh.ToString();
                db.InsertUser(user);

                companyModel.id = user.Id;

                string companyJson = JsonConvert.SerializeObject(companyModel);
                HttpContent companyContent = new StringContent(companyJson, Encoding.UTF8, contentType);
                var companyResponse = await client.PostAsync(urlCompany, companyContent);

                if (companyResponse.IsSuccessStatusCode)
                {
                    LoginCompanyResponse companyDetails = new LoginCompanyResponse();
                    companyDetails = JsonConvert.DeserializeObject<LoginCompanyResponse>(companyResponse.Content.ReadAsStringAsync().Result);

                    VehicleDetailModel vehicleModel = new VehicleDetailModel();

                    foreach (CompanyTable companyItem in companyDetails.Companies)
                    {
                        companyItem.DriverId = companyDetails.DriverId;
                        db.InsertUserComapany(companyItem);

                        vehicleModel.id = companyItem.Key;

                        string vehicleJson = JsonConvert.SerializeObject(vehicleModel);
                        HttpContent vehicleContent = new StringContent(vehicleJson, Encoding.UTF8, contentType);
                        var vehicleResponse = await client.PostAsync(urlVehicle, vehicleContent);

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

                //foreach (CompanyAndVehicles companyItem in userDetails.CompaniesAndVehicle)
                //{
                //    companyItem.Company.DriverId = user.Id;
                //    db.InsertUserComapany(companyItem.Company);

                //    foreach (VehicleTable vehicleItem in companyItem.Vehicles)
                //    {
                //        db.InsertUserVehicles(vehicleItem);
                //    }
                //}

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
            using (var client = new HttpClient())
            {
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
                    return false;
                }
            }
        }

        internal async Task<bool> QueryAddVehicle(VehicleTable vehicle)
        {
            using (var client = new HttpClient())
            {
                string url = GetBaseUrl() + Constants.REST_URL_ADDVEHICLE;
                string contentType = Constants.CONTENT_TYPE;

                VehicleRequestModel vehicleModel = new VehicleRequestModel();
                vehicleModel.registrationNo = vehicle.Registration;
                vehicleModel.make = vehicle.Make;
                vehicleModel.model = vehicle.Model;
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
        }

        internal async Task<int> QueryShift(ShiftTable shift, bool shiftStarted, NoteTable note)
        {
            using (var client = new HttpClient())
            {
                string contentType = Constants.CONTENT_TYPE;
                string url;

                ShiftModel shiftModel = new ShiftModel();
                ShiftExpando shiftExpando = new ShiftExpando();
                NoteExpando noteExpando = new NoteExpando();

                if (shiftStarted)
                {
                    url = GetBaseUrl() + Constants.REST_URL_ADDSHIFTEND;

                    shiftExpando.shiftId = 1;

                    noteExpando.noteText = note.Note;
                    noteExpando.date = note.Date;
                    noteExpando.hubo = note.Hubo;
                    noteExpando.latitude = note.Latitude;
                    noteExpando.longitude = note.Longitude;

                    shiftModel.shift = shiftExpando;
                    shiftModel.note = noteExpando;
                }
                else
                {
                    url = GetBaseUrl() + Constants.REST_URL_ADDSHIFTSTART;

                    shiftExpando.driverId = 1;
                    shiftExpando.vehicleId = 1;

                    noteExpando.noteText = note.Note;
                    noteExpando.date = note.Date;
                    noteExpando.hubo = note.Hubo;
                    noteExpando.latitude = note.Latitude;
                    noteExpando.longitude = note.Longitude;

                    shiftModel.shift = shiftExpando;
                    shiftModel.note = noteExpando;
                }

                string json = JsonConvert.SerializeObject(shiftModel);
                HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    int result;
                    result = JsonConvert.DeserializeObject<int>(response.Content.ReadAsStringAsync().Result);
                    if (result > 0)
                    {
                        return result;
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
        }
    }
}