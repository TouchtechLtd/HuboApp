using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    public class VehicleInUseTable
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int ShiftKey { get; set; }
        public int VehicleKey { get; set; }
        public int ActiveVehicle { get; set; }
        public int HuboStart { get; set; }
        public int HuboEnd { get; set; }
    }
}
