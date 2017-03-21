using Xamarin.Forms.Platform.Android;
using Hubo;
using Hubo.Droid;
using Xamarin.Forms;
using Hubo.Droid.Renderers;
using Android.Graphics.Drawables;
using static Java.Util.ResourceBundle;

[assembly: ExportRenderer(typeof(MyEntry), typeof(MyEntryRenderer))]
namespace Hubo.Droid.Renderers
{
    public class MyEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (this.Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetCornerRadius(10);
                gd.SetColor(Color.White.ToAndroid());
                this.Control.SetBackground(gd);
            }
        }
    }
}