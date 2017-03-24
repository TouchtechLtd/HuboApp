// <copyright file="NotificationTable.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using SQLite.Net.Attributes;

    public enum NotificationCategory
    {
        Ongoing,
        Shift,
        Break
    }

    internal class NotificationTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public string Text { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.Now;

        public TimeSpan FireTime { get; set; } = TimeSpan.FromMinutes(0);

        public NotificationCategory Category { get; set; }

        public bool Canceled { get; set; } = false;

        public bool Fired { get; set; } = false;
    }
}
