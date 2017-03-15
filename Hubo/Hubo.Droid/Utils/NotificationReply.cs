// <copyright file="NotificationReply.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo.Droid.Utils
{
    using Android.App;
    using Android.Content;
    using Xamarin.Forms;

    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { NotifyService.KEY_TOGGLE_SHIFT })]
    [IntentFilter(new[] { NotifyService.KEY_TOGGLE_BREAK })]

    public class NotificationReply : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Intent open = new Intent(Forms.Context, typeof(MainActivity))
                .AddFlags(ActivityFlags.SingleTop);
            Forms.Context.ApplicationContext.StartActivity(open);

            Intent close = new Intent(Intent.ActionCloseSystemDialogs);
            Forms.Context.SendBroadcast(close);

            switch (intent.Action)
            {
                case NotifyService.KEY_TOGGLE_SHIFT:
                    MessagingCenter.Send<string>("Toggle Shift", "Toggle Shift");
                    break;
                case NotifyService.KEY_TOGGLE_BREAK:
                    MessagingCenter.Send<string>("Toggle Break", "Toggle Break");
                    break;
                case NotifyService.KEY_TOGGLE_DRIVE:
                    MessagingCenter.Send<string>("Toggle Drive", "Toggle Drive");
                    break;
                default:
                    break;
            }
        }
    }
}