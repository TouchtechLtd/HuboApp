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
    public class BackgroundBinder : Binder
    {
        private BackgroundService service;

        public BackgroundBinder(BackgroundService bService)
        {
            this.Service = bService;
        }

        public BackgroundService Service
        {
            get
            {
                return this.service;
            }

            private set
            {
                this.service = value;
            }
        }
    }
}