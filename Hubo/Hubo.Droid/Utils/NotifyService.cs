// <copyright file="NotifyService.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using Hubo.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(NotifyService))]

namespace Hubo.Droid
{
    using System;
    using Android.App;
    using Android.Content;

    public class NotifyService : INotifyService
    {
        public NotifyService()
        {
        }

        public void LocalNotification(string title, string text, DateTime time, int id)
        {
            Notification.Builder builder = new Notification.Builder(Forms.Context)
                .SetContentTitle(title)
                .SetContentText(text)
                .SetSmallIcon(Resource.Drawable.icon)
                .SetPriority((int)NotificationPriority.High)
                .SetWhen(time.Millisecond)
                .SetCategory(Notification.CategoryAlarm)
                .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate | NotificationDefaults.Lights);

            Notification notification = builder.Build();

            NotificationManager notifyManager = Forms.Context.GetSystemService(Context.NotificationService) as NotificationManager;

            notifyManager.Notify(id, notification);
        }

        public void CancelNotification(int notificationId)
        {
            NotificationManager notifyManager = Forms.Context.GetSystemService(Context.NotificationService) as NotificationManager;
            notifyManager.Cancel(notificationId);
        }
    }
}