// <copyright file="MainActivity.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using HockeyApp.Android;
using Plugin.Permissions;
using RoundedBoxView.Forms.Plugin.Droid;
using Syncfusion.SfAutoComplete.XForms.Droid;
using Syncfusion.SfChart.XForms.Droid;
using Syncfusion.SfGauge.XForms.Droid;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(Telerik.XamarinForms.Chart.RadCartesianChart), typeof(Telerik.XamarinForms.ChartRenderer.Android.CartesianChartRenderer))]

namespace Hubo.Droid
{
    [Activity(Label = "Hubo", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation =ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            UserDialogs.Init(() => (Activity)Forms.Context);
            Forms.Init(this, bundle);
            RoundedBoxViewRenderer.Init();
            new SfGaugeRenderer();
            new SfChartRenderer();
            new SfAutoCompleteRenderer();
            new FAB.Forms.FloatingActionButton();
            if (((int)Build.VERSION.SdkInt) >= 21)
            {
                this.Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#0e1d25"));
            }

            CrashManager.Register(this, Configuration.HockeyAppIdDroid);

            this.LoadApplication(new Application());
        }
    }
}