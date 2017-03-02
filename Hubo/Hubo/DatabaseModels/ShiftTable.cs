// <copyright file="ShiftTable.cs" company="TrioTech">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Hubo
{
    using SQLite.Net.Attributes;

    public class ShiftTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public bool ActiveShift { get; set; }

        public int ServerKey { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public double StartLat { get; set; }

        public double StartLong { get; set; }

        public double EndLat { get; set; }

        public double EndLong { get; set; }

        public string StartLocation { get; set; }

        public string EndLocation { get; set; }
    }
}
