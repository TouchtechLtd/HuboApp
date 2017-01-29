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
            using (var client = new HttpClient())
            {
                string url = GetBaseUrl() + Constants.REST_URL_LOGIN;
                string contentType = Constants.CONTENT_TYPE;

                dynamic loginModel = new ExpandoObject();
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
                        //if (await GetUser(loginModel.usernameOrEmailAddress))
                        //    return true;
                        //else
                        //{
                        //    await Application.Current.MainPage.DisplayAlert(Resource.DisplayAlertTitle, "Unable to get user details", Resource.DisplayAlertOkay);
                        //    return false;
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

        private async Task<bool> GetUser(string userName)
        {
            string url = GetBaseUrl() + Constants.REST_URL_GETUSERDETAILS;
            string contentType = Constants.CONTENT_TYPE;

            LoginResponse login = new LoginResponse();

            if (userName == null)
                return false;

            dynamic userModel = new ExpandoObject();
            userModel.userName = userName;

            string json = JsonConvert.SerializeObject(userModel);

            HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                LoginResponse userDetails = new LoginResponse();
                userDetails = JsonConvert.DeserializeObject<LoginResponse>(response.Content.ReadAsStringAsync().Result);

                UserTable user = new UserTable();
                CompanyTable userCompany = new CompanyTable();
                VehicleTable userVehicles = new VehicleTable();

                user.Id = Int32.Parse(userDetails.UserId.ToString());
                user.FirstName = userDetails.DriverFirstName;
                user.LastName = userDetails.DriverSurname;
                user.License = userDetails.LicenceNo;
                user.LicenseVersion = userDetails.LicenceVersion.ToString();
                user.Phone = userDetails.MobilePh.ToString();

                foreach (CompanyAndVehicles companyItem in userDetails.CompaniesAndVehicle)
                {
                    companyItem.Company.DriverId = user.Id;
                    db.InsertUserComapany(companyItem.Company);

                    foreach (VehicleTable vehicleItem in companyItem.Vehicles)
                    {
                        db.InsertUserVehicles(vehicleItem);
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private string GetBaseUrl()
        {
            return "http://test.triotech.co.nz/huboportal/api";
        }

        internal void QueryUpdateUserInfo(UserTable user)
        {
            //TODO: Code to communicate with server to update user info
            db.UpdateUserInfo(user);
        }

        internal async Task<bool> QueryAddVehicle(VehicleTable vehicle)
        {
            using (var client = new HttpClient())
            {
                string url = GetBaseUrl() + Constants.REST_URL_ADDVEHICLE;
                string contentType = Constants.CONTENT_TYPE;

                dynamic vehicleModel = new ExpandoObject();
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

                dynamic shiftModel = new ExpandoObject();
                dynamic shiftExpando = new ExpandoObject();
                dynamic noteExpando = new ExpandoObject();

                if (shiftStarted)
                {
                    url = GetBaseUrl() + Constants.REST_URL_ADDSHIFTEND;

                    shiftModel.id = shift.ServerKey;
                    shiftModel.endDateTime = DateTime.Now;
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