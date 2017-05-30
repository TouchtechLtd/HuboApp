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
using Xamarin.Forms;
using Hubo.Droid;

[assembly: Dependency(typeof(ServiceControl))]
namespace Hubo.Droid
{
    public class ServiceControl : IServiceController
    {
        private BackgroundServiceConnection bgServiceConnection;

        public void StartService()
        {
            Intent serviceIntent = new Intent(Forms.Context, typeof(BackgroundService));

            this.bgServiceConnection = new BackgroundServiceConnection();

            Forms.Context.BindService(serviceIntent, this.bgServiceConnection, Bind.AutoCreate);
        }

        public void StopService()
        {
            Intent serviceIntent = new Intent(Forms.Context, typeof(BackgroundService));

            this.bgServiceConnection = new BackgroundServiceConnection();

            Forms.Context.BindService(serviceIntent, this.bgServiceConnection, Bind.AutoCreate);
        }
    }
}