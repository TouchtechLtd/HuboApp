// <copyright file="DayShiftTable.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using SQLite.Net.Attributes;

    internal class DayShiftTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public int ServerKey { get; set; }

        public string DayShiftStart { get; set; }

        public bool IsActive { get; set; }
    }
}
