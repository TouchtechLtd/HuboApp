using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    public class DriveTable
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int ShiftKey { get; set; }
        public int VehicleKey { get; set; }
        public bool ActiveVehicle { get; set; }
        public int StartNoteKey { get; set; }
        public int EndNoteKey { get; set; }
    }
}
