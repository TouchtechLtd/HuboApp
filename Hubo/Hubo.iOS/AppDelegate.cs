using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using HockeyApp.iOS;
using UIKit;
using Syncfusion.SfChart.XForms.iOS.Renderers;
using Syncfusion.SfGauge.XForms.iOS;
using Telerik.XamarinForms.Common.iOS;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(Telerik.XamarinForms.Chart.RadCartesianChart), typeof(Telerik.XamarinForms.ChartRenderer.iOS.CartesianChartRenderer))]

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
            TelerikForms.Init();
            new SfChartRenderer();
            new SfGaugeRenderer();
            BITHockeyManager manager = BITHockeyManager.SharedHockeyManager;
            manager.Configure(Configuration.HockeyAppIdIOS);
            manager.StartManager();

            if (UIDevice.CurrentDevice.CheckSystemVersion(8,0))
            {
                var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }

            if (options != null && options.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey))
            {
                var localNotification = options[UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
                if (localNotification != null)
                {
                    UIAlertController okayAlertController = UIAlertController.Create(localNotification.AlertAction, localNotification.AlertBody, UIAlertControllerStyle.Alert);
                    okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default,  null));

                    Window.RootViewController.PresentViewController(okayAlertController, true, null);

                    UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
                }
            }

            LoadApplication(new Hubo.Application());

            return base.FinishedLaunching(app, options);
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            UIAlertController okayAlertController = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
            okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

            Window.RootViewController.PresentViewController(okayAlertController, true, null);

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }
    }
}
