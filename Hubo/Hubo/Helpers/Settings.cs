// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Hubo.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string HamburgerKey = "hamburger_key";
        private static readonly bool HamburgerDefault = false;

        private const string DarkLightKey = "darkLight_key";
        private static readonly bool DarkLightDefault = false;

        #endregion


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

    }
}