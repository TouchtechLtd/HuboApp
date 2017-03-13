// <copyright file="NotifyService.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using Hubo.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(NotifyService))]

namespace Hubo.Droid
{
    using System;
    using System.Threading;
    using Android.App;
    using Android.Content;

    public class NotifyService : INotifyService
    {
        private const int Id = 5;
        private NotificationManager notifyManager = Forms.Context.GetSystemService(Context.NotificationService) as NotificationManager;
        private Notification.Builder builder = new Notification.Builder(Forms.Context);

        private TaskStackBuilder stackBuilder = TaskStackBuilder.Create(Forms.Context);

        public NotifyService()
        {
        }

        public void PresentNotification(string title, string text)
        {
            this.builder.SetVisibility(NotificationVisibility.Public);
            this.builder.SetContentTitle(title);
            this.builder.SetContentText(text);
            this.builder.SetSmallIcon(Resource.Drawable.icon);
            this.builder.SetPriority((int)NotificationPriority.Min);
            this.builder.SetDefaults(NotificationDefaults.Vibrate);
            this.builder.SetCategory(Notification.CategoryEvent);
            this.builder.SetOngoing(true);
            this.builder.SetAutoCancel(false);

            //Intent resultIntent = new Intent(Forms.Context, typeof(HomePage));

            //this.stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(HomePage)));
            //this.stackBuilder.AddNextIntent(resultIntent);

            //PendingIntent pending = this.stackBuilder.GetPendingIntent(0, PendingIntentFlags.OneShot);

            //this.builder.SetContentIntent(pending);

            Notification notification = this.builder.Build();
            this.notifyManager.Notify(Id, notification);
        }

        public void UpdateNotification(string title, string text, bool endCounter)
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

            //Intent resultIntent = new Intent(Forms.Context, typeof(HomePage));

            //this.stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(HomePage)));
            //this.stackBuilder.AddNextIntent(resultIntent);

            //PendingIntent pending = this.stackBuilder.GetPendingIntent(0, PendingIntentFlags.OneShot);

            //this.builder.SetContentIntent(pending);

            //Notification.Action action = new Notification.Action.Builder(Resource.Drawable.icon, "Test", pending).Build();

            //builder.AddAction(action);

            Notification notification = this.builder.Build();
            this.notifyManager.Notify(Id, notification);
        }

        public void CancelNotification()
        {
            return;
        }
    }
}