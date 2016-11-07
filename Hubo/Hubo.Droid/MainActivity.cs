using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HockeyApp.Android;
using Syncfusion.SfChart.XForms.Droid;
using Syncfusion.SfGauge.XForms.Droid;
using Com.Syncfusion.Charts;
using Com.Syncfusion.Gauges;

namespace Hubo.Droid
{
    [Activity(Label = "Hubo", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            new SfGaugeRenderer();
            new SfChartRenderer();
            //Xamarin.Forms.Forms.ViewInitialized += (sender, e) =>
            //{
            //    if (!(e.NativeView is SfGauge)) return;

            //    // (e.NativeView as SfChart).SetOnTouchListener(new OnTouchListenerExt());
            //};
            CrashManager.Register(this, Configuration.HockeyAppId);

            LoadApplication(new App());
        }
    }
}

