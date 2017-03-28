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
    using Android.Support.V4.App;

    public class NotifyService : INotifyService
    {
        public const string KEY_TOGGLE_SHIFT = "triotech.hubo.droid.SHIFT";
        public const string KEY_TOGGLE_BREAK = "triotech.hubo.droid.BREAK";
        public const string KEY_TOGGLE_DRIVE = "triotech.hubo.droid.DRIVE";

        private const int Id = 5;
        private NotificationManager notifyManager = Forms.Context.GetSystemService(Context.NotificationService) as NotificationManager;
        private NotificationCompat.Builder builder = new NotificationCompat.Builder(Forms.Context);

        public NotifyService()
        {
        }

        public void PresentNotification(string title, string text, bool endCounter)
        {
            this.builder.SetVisibility((int)NotificationVisibility.Public);
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
                this.builder.SetDefaults((int)NotificationDefaults.All);
            }
            else
            {
                this.builder.SetPriority((int)NotificationPriority.Min);
                this.builder.SetDefaults((int)NotificationDefaults.Vibrate);
            }

            if (title.Contains("Ready"))
            {
                this.builder.MActions.Clear();

                Intent shiftIntent = new Intent(KEY_TOGGLE_SHIFT);
                PendingIntent shiftPending = PendingIntent.GetBroadcast(Forms.Context, 0, shiftIntent, PendingIntentFlags.UpdateCurrent);

                NotificationCompat.Action action = new NotificationCompat.Action.Builder(Resource.Drawable.icon, "Start Shift", shiftPending).Build();

                this.builder.AddAction(action);
            }
            else if (title.Contains("Shift"))
            {
                this.builder.MActions.Clear();

                Intent shiftIntent = new Intent(KEY_TOGGLE_SHIFT);
                PendingIntent shiftPending = PendingIntent.GetBroadcast(Forms.Context, 0, shiftIntent, PendingIntentFlags.UpdateCurrent);

                Intent driveIntent = new Intent(KEY_TOGGLE_DRIVE);
                PendingIntent drivePending = PendingIntent.GetBroadcast(Forms.Context, 0, driveIntent, PendingIntentFlags.UpdateCurrent);

                Intent breakIntent = new Intent(KEY_TOGGLE_BREAK);
                PendingIntent breakPending = PendingIntent.GetBroadcast(Forms.Context, 1, breakIntent, PendingIntentFlags.UpdateCurrent);

                NotificationCompat.Action action = new NotificationCompat.Action.Builder(Resource.Drawable.icon, "End Shift", shiftPending).Build();

                NotificationCompat.Action actionDrive = new NotificationCompat.Action.Builder(Resource.Drawable.icon, "Start Drive", drivePending).Build();

                NotificationCompat.Action actionBreak = new NotificationCompat.Action.Builder(Resource.Drawable.icon, "Start Break", breakPending).Build();

                this.builder.AddAction(action);
                this.builder.AddAction(actionDrive);
                this.builder.AddAction(actionBreak);
            }
            else if (title.Contains("Drive"))
            {
                this.builder.MActions.Clear();

                Intent driveIntent = new Intent(KEY_TOGGLE_DRIVE);
                PendingIntent drivePending = PendingIntent.GetBroadcast(Forms.Context, 0, driveIntent, PendingIntentFlags.UpdateCurrent);

                Intent breakIntent = new Intent(KEY_TOGGLE_BREAK);
                PendingIntent breakPending = PendingIntent.GetBroadcast(Forms.Context, 1, breakIntent, PendingIntentFlags.UpdateCurrent);

                NotificationCompat.Action action = new NotificationCompat.Action.Builder(Resource.Drawable.icon, "End Drive", drivePending).Build();

                NotificationCompat.Action actionBreak = new NotificationCompat.Action.Builder(Resource.Drawable.icon, "Start Break", breakPending).Build();

                this.builder.AddAction(action);
                this.builder.AddAction(actionBreak);
            }
            else if (title.Contains("Break"))
            {
                this.builder.MActions.Clear();

                Intent intent = new Intent(KEY_TOGGLE_BREAK);
                PendingIntent pending = PendingIntent.GetBroadcast(Forms.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

                NotificationCompat.Action action = new NotificationCompat.Action.Builder(Resource.Drawable.icon, "End Break", pending).Build();

                this.builder.AddAction(action);
            }

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
                this.builder.SetDefaults((int)NotificationDefaults.All);
            }
            else
            {
                this.builder.SetPriority((int)NotificationPriority.Min);
                this.builder.SetDefaults((int)NotificationDefaults.Vibrate);
            }

            if (title.Contains("Ready"))
            {
                this.builder.MActions.Clear();

                Intent shiftIntent = new Intent(KEY_TOGGLE_SHIFT);
                PendingIntent shiftPending = PendingIntent.GetBroadcast(Forms.Context, 0, shiftIntent, PendingIntentFlags.UpdateCurrent);

                NotificationCompat.Action action = new NotificationCompat.Action.Builder(Resource.Drawable.icon, "Start Shift", shiftPending).Build();

                this.builder.AddAction(action);
            }
            else if (title.Contains("Shift"))
            {
                this.builder.MActions.Clear();

                Intent shiftIntent = new Intent(KEY_TOGGLE_SHIFT);
                PendingIntent shiftPending = PendingIntent.GetBroadcast(Forms.Context, 0, shiftIntent, PendingIntentFlags.UpdateCurrent);

                Intent driveIntent = new Intent(KEY_TOGGLE_DRIVE);
                PendingIntent drivePending = PendingIntent.GetBroadcast(Forms.Context, 0, driveIntent, PendingIntentFlags.UpdateCurrent);

                Intent breakIntent = new Intent(KEY_TOGGLE_BREAK);
                PendingIntent breakPending = PendingIntent.GetBroadcast(Forms.Context, 1, breakIntent, PendingIntentFlags.UpdateCurrent);

                NotificationCompat.Action action = new NotificationCompat.Action.Builder(Resource.Drawable.icon, "End Shift", shiftPending).Build();

                NotificationCompat.Action actionDrive = new NotificationCompat.Action.Builder(Resource.Drawable.icon, "Start Drive", drivePending).Build();

                NotificationCompat.Action actionBreak = new NotificationCompat.Action.Builder(Resource.Drawable.icon, "Start Break", breakPending).Build();

                this.builder.AddAction(action);
                this.builder.AddAction(actionDrive);
                this.builder.AddAction(actionBreak);
            }
            else if (title.Contains("Drive"))
            {
                this.builder.MActions.Clear();

                Intent driveIntent = new Intent(KEY_TOGGLE_DRIVE);
                PendingIntent drivePending = PendingIntent.GetBroadcast(Forms.Context, 0, driveIntent, PendingIntentFlags.UpdateCurrent);

                Intent breakIntent = new Intent(KEY_TOGGLE_BREAK);
                PendingIntent breakPending = PendingIntent.GetBroadcast(Forms.Context, 1, breakIntent, PendingIntentFlags.UpdateCurrent);

                NotificationCompat.Action action = new NotificationCompat.Action.Builder(Resource.Drawable.icon, "End Drive", drivePending).Build();

                NotificationCompat.Action actionBreak = new NotificationCompat.Action.Builder(Resource.Drawable.icon, "Start Break", breakPending).Build();

                this.builder.AddAction(action);
                this.builder.AddAction(actionBreak);
            }
            else if (title.Contains("Break"))
            {
                this.builder.MActions.Clear();

                Intent intent = new Intent(KEY_TOGGLE_BREAK);
                PendingIntent pending = PendingIntent.GetBroadcast(Forms.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

                NotificationCompat.Action action = new NotificationCompat.Action.Builder(Resource.Drawable.icon, "End Break", pending).Build();

                this.builder.AddAction(action);
            }

            Notification notification = this.builder.Build();
            this.notifyManager.Notify(Id, notification);
        }
    }
}