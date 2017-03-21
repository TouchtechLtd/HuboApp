// <copyright file="BottomBarPage.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using Xamarin.Forms;

    public class BottomBarPage : TabbedPage
    {
        public enum BarThemeTypes
        {
            Light, DarkWithAlpha, DarkWithoutAlpha
        }

        public bool FixedMode { get; set; }

        public BarThemeTypes BarTheme { get; set; }

        public void RaiseCurrentPageChanged()
        {
            OnCurrentPageChanged();
        }
    }
}
