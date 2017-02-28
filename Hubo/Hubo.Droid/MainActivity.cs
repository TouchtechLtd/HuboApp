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
using Syncfusion.SfAutoComplete.XForms.Droid;
using Plugin.Permissions;
using Acr.UserDialogs;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(Telerik.XamarinForms.Chart.RadCartesianChart), typeof(Telerik.XamarinForms.ChartRenderer.Android.CartesianChartRenderer))]

namespace Hubo.Droid
{
    [Activity(Label = "Hubo", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation =ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            UserDialogs.Init(() => (Activity)Forms.Context);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            new SfGaugeRenderer();
            new SfChartRenderer();
            new SfAutoCompleteRenderer();
            if (((int)Build.VERSION.SdkInt) >= 21)
            {
                Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#0e1d25"));
            }
            //Xamarin.Forms.Forms.ViewInitialized += (sender, e) =>
            //{
            //    if (!(e.NativeView is SfGauge)) return;

            //    // (e.NativeView as SfChart).SetOnTouchListener(new OnTouchListenerExt());
            //};
            CrashManager.Register(this, Configuration.HockeyAppIdDroid);

            //String that contains the path and loads the preloaded database
            string dbPath = FileAccessHelper.GetLocalFilePath("Hubo.db3");

            //AndroidBug5497WorkaroundForXamarinAndroid.assistActivity(this);
            //ResizeBugWorkaround.assistActivity(this);

            LoadApplication(new Application());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

