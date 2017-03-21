// <copyright file="BottomBarPageExtensions.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using Xamarin.Forms;

    public static class BottomBarPageExtensions
    {
        public static readonly BindableProperty TabColorProperty = BindableProperty.CreateAttached(
            propertyName: "TabColor",
            returnType: typeof(Color?),
            declaringType: typeof(Page),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay,
            validateValue: null,
            propertyChanged: null);

        public static void SetTabColor(this Page page, Color? color)
        {
            page.SetValue(TabColorProperty, color);
        }

        public static Color? GetTabColor(this Page page)
        {
            return (Color?)page.GetValue(TabColorProperty);
        }
    }
}
