// <copyright file="DriveTable.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using SQLite.Net.Attributes;

    public class DriveTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public int ShiftKey { get; set; }

        public int VehicleKey { get; set; }

        public bool ActiveVehicle { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public int StartHubo { get; set; }

        public int EndHubo { get; set; }

        public int ServerId { get; set; }

        public TimeSpan TimeSinceBreak { get; set; }

        public string StartNote { get; set; }

        public string EndNote { get; set; }

        public int LocalKey { get; set; }
    }
}
