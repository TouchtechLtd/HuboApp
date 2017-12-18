// <copyright file="LicenceTable.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using SQLite.Net.Attributes;

    public class LicenceTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public int DriverId { get; set; }

        public string LicenceNumber { get; set; }

        public int LicenceVersion { get; set; }

        public string Endorsements { get; set; }
    }
}
