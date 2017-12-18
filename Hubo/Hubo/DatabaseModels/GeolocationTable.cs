// <copyright file="GeolocationTable.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using SQLite.Net.Attributes;

    internal class GeolocationTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public int DriveKey { get; set; }

        public string TimeStamp { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
