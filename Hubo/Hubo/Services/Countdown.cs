//
//  Countdown.cs
//  Created by Alexey Kinev on 11 Jan 2015.
//
//    Licensed under The MIT License (MIT)
//    http://opensource.org/licenses/MIT
//
//    Copyright (c) 2015 Alexey Kinev <alexey.rudy@gmail.com>
//
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Hubo
{
    /// <summary>
    /// Countdown timer with periodical ticks.
    /// </summary>
    public class Countdown : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the start date time.
        /// </summary>
        public DateTime StartDateTime { get; private set; }

        public static Stopwatch sw = new Stopwatch();

        /// <summary>
        /// Gets the remain time in seconds.
        /// </summary>
        public double RemainTime
        {
            get { return remainTime; }

            private set
            {
                remainTime = value;
                OnPropertyChanged();
            }
        }

        public double TotalTime
        {
            get { return remainTimeTotal; }
            private set
            {
                remainTimeTotal = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The remain time.
        /// </summary>
        double remainTime;

        /// <summary>
        /// The remain time total.
        /// </summary>
        double remainTimeTotal;

        public Countdown()
        {
            RemainTime = 0;

            TotalTime = 10;
        }

        /// <summary>
        /// Starts the updating with specified period, total time and period are specified in seconds.
        /// </summary>
        public void StartUpdating(double total)
        {
            if (sw.IsRunning)
            {
                StopUpdating();
                return;
            }

            TotalTime = total;
            RemainTime = total;

            StartDateTime = DateTime.Now;

            sw.Start();

            Device.StartTimer(TimeSpan.FromMilliseconds(200), () =>
            {
                Tick();
                return sw.IsRunning;
            });
        }

        /// <summary>
        /// Stops the updating.
        /// </summary>
        public void StopUpdating()
        {
            if (sw.IsRunning)
            {
                sw.Stop();
                sw.Reset();
            }
        }

        /// <summary>
        /// Updates the time remain.
        /// </summary>
        public void Tick()
        {
            var delta = (DateTime.Now - StartDateTime).TotalSeconds;

            if (delta < remainTimeTotal)
            {
                RemainTime = remainTimeTotal - delta;
            }
            else
            {
                RemainTime = 0;

                sw.Stop();
                sw.Reset();
            }
        }

        #region INotifyPropertyChanged implementation

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}

