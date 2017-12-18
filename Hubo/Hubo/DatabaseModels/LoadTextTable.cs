// <copyright file="LoadTextTable.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using SQLite.Net.Attributes;

    internal class LoadTextTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public string LoadText { get; set; }
    }
}
