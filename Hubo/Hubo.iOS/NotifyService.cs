// <copyright file="NotifyService.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using Hubo.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(NotifyService))]

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
        public NotifyService()
        {
        }

        public void PresentNotification(string title, string text, bool endCounter)
        {
            UILocalNotification notification = new UILocalNotification()
            {
                FireDate = NSDate.Now,
                AlertTitle = title,
                AlertAction = title,
                AlertBody = text
            };
            notification.ApplicationIconBadgeNumber = 0;
            notification.SoundName = UILocalNotification.DefaultSoundName;

            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }

        public void UpdateNotification(string title, string text, bool endCounter)
        {
            UNUserNotificationCenter notifications = UNUserNotificationCenter.Current;
            notifications.RemoveAllPendingNotificationRequests();
            notifications.RemoveAllDeliveredNotifications();

            UILocalNotification notification = new UILocalNotification()
            {
                FireDate = NSDate.Now,
                AlertTitle = title,
                AlertAction = title,
                AlertBody = text
            };
            notification.ApplicationIconBadgeNumber = 0;
            notification.SoundName = UILocalNotification.DefaultSoundName;

            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }
    }
}
