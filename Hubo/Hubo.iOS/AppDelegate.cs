using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using HockeyApp.iOS;
using UIKit;
using Syncfusion.SfChart.XForms.iOS.Renderers;
using Syncfusion.SfGauge.XForms.iOS;

namespace Hubo.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            #if ENABLE_TEST_CLOUD
                Xamarin.Calabash.Start();
            #endif

            global::Xamarin.Forms.Forms.Init();
            new SfChartRenderer();
            new SfGaugeRenderer();
            BITHockeyManager manager = BITHockeyManager.SharedHockeyManager;
            manager.Configure(Configuration.HockeyAppIdIOS);
            manager.StartManager();

            //String that contains the path and loads to the preloaded database
            string dbPath = FileAccessHelper.GetLocalFilePath("Hubo.db3");

            LoadApplication(new Hubo.Application());

            return base.FinishedLaunching(app, options);
        }
    }
}
