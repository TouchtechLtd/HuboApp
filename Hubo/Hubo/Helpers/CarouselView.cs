// <copyright file="CarouselView.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public class CarouselView : ScrollView
    {
        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(CarouselView), 0, BindingMode.TwoWay, propertyChanged: async (bindable, oldValue, newValue) => { await ((CarouselView)bindable).UpdateSelectedItem(); });
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(CarouselView), null, propertyChanging: (bindableObject, oldValue, newValue) => { ((CarouselView)bindableObject).ItemsSourceChanging(); }, propertyChanged: (bindableObject, oldValue, newValue) => { ((CarouselView)bindableObject).ItemsSourceChanged(); });
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(CarouselView), null, BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue) => { ((CarouselView)bindable).UpdateSelectedIndex(); });

        private readonly StackLayout stack;

        private int selectedIndex;
        private bool layingOutChildren;

        public CarouselView()
        {
            Orientation = ScrollOrientation.Horizontal;

            stack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 0
            };

            Content = stack;
        }

        public enum IndicatorStyleEnum
        {
            None,
            Dots,
            Tabs
        }

        public DataTemplateSelector ItemTemplate { get; set; }

        public IndicatorStyleEnum IndicatorStyle { get; set; }

        public IList<View> Children
        {
            get
            {
                return stack.Children;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }

            set
            {
                SetValue(SelectedIndexProperty, value);
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

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);
            if (layingOutChildren)
            {
                return;
            }

            layingOutChildren = true;
            foreach (var child in Children)
            {
                child.WidthRequest = width;
            }

            layingOutChildren = false;
        }

        private async Task UpdateSelectedItem()
        {
            await Task.Delay(200);
            SelectedItem = SelectedIndex > -1 ? Children[SelectedIndex].BindingContext : null;
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
            stack.Children.Clear();

            foreach (var item in ItemsSource)
            {
                var view = (View)ItemTemplate.SelectTemplate(item, stack.BindingContext as BindableObject).CreateContent();

                if (view is BindableObject bindableObject)
                {
                    bindableObject.BindingContext = item;
                }

                stack.Children.Add(view);
            }

            if (selectedIndex >= 0)
            {
                SelectedIndex = selectedIndex;
            }
        }

        private void UpdateSelectedIndex()
        {
            if (SelectedItem == BindingContext)
            {
                return;
            }

            SelectedIndex = Children.Select(c => c.BindingContext).ToList().IndexOf(SelectedItem);
        }
    }
}
