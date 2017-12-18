// <copyright file="ScheduledTasks.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using Plugin.Connectivity;

    public static class ScheduledTasks
    {
        public static bool TestTask()
        {
            return true;
        }

        public static async void CheckOfflineData()
        {
            DatabaseService db = new DatabaseService();

            bool isConnected = CrossConnectivity.Current.IsConnected;

            if (isConnected)
            {
                await db.ReturnOffline();
            }
        }
    }
}
