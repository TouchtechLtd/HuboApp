// <copyright file="AmendmentTable.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using SQLite.Net.Attributes;

    internal class AmendmentTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public int ShiftId { get; set; }

        public int DriveId { get; set; }

        public string Table { get; set; }

        public string Field { get; set; }

        public string TimeStamp { get; set; }

        public string BeforeValue { get; set; }

        public string AfterValue { get; set; }
    }
}
