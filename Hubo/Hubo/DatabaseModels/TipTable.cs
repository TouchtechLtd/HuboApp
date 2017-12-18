// <copyright file="TipTable.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using SQLite.Net.Attributes;

    public class TipTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public string TipName { get; set; }

        public int ActiveTip { get; set; }
    }
}
