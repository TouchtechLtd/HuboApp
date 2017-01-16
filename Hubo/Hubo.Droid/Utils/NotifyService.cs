using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Xamarin.Forms;

namespace Hubo.Droid
{
    public class NotifyService : INotifyService
    {
        public void LocalNotification(string title, string text, DateTime time)
        {
            Notification.Builder builder = new Notification.Builder(Forms.Context)
                .SetContentTitle(title)
                .SetContentText(text)
                .SetSmallIcon(Resource.Drawable.icon)
                .SetPriority((int)NotificationPriority.High)
                .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate | NotificationDefaults.Lights);

            Notification notification = builder.Build();

            NotificationManager notifyManager = Forms.Context.GetSystemService(Context.NotificationService) as NotificationManager;

            const int notifyId = 0;
            notifyManager.Notify(notifyId, notification);

            builder.SetWhen(time.Millisecond);
        }
    }
}