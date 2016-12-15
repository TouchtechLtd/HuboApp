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
        DatabaseService db = new DatabaseService();

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
                loginModel.usernameOrEmailAddress = username;
                loginModel.password = password;

                string json = JsonConvert.SerializeObject(loginModel);

                HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    UserResponse result = new UserResponse();
                    result = JsonConvert.DeserializeObject<UserResponse>(response.Content.ReadAsStringAsync().Result);
                    if(result.Success)
                    {
                        UserTable user = new UserTable();
                        user.User = username;
                        user.FirstName = "Ben";
                        user.LastName = "Suarez-Brodie";
                        user.Email = "ben@triotech.co.nz";
                        user.License = "DJ89473KL";
                        user.LicenseVersion = "158";
                        user.Endorsements = "F";
                        user.CompanyName = "Trio Technology";
                        user.Address = "41 The Square, Palmerston North";
                        user.CompanyEmail = "nick@triotech.co.nz";
                        user.Phone = "0278851100";
                        user.Token = result.Result;
                        db.InsertUser(user);
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

        private string GetBaseUrl()
        {
            return "http://test.triotech.co.nz/huboserver/api";
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

                string json = JsonConvert.SerializeObject(vehicle);
                HttpContent content = new StringContent(json, Encoding.UTF8, contentType);

                var response = await client.PostAsync(url, content);

                if(response.IsSuccessStatusCode)
                {
                    UserResponse result = new UserResponse();
                    result = JsonConvert.DeserializeObject<UserResponse>(response.Content.ReadAsStringAsync().Result);
                    if(result.Success)
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

    }
}