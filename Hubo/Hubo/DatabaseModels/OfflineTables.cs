// <copyright file="OfflineTables.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using SQLite.Net.Attributes;

    internal class OfflineTables
    {
    }

    internal class DayShiftOffline
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public int DayShiftKey { get; set; }
    }

    internal class ShiftOffline
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public int ShiftKey { get; set; }

        public bool StartOffline { get; set; }

        public bool EndOffline { get; set; }
    }

    internal class DriveOffline
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public int DriveKey { get; set; }

        public bool StartOffline { get; set; }

        public bool EndOffline { get; set; }
    }

    internal class BreakOffline
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public int BreakKey { get; set; }

        public bool StartOffline { get; set; }

        public bool EndOffline { get; set; }
    }

    internal class NoteOffline
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public int NoteKey { get; set; }
    }

    internal class VehicleOffline
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public int VehicleKey { get; set; }
    }

    internal class UserOffline
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public int UserKey { get; set; }
    }

    internal class CompanyOffline
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public int CompanyKey { get; set; }
    }
}
