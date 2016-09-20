using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo.Model
{
    class ApiResponse
    {
        public bool Success { get; set; }
        public string Result { get; set; }
        public bool UnAuthorizedRequest { get; set; }

    }
}
