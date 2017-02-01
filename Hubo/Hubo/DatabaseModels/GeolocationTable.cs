using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    class GeolocationTable
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int ShiftKey { get; set; }
        public string TimeStamp { get; set; }
        public double latitude { get; set; }
        public double Longitude { get; set; }
    }
}
