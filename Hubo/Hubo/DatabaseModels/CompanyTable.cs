// <copyright file="CompanyTable.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using SQLite.Net.Attributes;

    public class CompanyTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        public string Name { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public int PostCode { get; set; }

        public string Suburb { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public int DriverId { get; set; }

        public int ServerId { get; set; }
    }
}
