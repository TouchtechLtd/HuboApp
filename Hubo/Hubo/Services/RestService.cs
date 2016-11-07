using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ModernHttpClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;



namespace Hubo
{
    class RestService
    {
        System.Net.Http.HttpClient client;
        DatabaseService db = new DatabaseService();

        public RestService()
        {
            client = new System.Net.Http.HttpClient(new NativeMessageHandler());
            this.db = new DatabaseService();
        }

        internal bool Login(string username, string password)
        {
            //TODO: Code to communicate with server to login
            UserTable user = new UserTable();
            user.User = username;
            if(db.Login(user))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}