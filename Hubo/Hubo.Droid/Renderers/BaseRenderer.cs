// <copyright file="BaseRenderer.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using Android.Views.InputMethods;
using Android.Widget;
using Hubo;
using Hubo.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BaseEntry), typeof(BaseRenderer))]

namespace Hubo.Droid
{
    public class BaseRenderer : EntryRenderer
    {
        public BaseRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            BaseEntry base_entry = (BaseEntry)this.Element;

            this.SetReturnType(base_entry);

            this.Control.EditorAction += (object sender, TextView.EditorActionEventArgs args) =>
            {
                if (base_entry.ReturnType != ReturnType.Next)
                {
                    base_entry.Unfocus();
                }

                base_entry.InvokeCompleted();
            };
        }

        private void SetReturnType(BaseEntry base_entry)
        {
            ReturnType type = base_entry.ReturnType;

            switch (type)
            {
                case ReturnType.Go:
                    this.Control.ImeOptions = ImeAction.Go;
                    this.Control.SetImeActionLabel("Go", ImeAction.Go);
                    break;
                case ReturnType.Next:
                    this.Control.ImeOptions = ImeAction.Next;
                    this.Control.SetImeActionLabel("Next", ImeAction.Next);
                    break;
                case ReturnType.Send:
                    this.Control.ImeOptions = ImeAction.Send;
                    this.Control.SetImeActionLabel("Send", ImeAction.Send);
                    break;
                case ReturnType.Search:
                    this.Control.ImeOptions = ImeAction.Search;
                    this.Control.SetImeActionLabel("Search", ImeAction.Search);
                    break;
                default:
                    this.Control.ImeOptions = ImeAction.Done;
                    this.Control.SetImeActionLabel("Done", ImeAction.Done);
                    break;
            }
        }
    }
}