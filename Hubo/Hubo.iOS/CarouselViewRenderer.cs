// <copyright file="CarouselViewRenderer.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using CoreGraphics;
using Hubo;
using Hubo.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CarouselView), typeof(CarouselViewRenderer))]
namespace Hubo.iOS
{
    public class CarouselViewRenderer : ScrollViewRenderer
    {
        private UIScrollView native;

        public CarouselViewRenderer()
        {
            this.PagingEnabled = true;
            this.ShowsHorizontalScrollIndicator = false;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            this.ScrollToSelection(false);
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                return;
            }

            this.native = (UIScrollView)this.NativeView;
            this.native.Scrolled += this.NativeScrolled;
            e.NewElement.PropertyChanged += this.ElementPropertyChanged;
        }

        private void NativeScrolled(object sender, EventArgs e)
        {
            var center = this.native.ContentOffset.X + (this.native.Bounds.Width / 2);
            ((CarouselView)this.Element).SelectedIndex = ((int)center) / ((int)this.native.Bounds.Width);
        }

        private void ElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CarouselView.SelectedIndexProperty.PropertyName && !this.Dragging)
            {
                this.ScrollToSelection(false);
            }
        }

        private void ScrollToSelection(bool animate)
        {
            if (this.Element == null)
            {
                return;
            }

            this.native.SetContentOffset(new CoreGraphics.CGPoint(this.native.Bounds.Width * Math.Max(0, ((CarouselView)this.Element).SelectedIndex), this.native.ContentOffset.Y), animate);
        }
    }
}