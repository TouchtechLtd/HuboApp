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
        public string  StartTime { get; set; }
        public string EndTime { get; set; }
        public int ShiftKey { get; set; }
        public bool ActiveBreak { get; set; }
        public int StartNoteKey { get; set; }
        public int StopNoteKey { get; set; }

    }
}
