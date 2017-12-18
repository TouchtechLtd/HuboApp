// <copyright file="CustomTableView.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using Xamarin.Forms;

    public partial class CustomTableView : TableView
    {
        public static BindableProperty HeaderTextColorProperty = BindableProperty.Create("HeaderTextColor", typeof(Color), typeof(CustomTableView), Color.FromHex("#70dceb"));
        public static BindableProperty TextColorProperty = BindableProperty.Create("TextColor", typeof(Color), typeof(CustomTableView), Color.White);

        public static BindableProperty SectionSeparatorColorProperty = BindableProperty.Create("SectionSeparatorColor", typeof(Color), typeof(CustomTableView), Color.FromHex("#70dceb"));
        public static BindableProperty SeparatorColorProperty = BindableProperty.Create("SeparatorColor", typeof(Color), typeof(CustomTableView), Color.White);

        public CustomTableView()
        {
        }

        public Color HeaderTextColor
        {
            get
            {
                return (Color)GetValue(HeaderTextColorProperty);
            }

            set
            {
                SetValue(HeaderTextColorProperty, value);
            }
        }

        public Color TextColor
        {
            get
            {
                return (Color)GetValue(TextColorProperty);
            }

            set
            {
                SetValue(TextColorProperty, value);
            }
        }

        public Color SectionSeparatorColor
        {
            get
            {
                return (Color)GetValue(SectionSeparatorColorProperty);
            }

            set
            {
                SetValue(SectionSeparatorColorProperty, value);
            }
        }

        public Color SeparatorColor
        {
            get
            {
                return (Color)GetValue(SeparatorColorProperty);
            }

            set
            {
                SetValue(SeparatorColorProperty, value);
            }
        }
    }
}
