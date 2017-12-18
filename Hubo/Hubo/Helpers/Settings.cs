// <copyright file="Settings.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using Plugin.Settings;
    using Plugin.Settings.Abstractions;

    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters.
    /// </summary>
    public static class Settings
    {
        private const string HamburgerKey = "hamburger_key";
        private const string DarkLightKey = "darkLight_key";

        private static readonly bool HamburgerDefault = false;
        private static readonly bool DarkLightDefault = false;

        public static bool HamburgerSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault<bool>(HamburgerKey, HamburgerDefault);
            }

            set
            {
                AppSettings.AddOrUpdateValue<bool>(HamburgerKey, value);
            }
        }

        public static bool DarkLightSetting
        {
            get
            {
                return AppSettings.GetValueOrDefault<bool>(DarkLightKey, DarkLightDefault);
            }

            set
            {
                AppSettings.AddOrUpdateValue<bool>(DarkLightKey, value);
            }
        }

        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }
    }
}