// <copyright file="CloseApp.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using Android.App;
using Android.Content;
using Android.OS;
using Hubo.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloseApp))]

namespace Hubo.Droid
{
    public class CloseApp : ICloseApplication
    {
        public const string KEY_RESTART_APP = "triotech.hubo.droid.RESTART";

        public void CloseApplication()
        {
            var activity = (Activity)Forms.Context;

            Intent restartIntent = new Intent(KEY_RESTART_APP);
            PendingIntent restartPending = PendingIntent.GetBroadcast(Forms.Context, 0, restartIntent, PendingIntentFlags.OneShot);

            AlarmManager alarmManager = (AlarmManager)Forms.Context.GetSystemService(Context.AlarmService);

            alarmManager.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + (2 * 1000), restartPending);

            activity.FinishAffinity();
        }
    }
}