using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    public class VehicleTable
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public string Registration { get; set; }
        public string MakeModel { get; set; }
        public int CompanyId { get; set; }
        public string StartingOdometer { get; set; }
        public string CurrentOdometer { get; set; }
    }
}
