// <copyright file="Countdown.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Diagnostics;
    using Acr.UserDialogs;
    using Xamarin.Forms;

    /// <summary>
    /// Countdown timer with periodical ticks.
    /// </summary>
    internal class Countdown
    {
        private static Stopwatch sw = new Stopwatch();
        private MessagingModel message = new MessagingModel();
        private ToastConfig toastConfig;
        private int notificationId;

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

            IsRunning = sw.IsRunning;

            warningGiven = false;

            notificationId = 10;
        }

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

                message = new MessagingModel();
                message.PropertyName = "RemainTime";
                message.PropertyValue = remainTime;

                MessagingCenter.Send<string, MessagingModel>("Countdown Update", "CountDown Update", message);
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

                message = new MessagingModel();
                message.PropertyName = "TotalTime";
                message.PropertyValue = remainTimeTotal;

                MessagingCenter.Send<string, MessagingModel>("Countdown Update", "CountDown Update", message);
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

                message = new MessagingModel();
                message.PropertyName = "IsRunning";
                message.PropertyBool = isRunning;

                MessagingCenter.Send<string, MessagingModel>("Countdown Update", "CountDown Update", message);
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

            CountdownConverter convert = new CountdownConverter();
            DependencyService.Get<INotifyService>().LocalNotification("Break End", "You have " + convert.Convert(RemainTime, null, null, null) + " min left in your break", DateTime.Now + TimeSpan.FromSeconds(total), notificationId);

            IsRunning = sw.IsRunning;
            warningGiven = false;

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
                warningGiven = false;
                return;
            }

            TotalTime = total;

            StartDateTime = startTime;

            RemainTime = total - startTime.TimeOfDay.TotalMinutes;

            sw.Start();

            IsRunning = sw.IsRunning;
            warningGiven = false;

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

                DependencyService.Get<INotifyService>().CancelNotification(notificationId);
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

            if (RemainTime < (5 * 60) && !warningGiven)
            {
                CountdownConverter convert = new CountdownConverter();
                toastConfig = new ToastConfig("You have " + convert.Convert(RemainTime, null, null, null) + " min left in your break");
                UserDialogs.Instance.Toast(toastConfig);

                warningGiven = true;
            }
        }
    }
}