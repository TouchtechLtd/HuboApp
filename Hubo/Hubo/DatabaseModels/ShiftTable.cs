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
        public int StartShiftNoteId { get; set; }
        public int StopShiftNoteId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
