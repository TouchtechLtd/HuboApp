// <copyright file="BreakTable.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using SQLite.Net.Attributes;

    public class BreakTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public int ShiftKey { get; set; }

        public bool ActiveBreak { get; set; }

        public string StartLocation { get; set; }

        public string EndLocation { get; set; }

        public int ServerId { get; set; }

        public string StartNote { get; set; }

        public string EndNote { get; set; }
    }
}
