// <copyright file="Countdown.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Xamarin.Forms;

    /// <summary>
    /// Countdown timer with periodical ticks.
    /// </summary>
    internal class Countdown : INotifyPropertyChanged
    {
        private static Stopwatch sw = new Stopwatch();

        /// <summary>
        /// The remain time.
        /// </summary>
        private double remainTime;

        /// <summary>
        /// The remain time total.
        /// </summary>
        private double remainTimeTotal;

        private DateTime startDateTime;

        private bool isRunning;

        private bool warningGiven;

        public Countdown()
        {
            RemainTime = 0;

            TotalTime = 10;

            IsRunning = false;

            WarningGiven = false;
        }

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets gets the start date time.
        /// </summary>
        public DateTime StartDateTime
        {
            get
            {
                return startDateTime;
            }

            private set
            {
                startDateTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the remain time in seconds.
        /// </summary>
        public double RemainTime
        {
            get
            {
                return remainTime;
            }

             private set
            {
                remainTime = value;
                OnPropertyChanged("RemainTime");
            }
        }

        public double TotalTime
        {
            get
            {
                return remainTimeTotal;
            }

            private set
            {
                remainTimeTotal = value;
                OnPropertyChanged("TotalTime");
            }
        }

        public bool IsRunning
        {
            get
            {
                return isRunning;
            }

            private set
            {
                isRunning = value;
                OnPropertyChanged("IsRunning");
            }
        }

        public bool WarningGiven
        {
            get
            {
                return warningGiven;
            }

            private set
            {
                warningGiven = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Starts the updating with specified period, total time and period are specified in seconds.
        /// </summary>
        /// <param name="total"> the total time for the timer</param>
        public void Start(double total)
        {
            if (IsRunning)
            {
                Stop();
                return;
            }

            TotalTime = total;
            RemainTime = total;

            StartDateTime = DateTime.Now;

            sw.Start();

            IsRunning = sw.IsRunning;

            Device.StartTimer(TimeSpan.FromMilliseconds(200), () =>
            {
                Tick();
                return IsRunning;
            });
        }

        public void Restart(double total, DateTime startTime)
        {
            if (IsRunning)
            {
                Stop();
                return;
            }

            TotalTime = total;

            StartDateTime = startTime;

            RemainTime = total - startTime.TimeOfDay.TotalMinutes;

            sw.Start();

            IsRunning = sw.IsRunning;

            Device.StartTimer(TimeSpan.FromMilliseconds(200), () =>
            {
                Tick();
                return IsRunning;
            });
        }

        /// <summary>
        /// Stops the updating.
        /// </summary>
        public void Stop()
        {
            if (IsRunning)
            {
                sw.Stop();
                sw.Reset();

                IsRunning = sw.IsRunning;
            }
        }

        /// <summary>
        /// Updates the time remain.
        /// </summary>
        public void Tick()
        {
            var delta = (DateTime.Now - StartDateTime).TotalSeconds;

            if (delta < TotalTime)
            {
                RemainTime = TotalTime - delta;
            }
            else
            {
                RemainTime = 0;

                sw.Stop();
                sw.Reset();

                IsRunning = sw.IsRunning;
            }

            if (RemainTime < (5 * 60) && !WarningGiven)
            {
                Application.Current.MainPage.DisplayAlert("Break End", "You have less than 5 min left in your break", Resource.DisplayAlertOkay);
                WarningGiven = true;
            }
        }

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property changed</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}