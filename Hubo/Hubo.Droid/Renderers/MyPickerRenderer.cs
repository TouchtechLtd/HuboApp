// <copyright file="MyPickerRenderer.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using Android.Graphics.Drawables;
using Hubo;
using Hubo.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MyPicker), typeof(MyPickerRenderer))]
namespace Hubo.Droid.Renderers
{
    public class MyPickerRenderer : PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
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