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

namespace Hubo.Droid
{
    public class BackgroundServiceConnection : Java.Lang.Object, IServiceConnection
    {
        private bool isConnected;
        private BackgroundBinder binder;

        public BackgroundServiceConnection()
        {
            this.IsConnected = false;
            this.Binder = null;
        }

        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }

            private set
            {
                this.isConnected = value;
            }
        }

        public BackgroundBinder Binder
        {
            get
            {
                return this.binder;
            }

            private set
            {
                this.binder = value;
            }
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            this.Binder = service as BackgroundBinder;
            this.IsConnected = this.Binder != null;
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            this.IsConnected = false;
            this.Binder = null;
        }
    }
}