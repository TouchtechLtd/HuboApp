// <copyright file="NoteTable.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using SQLite.Net.Attributes;

    public class NoteTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public string Note { get; set; }

        public string Date { get; set; }

        public int ShiftKey { get; set; }
    }
}
