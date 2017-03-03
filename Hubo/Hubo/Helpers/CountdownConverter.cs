//
//  CountdownConverter.cs
//  Created by Alexey Kinev on 11 Jan 2015.
//
//    Licensed under The MIT License (MIT)
//    http://opensource.org/licenses/MIT
//
//    Copyright (c) 2015 Alexey Kinev <alexey.rudy@gmail.com>
//
using System;
using Xamarin.Forms;

namespace Hubo
{
    /// <summary>
    /// Converts countdown seconds double value to string "HH : MM : SS"
    /// </summary>
    public class CountdownConverter : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var timespan = TimeSpan.FromSeconds((double)value);

            if (timespan.TotalSeconds < 1.0)
            {
                return "-- : --";
            }
            else if (timespan.TotalSeconds < 3600)
            {
                return string.Format("{0:D2} : {1:D2}",
                    timespan.Minutes, timespan.Seconds);
            }
            else if (timespan.TotalSeconds > 3600 * 24)
            {
                return "24 : 00 : 00";
            }

            return string.Format("{0:D2} : {1:D2} : {2:D2}",
                timespan.Hours, timespan.Minutes, timespan.Seconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}


