// <copyright file="CarouselTabs.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Collections;
    using System.Linq;
    using Xamarin.Forms;

    public class CarouselTabs : Grid
    {
        public static BindableProperty TabBackgroundProperty = BindableProperty.Create(nameof(TabBackgroundColor), typeof(Color), typeof(CarouselTabs), Color.Gray, BindingMode.OneWay, propertyChanged: (bindable, oldValue, newValue) => { ((CarouselTabs)bindable).BackgroundColor = (Color)newValue; });
        public static BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(CarouselTabs), null, BindingMode.OneWay, propertyChanging: (bindable, oldValue, newValue) => { ((CarouselTabs)bindable).ItemsSourceChanging(); }, propertyChanged: (bindable, oldValue, newValue) => { ((CarouselTabs)bindable).ItemsSourceChanged(); });
        public static BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(CarouselTabs), null, BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue) => { ((CarouselTabs)bindable).SelectedItemChanged(); });

        private int selectedIndex;

        public CarouselTabs()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.Fill;
            BackgroundColor = TabBackgroundColor;
        }

        public Color TabBackgroundColor
        {
            get
            {
                return (Color)GetValue(TabBackgroundProperty);
            }

            set
            {
                SetValue(TabBackgroundProperty, value);
            }
        }

        public IList ItemsSource
        {
            get
            {
                return (IList)GetValue(ItemsSourceProperty);
            }

            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public object SelectedItem
        {
            get
            {
                return GetValue(SelectedItemProperty);
            }

            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        private static void UnselectTab(StackLayout tab)
        {
            tab.Opacity = 0.5;
            tab.BackgroundColor = Color.Transparent;
            tab.Children[tab.Children.Count - 1].IsVisible = false;
        }

        private static void SelectTab(StackLayout tab)
        {
            tab.Opacity = 1.0;
            tab.BackgroundColor = Color.FromHex("#4C4C4C");
            tab.Children[tab.Children.Count - 1].IsVisible = true;
        }

        private void CreateTabs()
        {
            if (Children != null && Children.Count > 0)
            {
                Children.Clear();
            }

            foreach (CarouselViewModel item in ItemsSource)
            {
                var index = Children.Count;
                var tab = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Padding = new Thickness(7)
                };

                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        tab.Children.Add(new Image { Source = item.ImageSource, HeightRequest = 25 });
                        tab.Children.Add(new Label { Text = item.Title, FontSize = 11, HorizontalTextAlignment = TextAlignment.Center });
                        tab.Children.Add(new BoxView { BackgroundColor = Color.White, HeightRequest = 2, HorizontalOptions = LayoutOptions.FillAndExpand, IsVisible = false });
                        break;
                    case Device.iOS:
                        tab.Children.Add(new Image { Source = item.ImageSource, HeightRequest = 20 });
                        tab.Children.Add(new Label { Text = item.Title, FontSize = 11, HorizontalTextAlignment = TextAlignment.Center });
                        tab.Children.Add(new BoxView { BackgroundColor = Color.White, HeightRequest = 2, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.End, IsVisible = false });
                        break;
                }

                var tgr = new TapGestureRecognizer()
                {
                    Command = new Command(() =>
                    {
                        SelectedItem = ItemsSource[index];
                    })
                };
                tab.GestureRecognizers.Add(tgr);
                Children.Add(tab, index, 0);
            }

            if (Children.Count == 0)
            {
                BackgroundColor = Color.Transparent;
            }
            else
            {
                BackgroundColor = TabBackgroundColor;
            }
        }

        private void ItemsSourceChanging()
        {
            if (ItemsSource != null)
            {
                selectedIndex = ItemsSource.IndexOf(SelectedItem);
            }
        }

        private void ItemsSourceChanged()
        {
            if (ItemsSource == null)
            {
                return;
            }

            this.ColumnDefinitions.Clear();
            foreach (var item in ItemsSource)
            {
                this.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }

            if (this.ColumnDefinitions.Count == 0)
            {
                this.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }

            CreateTabs();
        }

        private void SelectedItemChanged()
        {
            var selectedIndex = ItemsSource.IndexOf(SelectedItem);
            var carouselTabs = Children.Cast<StackLayout>().ToList();

            foreach (var ct in carouselTabs)
            {
                UnselectTab(ct);
            }

            if (selectedIndex > -1)
            {
                SelectTab(carouselTabs[selectedIndex]);
            }
        }
    }
}
