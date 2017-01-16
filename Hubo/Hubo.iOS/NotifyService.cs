using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace Hubo.iOS
{
    public class NotifyService : INotifyService
    {
        public void LocalNotification(string title, string text, DateTime time)
        {
            UILocalNotification notification = new UILocalNotification();
            notification.FireDate = NSDate.FromTimeIntervalSinceNow(30);
            notification.AlertTitle = title;
            notification.AlertAction = title;
            notification.AlertBody = text;
            notification.ApplicationIconBadgeNumber = 1;
            notification.SoundName = UILocalNotification.DefaultSoundName;

            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }
    }
}
