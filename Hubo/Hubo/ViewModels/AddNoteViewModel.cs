using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    class AddNoteViewModel
    {
        public string SaveText { get; set; }
        public AddNoteViewModel()
        {
            SaveText = Resource.Save;
        }
    }
}
