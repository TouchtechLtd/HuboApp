using Android.Widget;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Hubo;
using Hubo.Droid;
using Android.Views.InputMethods;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(BaseEntry), typeof(BaseRenderer))]

namespace Hubo.Droid
{
    public class BaseRenderer : EntryRenderer
    {
        public BaseRenderer() { }

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

            BaseEntry base_entry = (BaseEntry)Element;

            SetReturnType(base_entry);

            Control.EditorAction += (object sender, TextView.EditorActionEventArgs args) =>
            {
                if (base_entry.ReturnType != ReturnType.Next)
                    base_entry.Unfocus();

                base_entry.InvokeCompleted();
            };
        }

        private void SetReturnType(BaseEntry base_entry)
        {
            ReturnType type = base_entry.ReturnType;

            switch (type)
            {
                case ReturnType.Go:
                    Control.ImeOptions = ImeAction.Go;
                    Control.SetImeActionLabel("Go", ImeAction.Go);
                    break;
                case ReturnType.Next:
                    Control.ImeOptions = ImeAction.Next;
                    Control.SetImeActionLabel("Next", ImeAction.Next);
                    break;
                case ReturnType.Send:
                    Control.ImeOptions = ImeAction.Send;
                    Control.SetImeActionLabel("Send", ImeAction.Send);
                    break;
                case ReturnType.Search:
                    Control.ImeOptions = ImeAction.Search;
                    Control.SetImeActionLabel("Search", ImeAction.Search);
                    break;
                default:
                    Control.ImeOptions = ImeAction.Done;
                    Control.SetImeActionLabel("Done", ImeAction.Done);
                    break;
            }
        }
    }
}