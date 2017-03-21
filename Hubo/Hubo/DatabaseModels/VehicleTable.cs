// <copyright file="VehicleTable.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using SQLite.Net.Attributes;

    public class VehicleTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public string Registration { get; set; }

        public string MakeModel { get; set; }

        public string FleetNumber { get; set; }

        public int CompanyId { get; set; }

        public int ServerKey { get; set; }
    }
}
