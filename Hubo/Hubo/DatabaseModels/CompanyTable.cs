using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    public class CompanyTable
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Suburb { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int DriverId { get; set; }
    }
}
