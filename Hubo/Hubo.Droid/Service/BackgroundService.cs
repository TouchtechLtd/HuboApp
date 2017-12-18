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
using Hubo.Droid;
using Xamarin.Forms;
using Android.Support.V4.App;

namespace Hubo.Droid
{
    [Service(Label = "AndroidBackgroundService", Exported = true, Name = "triotech.hubo.android.Background")]
    [IntentFilter(new string[] { "triotech.hubo.droid.backgroundService"})]
    public class BackgroundService : Service
    {
        private NotificationManager notifyManager = Forms.Context.GetSystemService(Context.NotificationService) as NotificationManager;
        private NotificationCompat.Builder builder = new NotificationCompat.Builder(Forms.Context);

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override IBinder OnBind(Intent intent)
        {
            return new BackgroundBinder(this);
        }

        public override bool OnUnbind(Intent intent)
        {
            return base.OnUnbind(intent);
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            this.StartForgroundService();

            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            this.StopForeground(true);
            base.OnDestroy();
        }

        private void StartForgroundService()
        {
            this.builder.SetVisibility((int)NotificationVisibility.Public);
            this.builder.SetContentTitle("Background Service");
            this.builder.SetContentText("Backgound Service in Foreground");
            this.builder.SetSmallIcon(Resource.Drawable.IconSmall);
            this.builder.SetCategory(Notification.CategoryEvent);
            this.builder.SetOngoing(true);
            this.builder.SetAutoCancel(false);
            this.builder.SetShowWhen(false);

            PendingIntent pending = PendingIntent.GetActivity(Forms.Context, 0, new Intent(Forms.Context, typeof(MainActivity)), PendingIntentFlags.OneShot);

            this.builder.SetContentIntent(pending);

            Notification notification = this.builder.Build();

            this.StartForeground((int)NotificationFlags.ForegroundService, notification);
        }

        private void UpdateNotification()
        {
            this.builder.SetContentText("Message from Background service");
            this.builder.SetContentTitle("Background Service Notification");

            Notification notification = this.builder.Build();

            this.notifyManager.Notify(1000, notification);
        }

        public void StopService()
        {
            this.StopForeground(true);

            this.StopSelf();
        }
    }
}