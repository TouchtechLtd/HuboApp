// <copyright file="BaseRenderer.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

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

            if (this.Control != null && base_entry != null)
            {
                this.SetReturnType(base_entry);
            }

            this.Control.ShouldReturn += (UITextField tf) =>
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
                    this.Control.ReturnKeyType = UIReturnKeyType.Go;
                    break;
                case ReturnType.Next:
                    this.Control.ReturnKeyType = UIReturnKeyType.Next;
                    break;
                case ReturnType.Send:
                    this.Control.ReturnKeyType = UIReturnKeyType.Send;
                    break;
                case ReturnType.Search:
                    this.Control.ReturnKeyType = UIReturnKeyType.Search;
                    break;
                case ReturnType.Done:
                    this.Control.ReturnKeyType = UIReturnKeyType.Done;
                    break;
                default:
                    this.Control.ReturnKeyType = UIReturnKeyType.Default;
                    break;
            }
        }
    }
}
