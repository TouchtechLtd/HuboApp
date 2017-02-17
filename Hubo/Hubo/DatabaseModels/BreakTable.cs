using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    class BreakTable
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public string  StartDate { get; set; }
        public string EndDate { get; set; }
        public int ShiftKey { get; set; }
        public bool ActiveBreak { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public int ServerId { get; set; }
    }
}
