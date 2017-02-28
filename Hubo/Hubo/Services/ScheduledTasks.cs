using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    public static class ScheduledTasks
    {
        public static Boolean testTask()
        {
            return true;
        }

        public static async void CheckOfflineData()
        {
            DatabaseService db = new DatabaseService();

            bool isConnected = CrossConnectivity.Current.IsConnected;

            if (isConnected)
                await db.ReturnOffline();
        }
    }
}
