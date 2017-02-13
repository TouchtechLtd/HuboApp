using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    public class ShiftTable
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public bool ActiveShift { get; set; }
        public int ServerKey { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public double StartLat { get; set; }
        public double StartLong { get; set; }
        public double EndLat { get; set; }
        public double EndLong { get; set; }
    }
}
