// <copyright file="Countdown.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Diagnostics;
    using System.Threading;
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
        private DatabaseService db = new DatabaseService();

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

                message = new MessagingModel()
                {
                    PropertyName = "RemainTime",
                    PropertyValue = remainTime
                };

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

                message = new MessagingModel()
                {
                    PropertyName = "TotalTime",
                    PropertyValue = remainTimeTotal
                };
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

                message = new MessagingModel()
                {
                    PropertyName = "IsRunning",
                    PropertyBool = isRunning
                };
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

            double criticalTime = TotalTime * 0.7;

            db.CancelNotification(NotificationCategory.Ongoing, false);
            db.CreateNotification("You are currently on your break", false, NotificationCategory.Ongoing);

            DependencyService.Get<INotifyService>().UpdateNotification("Break Running", "You are currently on your break", false);

            db.CreateNotification("You have less than {0} mins left in your break", true, NotificationCategory.Break, TimeSpan.FromSeconds(criticalTime));
            Device.StartTimer(TimeSpan.FromSeconds(criticalTime), () =>
            {
                if (!db.CheckTimedNotification(NotificationCategory.Break))
                {
                    return false;
                }

                db.CancelNotification(NotificationCategory.Break, true, true);
                db.CreateNotification("Your break is at an end", true, NotificationCategory.Break, TimeSpan.FromSeconds(TotalTime - criticalTime));

                DependencyService.Get<INotifyService>().UpdateNotification("Break End", "You have less than " + (criticalTime / 60) + " mins left in your break", true);

                Device.StartTimer(TimeSpan.FromSeconds(TotalTime - criticalTime), () =>
                {
                    if (!db.CheckTimedNotification(NotificationCategory.Break))
                    {
                        return false;
                    }

                    DependencyService.Get<INotifyService>().UpdateNotification("Break End", "Your break is at an end", true);

                    db.CancelNotification(NotificationCategory.Break, true, true);
                    return false;
                });
                return false;
            });

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

            double criticalTime = RemainTime * 0.7;

            if (!db.CheckTimedNotification(NotificationCategory.Ongoing))
            {
                db.CreateNotification("You are currently on your break", false, NotificationCategory.Ongoing);
            }

            DependencyService.Get<INotifyService>().UpdateNotification("Break Running", "You are currently on your break", false);

            if (!db.CheckTimedNotification(NotificationCategory.Break))
            {
                db.CreateNotification("You have less than {0} mins left in your break", true, NotificationCategory.Break, TimeSpan.FromSeconds(criticalTime));
            }

            Device.StartTimer(TimeSpan.FromSeconds(criticalTime), () =>
            {
                if (!db.CheckTimedNotification(NotificationCategory.Break))
                {
                    return false;
                }

                db.CancelNotification(NotificationCategory.Break, true, true);
                db.CreateNotification("Your break is at an end", true, NotificationCategory.Break, TimeSpan.FromSeconds(TotalTime - criticalTime));

                DependencyService.Get<INotifyService>().UpdateNotification("Break End", "You have less than " + (criticalTime / 60) + " mins left in your break", true);

                Device.StartTimer(TimeSpan.FromSeconds(TotalTime - criticalTime), () =>
                {
                    if (!db.CheckTimedNotification(NotificationCategory.Break))
                    {
                        return false;
                    }

                    DependencyService.Get<INotifyService>().UpdateNotification("Break End", "Your break is at an end", true);

                    db.CancelNotification(NotificationCategory.Break, true, true);
                    return false;
                });
                return false;
            });

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

                db.CancelNotification(NotificationCategory.Ongoing, false);
                db.CreateNotification("Your shift is running", false, NotificationCategory.Ongoing);

                DependencyService.Get<INotifyService>().UpdateNotification("Shift Running", "Your Shift is Running", false);

                db.CancelNotification(NotificationCategory.Break, true);
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

                this.Stop();
            }

            if (((RemainTime / TotalTime) * 100) < 10 && !warningGiven)
            {
                CountdownConverter convert = new CountdownConverter();
                toastConfig = new ToastConfig("You have " + convert.Convert(RemainTime, null, null, null) + " min left in your break");
                UserDialogs.Instance.Toast(toastConfig);

                warningGiven = true;
            }
        }
    }
}