using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    class LoadTextTable
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public string LoadText { get; set; }
    }
}
