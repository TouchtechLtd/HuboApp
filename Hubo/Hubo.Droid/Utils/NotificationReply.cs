// <copyright file="NotificationReply.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo.Droid.Utils
{
    using System.Threading.Tasks;
    using Android.App;
    using Android.Content;
    using Xamarin.Forms;

    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { NotifyService.KEY_TOGGLE_SHIFT })]
    [IntentFilter(new[] { NotifyService.KEY_TOGGLE_DRIVE })]
    [IntentFilter(new[] { NotifyService.KEY_TOGGLE_BREAK })]
    [IntentFilter(new[] { CloseApp.KEY_RESTART_APP })]

    public class NotificationReply : BroadcastReceiver
    {
        public override async void OnReceive(Context context, Intent intent)
        {
            if (intent.Action != CloseApp.KEY_RESTART_APP) {
                Intent open = new Intent(Forms.Context, typeof(MainActivity))
                    .AddFlags(ActivityFlags.SingleTop);
                Forms.Context.ApplicationContext.StartActivity(open);

                await Task.Delay(10);

                Intent close = new Intent(Intent.ActionCloseSystemDialogs);
                Forms.Context.SendBroadcast(close);

                await Task.Delay(10);

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
            else if (intent.Action == CloseApp.KEY_RESTART_APP)
            {
                Intent open = new Intent(Forms.Context, typeof(MainActivity))
                    .AddFlags(ActivityFlags.SingleTop);
                Forms.Context.ApplicationContext.StartActivity(open);

                await Task.Delay(10);
            }
        }
    }
}