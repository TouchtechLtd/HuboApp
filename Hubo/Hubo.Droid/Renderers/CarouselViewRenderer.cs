// <copyright file="CarouselViewRenderer.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Reflection;
using System.Timers;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Hubo;
using Hubo.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CarouselView), typeof(CarouselViewRenderer))]
namespace Hubo.Droid
{
    public class CarouselViewRenderer : ScrollViewRenderer
    {
        private int prevScrollX;
        private int deltaX;
        private bool motionDown;
        private Timer deltaXResetTimer;
        private Timer scrollStopTimer;
        private HorizontalScrollView scrollView;

        private bool initialized = false;

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
            if (this.initialized)
            {
                return;
            }

            this.initialized = true;
            var carouselView = (CarouselView)this.Element;
            this.scrollView.ScrollTo(carouselView.SelectedIndex * this.Width, 0);
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (e.NewElement == null)
            {
                return;
            }

            this.deltaXResetTimer = new Timer(100) { AutoReset = false };
            this.deltaXResetTimer.Elapsed += (object sender, ElapsedEventArgs args) => this.deltaX = 0;

            this.scrollStopTimer = new Timer(200) { AutoReset = false };
            this.scrollStopTimer.Elapsed += (object sender, ElapsedEventArgs args) => this.UpdateSelectedIndex();

            e.NewElement.PropertyChanged += this.ElementPropertyChanged;
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            if (this.initialized && (w != oldw))
            {
                this.initialized = false;
            }

            base.OnSizeChanged(w, h, oldw, oldh);
        }

        private void ElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Renderer")
            {
                this.scrollView = (HorizontalScrollView)typeof(ScrollViewRenderer).GetField("_hScrollView", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);

                this.scrollView.HorizontalScrollBarEnabled = false;
                this.scrollView.Touch += this.HScrollViewTouch;
            }

            if (e.PropertyName == CarouselView.SelectedIndexProperty.PropertyName && !this.motionDown)
            {
                this.ScrollToIndex(((CarouselView)this.Element).SelectedIndex);
            }
        }

        private void ScrollToIndex(int targetIndex)
        {
            var targetX = targetIndex * this.scrollView.Width;
            this.scrollView.Post(new Java.Lang.Runnable(() =>
            {
                this.scrollView.SmoothScrollTo(targetX, 0);
            }));
        }

        private void HScrollViewTouch(object sender, TouchEventArgs e)
        {
            e.Handled = false;

            switch (e.Event.Action)
            {
                case MotionEventActions.Move:
                    this.deltaXResetTimer.Stop();
                    this.deltaX = this.scrollView.ScrollX - this.prevScrollX;
                    this.prevScrollX = this.scrollView.ScrollX;

                    this.UpdateSelectedIndex();

                    this.deltaXResetTimer.Start();
                    break;
                case MotionEventActions.Down:
                    this.motionDown = true;
                    this.scrollStopTimer.Stop();
                    break;
                case MotionEventActions.Up:
                    this.motionDown = false;
                    this.SnapScroll();
                    this.scrollStopTimer.Start();
                    break;
            }
        }

        private void SnapScroll()
        {
            var roughIndex = (float)this.scrollView.ScrollX / this.scrollView.Width;

            var targetIndex = this.deltaX < 0 ? Java.Lang.Math.Floor(roughIndex) : this.deltaX > 0 ? Java.Lang.Math.Ceil(roughIndex) : Java.Lang.Math.Round(roughIndex);

            this.ScrollToIndex((int)targetIndex);
        }

        private void UpdateSelectedIndex()
        {
            var center = this.scrollView.ScrollX + (this.scrollView.Width / 2);
            var carouselView = (CarouselView)this.Element;
            var calculatedIdx = center / this.scrollView.Width;

            using (var h = new Handler(Looper.MainLooper))
            {
                h.Post(() => carouselView.SelectedIndex = calculatedIdx);
            }
        }
    }
}