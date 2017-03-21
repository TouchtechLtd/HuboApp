using Android.Graphics.Drawables;
using Hubo.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Hubo;
using static Java.Util.ResourceBundle;

[assembly: ExportRenderer(typeof(MyButton), typeof(MyButtonRenderer))]
namespace Hubo.Droid.Renderers
{
    public class MyButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
        }
    }
}