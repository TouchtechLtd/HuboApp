using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    class ShiftOffline
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int ShiftKey { get; set; }
    }

    class DriveOffline
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int DriveKey { get; set; }
    }

    class BreakOffline
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int BreakKey { get; set; }
    }

    class NoteOffline
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int NoteKey { get; set; }
    }

    class GeolocationOffline
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int DriveId { get; set; }
        public string TimeStamp { get; set; }
        public double Lat { get; set; }
        public double Longitude { get; set; }
    }

    class VehicleOffline
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int VehicleKey { get; set; }
    }

    class UserOffline
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int UserKey { get; set; }
    }

    class CompanyOffline
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int CompanyKey { get; set; }
    }
}
