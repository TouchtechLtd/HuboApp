using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    class AmendmentTable
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int ShiftId { get; set; }
        public string Table { get; set; }
        public string Field { get; set; }
        public string TimeStamp { get; set; }
        public string BeforeValue { get; set; }
    }
}
