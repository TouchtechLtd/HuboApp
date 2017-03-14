// <copyright file="NotifyService.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using Hubo.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(NotifyService))]

namespace Hubo.Droid
{
    using Android.App;
    using Android.Content;
    using System;

    public class NotifyService : INotifyService
    {
        private const int Id = 5;
        private NotificationManager notifyManager = Forms.Context.GetSystemService(Context.NotificationService) as NotificationManager;
        private Notification.Builder builder = new Notification.Builder(Forms.Context);

        private static string KEY_SHIFT_END = "key_shift_end";
        private static string KEY_BREAK_END = "key_break_end";

        private TaskStackBuilder stackBuilder = TaskStackBuilder.Create(Forms.Context);

        public NotifyService()
        {
        }

        public void PresentNotification(string title, string text, bool endCounter, bool endButton)
        {
            this.builder.SetVisibility(NotificationVisibility.Public);
            this.builder.SetContentTitle(title);
            this.builder.SetContentText(text);
            this.builder.SetSmallIcon(Resource.Drawable.icon);
            this.builder.SetCategory(Notification.CategoryEvent);
            this.builder.SetOngoing(true);
            this.builder.SetAutoCancel(false);
            this.builder.SetShowWhen(false);

            if (endCounter)
            {
                this.builder.SetPriority((int)NotificationPriority.High);
                this.builder.SetDefaults(NotificationDefaults.All);
            }
            else
            {
                this.builder.SetPriority((int)NotificationPriority.Min);
                this.builder.SetDefaults(NotificationDefaults.Vibrate);
            }

            Intent resultIntent = new Intent(Forms.Context, typeof(MainActivity));
            resultIntent.AddFlags(ActivityFlags.SingleTop);

            PendingIntent pending = PendingIntent.GetActivity(Forms.Context, 0, resultIntent, PendingIntentFlags.OneShot);

            if (endButton)
            {
                if (title.Contains("Shift"))
                {
                    RemoteInput remote = new RemoteInput.Builder(KEY_SHIFT_END).SetLabel("End Shift").Build();
                    Notification.Action action = new Notification.Action.Builder(Resource.Drawable.icon, "End Shift", pending).AddRemoteInput(remote).Build();

                    this.builder.AddAction(action);
                }
                else if (title.Contains("Break"))
                {
                    RemoteInput remote = new RemoteInput.Builder(KEY_BREAK_END).SetLabel("End Break").Build();
                    Notification.Action action = new Notification.Action.Builder(Resource.Drawable.icon, "End Break", pending).AddRemoteInput(remote).Build();

                    this.builder.AddAction(action);
                }
            }

            //this.builder.SetContentIntent(pending);

            Notification notification = this.builder.Build();
            this.notifyManager.Notify(Id, notification);
        }

        public void UpdateNotification(string title, string text, bool endCounter, bool endButton)
        {
            this.builder.SetContentTitle(title);
            this.builder.SetContentText(text);

            if (endCounter)
            {
                this.builder.SetPriority((int)NotificationPriority.High);
                this.builder.SetDefaults(NotificationDefaults.All);
            }
            else
            {
                this.builder.SetPriority((int)NotificationPriority.Min);
                this.builder.SetDefaults(NotificationDefaults.Vibrate);
            }

            Intent resultIntent = new Intent(Forms.Context, typeof(MainActivity));
            resultIntent.AddFlags(ActivityFlags.SingleTop);

            PendingIntent pending = PendingIntent.GetActivity(Forms.Context, 0, resultIntent, PendingIntentFlags.UpdateCurrent);

            this.builder.SetContentIntent(pending);

            Notification notification = this.builder.Build();
            this.notifyManager.Notify(Id, notification);
        }
    }
}