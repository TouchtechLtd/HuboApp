// <copyright file="CountdownConverter.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using Xamarin.Forms;

    /// <summary>
    /// Converts countdown seconds double value to string "HH : MM : SS"
    /// </summary>
    public class CountdownConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var timespan = TimeSpan.FromSeconds((double)value);

            if (timespan.TotalSeconds < 1.0)
            {
                return "-- : --";
            }
            else if (timespan.TotalSeconds < 3600)
            {
                return string.Format(
                    "{0:D2} : {1:D2}",
                    timespan.Minutes,
                    timespan.Seconds);
            }
            else if (timespan.TotalSeconds > 3600 * 24)
            {
                return "24 : 00 : 00";
            }

            return string.Format(
                "{0:D2} : {1:D2} : {2:D2}",
                timespan.Hours,
                timespan.Minutes,
                timespan.Seconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}