using Hubo;
using Hubo.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BaseEntry), typeof(BaseRenderer))]

namespace Hubo.iOS
{
    public class BaseRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            BaseEntry base_entry = (BaseEntry)this.Element;

            if (Control != null && base_entry != null)
                SetReturnType(base_entry);

            Control.ShouldReturn += (UITextField tf) =>
            {
                base_entry.InvokeCompleted();
                return true;
            };
        }

        private void SetReturnType(BaseEntry base_entry)
        {
            ReturnType type = base_entry.ReturnType;

            switch (type)
            {
                case ReturnType.Go:
                    Control.ReturnKeyType = UIReturnKeyType.Go;
                    break;
                case ReturnType.Next:
                    Control.ReturnKeyType = UIReturnKeyType.Next;
                    break;
                case ReturnType.Send:
                    Control.ReturnKeyType = UIReturnKeyType.Send;
                    break;
                case ReturnType.Search:
                    Control.ReturnKeyType = UIReturnKeyType.Search;
                    break;
                case ReturnType.Done:
                    Control.ReturnKeyType = UIReturnKeyType.Done;
                    break;
                default:
                    Control.ReturnKeyType = UIReturnKeyType.Default;
                    break;
            }
        }
    }
}
