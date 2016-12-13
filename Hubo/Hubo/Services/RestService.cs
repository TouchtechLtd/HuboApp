using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Net;

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

        internal Task<bool> Login(string username, string password)
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

            //using (var client = new HttpClient(new NativeMessageHandler()))
            //{
            ////    var content = new FormUrlEncodedContent(new[]
            ////    {
            ////    new KeyValuePair<string,string>("usernameOrEmailaddress", username),
            ////    new KeyValuePair<string,string>("password", password)
            ////});




            //    var huboApiTokenUrl = GetBaseUrl() + Constants.REST_URL_LOGIN;
            //    HttpResponseMessage authenticateResponse;
            //    authenticateResponse = await client.PostAsJsonAsync(new Uri(huboApiTokenUrl), content);
            //    if (authenticateResponse.IsSuccessStatusCode)
            //    {
            //        return true;
            //    }
            //    return false;
            //}

            LoginModel login = new LoginModel();
            login.usernameOrEmailAddress = username;
            login.password = password;
            string json = JsonConvert.SerializeObject(login);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetBaseUrl() + Constants.REST_URL_LOGIN);
            request.ContentType = "application/json";
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }
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