using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    public class TipTable
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public string TipName { get; set; }
        public int ActiveTip { get; set; }
    }
}
