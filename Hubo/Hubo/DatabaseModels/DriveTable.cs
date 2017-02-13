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
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int StartHubo { get; set; }
        public int EndHubo { get; set; }
        public int ServerId { get; set; }
    }
}
