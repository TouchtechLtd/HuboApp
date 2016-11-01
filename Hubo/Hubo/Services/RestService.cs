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
    }
}