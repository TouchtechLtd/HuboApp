using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using ModernHttpClient;
using System.Net.Http;
using System.Text;

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
            //TODO: Code to communicate with server to login
            UserTable user = new UserTable();
            //user.User = username;
            //user.FirstName = "Ben";
            //user.LastName = "Suarez-Brodie";
            //user.Email = "ben@triotech.co.nz";
            //user.License = "DJ89473KL";
            //user.LicenseVersion = "158";
            //user.Endorsements = "F";
            //user.CompanyName = "Trio Technology";
            //user.Address = "41 The Square, Palmerston North";
            //user.CompanyEmail = "nick@triotech.co.nz";
            //user.Phone = "0278851100";
            username = "Developer";
            password = "D3v@triotech";

            string url = GetBaseUrl() + Constants.REST_URL_LOGIN;
            string contentType = "application/json"; // or application/xml

            LoginModel login = new LoginModel();
            login.usernameOrEmailAddress = username;
            login.password = password;
            string json = JsonConvert.SerializeObject(login);

            HttpClient client = new HttpClient();
            var taskPostClient = client.PostAsync(url, new StringContent(json, Encoding.UTF8, contentType));

            await taskPostClient.ContinueWith((HttpResponseMessage) =>
             {
                 return true;
             });

            return false;

          


           
        }

        private string GetBaseUrl()
        {
            return "http://192.168.1.112:6634/api";
        }

        internal void QueryUpdateUserInfo(UserTable user)
        {
            //TODO: Code to communicate with server to update user info
            db.UpdateUserInfo(user);
        }
        internal void QueryAddVehicle()
        {
            //JSON Structure

            //{
            //    "vehicle": {
            //        "LicenceNo": "abc123",
            //      "CompanyId": 1,
            //      "Make": "Toyota",
            //      "Model": "Giga",
            //      "StartingOdometer": 123000
            //    }
            //}

            //json for sending a vehicle
        }

    }
}