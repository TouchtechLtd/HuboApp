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
        public bool StartOffline { get; set; }
        public bool EndOffline { get; set; }
    }

    class DriveOffline
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int DriveKey { get; set; }
        public bool StartOffline { get; set; }
        public bool EndOffline { get; set; }
    }

    class BreakOffline
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int BreakKey { get; set; }
        public bool StartOffline { get; set; }
        public bool EndOffline { get; set; }
    }

    class NoteOffline
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int NoteKey { get; set; }
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
