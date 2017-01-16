using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Hubo.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloseApplication))]

namespace Hubo.iOS
{
    public class CloseApplication : ICloseApplication
    {
        public void closeApplication()
        {
            Thread.CurrentThread.Abort();
        }
    }
}
