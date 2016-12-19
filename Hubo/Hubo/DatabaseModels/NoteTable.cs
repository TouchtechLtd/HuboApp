using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    class NoteTable
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public string Note { get; set; }
        public string Date { get; set; }
        public int ShiftKey { get; set; }
        public int Hubo { get; set; }
        public string Location { get; set; }
        public bool StandAloneNote { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
