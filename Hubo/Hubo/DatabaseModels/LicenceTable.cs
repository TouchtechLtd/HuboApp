using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    public class LicenceTable
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int DriverId { get; set; }
        public string LicenceNumber { get; set; }
        public int LicenceVersion { get; set; }
        public string Endorsements { get; set; }
    }
}
