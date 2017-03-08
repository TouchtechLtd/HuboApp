// <copyright file="NotifyService.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo.iOS
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Foundation;
    using UIKit;
    using UserNotifications;

    public class NotifyService : INotifyService
    {
        public void LocalNotification(string title, string text, DateTime time, int notifyId)
        {
            NSString id = (NSString)notifyId.ToString();

            UILocalNotification notification = new UILocalNotification();
            notification.FireDate = NSDate.FromTimeIntervalSinceNow(time.Second);
            notification.AlertTitle = title;
            notification.AlertAction = title;
            notification.AlertBody = text;
            notification.SetValueForKey(notification, id);
            notification.ApplicationIconBadgeNumber = 1;
            notification.SoundName = UILocalNotification.DefaultSoundName;

            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }

        public void CancelNotification(int notifyId)
        {
            UNUserNotificationCenter notifications = UNUserNotificationCenter.Current;
            var pending = notifications.GetPendingNotificationRequestsAsync().Result;

            foreach (var item in pending)
            {
                if (item.Identifier == notifyId.ToString())
                {
                    string notificationtest = item.Identifier;

                    string[] idtest = { notificationtest };

                    // notifications.RemovePendingNotificationRequests(idtest);
                }
            }

            string notification = pending[pending.Length - 1].Identifier;

            string[] id = { notification };

            // notifications.RemovePendingNotificationRequests(id);
        }
    }
}
