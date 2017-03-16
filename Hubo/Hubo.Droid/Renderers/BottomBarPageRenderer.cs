// <copyright file="BottomBarPageRenderer.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using BottomNavigationBar;
using BottomNavigationBar.Listeners;
using Hubo;
using Hubo.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(BottomBarPage), typeof(BottomBarPageRenderer))]

namespace Hubo.Droid
{
    public class BottomBarPageRenderer : VisualElementRenderer<BottomBarPage>, IOnTabClickListener
    {
        private bool disposed;
        private BottomNavigationBar.BottomBar bottomBar;
        private FrameLayout frameLayout;
        private IPageController pageController;

        public BottomBarPageRenderer()
        {
            this.AutoPackage = false;
        }

        public void OnTabSelected(int position)
        {
            // Do we need this call? It's also done in OnElementPropertyChanged
            this.SwitchContent(this.Element.Children[position]);
            var bottomBarPage = this.Element as BottomBarPage;
            bottomBarPage.CurrentPage = this.Element.Children[position];
        }

        public void OnTabReSelected(int position)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.disposed = true;

                this.RemoveAllViews();

                foreach (Page pageToRemove in this.Element.Children)
                {
                    IVisualElementRenderer pageRenderer = Platform.GetRenderer(pageToRemove);

                    if (pageRenderer != null)
                    {
                        pageRenderer.ViewGroup.RemoveFromParent();
                        pageRenderer.Dispose();
                    }
                }

                if (this.bottomBar != null)
                {
                    this.bottomBar.SetOnTabClickListener(null);
                    this.bottomBar.Dispose();
                    this.bottomBar = null;
                }

                if (this.frameLayout != null)
                {
                    this.frameLayout.Dispose();
                    this.frameLayout = null;
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            this.pageController.SendAppearing();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            this.pageController.SendDisappearing();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<BottomBarPage> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                BottomBarPage bottomBarPage = e.NewElement;

                if (this.bottomBar == null)
                {
                    this.pageController = PageController.Create(bottomBarPage);

                    // create a view which will act as container for Page's
                    this.frameLayout = new FrameLayout(Forms.Context)
                    {
                        LayoutParameters = new FrameLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent, GravityFlags.Fill)
                    };
                    this.AddView(this.frameLayout, 0);

                    // create bottomBar control
                    this.bottomBar = BottomNavigationBar.BottomBar.Attach(this.frameLayout, null);
                    this.bottomBar.NoTabletGoodness();
                    if (bottomBarPage.FixedMode)
                    {
                        this.bottomBar.UseFixedMode();
                    }

                    switch (bottomBarPage.BarTheme)
                    {
                        case BottomBarPage.BarThemeTypes.Light:
                            break;
                        case BottomBarPage.BarThemeTypes.DarkWithAlpha:
                            this.bottomBar.UseDarkThemeWithAlpha(true);
                            break;
                        case BottomBarPage.BarThemeTypes.DarkWithoutAlpha:
                            this.bottomBar.UseDarkThemeWithAlpha(false);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    this.bottomBar.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                    this.bottomBar.SetOnTabClickListener(this);

                    this.UpdateTabs();
                    this.UpdateBarBackgroundColor();
                    this.UpdateBarTextColor();
                }

                if (bottomBarPage.CurrentPage != null)
                {
                    this.SwitchContent(bottomBarPage.CurrentPage);
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(TabbedPage.CurrentPage))
            {
                this.SwitchContent(this.Element.CurrentPage);
            }
            else if (e.PropertyName == NavigationPage.BarBackgroundColorProperty.PropertyName)
            {
                this.UpdateBarBackgroundColor();
            }
            else if (e.PropertyName == NavigationPage.BarTextColorProperty.PropertyName)
            {
                this.UpdateBarTextColor();
            }
        }

        protected virtual void SwitchContent(Page view)
        {
            this.Context.HideKeyboard(this);

            this.frameLayout.RemoveAllViews();

            if (view == null)
            {
                return;
            }

            if (Platform.GetRenderer(view) == null)
            {
                Platform.SetRenderer(view, Platform.CreateRenderer(view));
            }

            this.frameLayout.AddView(Platform.GetRenderer(view).ViewGroup);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            int width = r - l;
            int height = b - t;

            var context = this.Context;

            this.bottomBar.Measure(MeasureSpecFactory.MakeMeasureSpec(width, MeasureSpecMode.Exactly), MeasureSpecFactory.MakeMeasureSpec(height, MeasureSpecMode.AtMost));
            int tabsHeight = Math.Min(height, Math.Max(this.bottomBar.MeasuredHeight, this.bottomBar.MinimumHeight));

            if (width > 0 && height > 0)
            {
                this.pageController.SetContainerArea(new Rectangle(0, 0, context.FromPixels(width), context.FromPixels(this.frameLayout.Height)));
                ObservableCollection<Element> internalChildren = this.pageController.InternalChildren;

                for (var i = 0; i < internalChildren.Count; i++)
                {
                    var child = internalChildren[i] as VisualElement;

                    if (child == null)
                    {
                        continue;
                    }

                    IVisualElementRenderer renderer = Platform.GetRenderer(child);
                    var navigationRenderer = renderer as NavigationPageRenderer;
                }

                this.bottomBar.Measure(MeasureSpecFactory.MakeMeasureSpec(width, MeasureSpecMode.Exactly), MeasureSpecFactory.MakeMeasureSpec(tabsHeight, MeasureSpecMode.Exactly));
                this.bottomBar.Layout(0, 0, width, tabsHeight);
            }

            base.OnLayout(changed, l, t, r, b);
        }

        private void UpdateBarBackgroundColor()
        {
            if (this.disposed || this.bottomBar == null)
            {
                return;
            }

            this.bottomBar.SetBackgroundColor(this.Element.BarBackgroundColor.ToAndroid());
        }

        private void UpdateBarTextColor()
        {
            if (this.disposed || this.bottomBar == null)
            {
                return;
            }

            this.bottomBar.SetActiveTabColor(this.Element.BarTextColor.ToAndroid());

            // The problem SetActiveTabColor does only work in fiexed mode // haven't found yet how to set text color for tab items on_bottomBar, doesn't seem to have a direct way
        }

        private void UpdateTabs()
        {
            // create tab items
            this.SetTabItems();

            // set tab colors
            this.SetTabColors();
        }

        private void SetTabItems()
        {
            BottomBarTab[] tabs = this.Element.Children.Select(page =>
            {
                var tabIconId = ResourceManagerEx.IdFromTitle(page.Icon, ResourceManager.DrawableClass);
                return new BottomBarTab(tabIconId, page.Title);
            }).ToArray();

            this.bottomBar.SetItems(tabs);
        }

        private void SetTabColors()
        {
            for (int i = 0; i < this.Element.Children.Count; ++i)
            {
                Page page = this.Element.Children[i];

                Color? tabColor = page.GetTabColor();

                if (tabColor != null)
                {
                    this.bottomBar.MapColorForTab(i, tabColor.Value.ToAndroid());
                }
            }
        }
    }
}